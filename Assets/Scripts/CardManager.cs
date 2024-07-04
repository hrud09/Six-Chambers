using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using TMPro;

public enum CardType
{
    Clove,
    Heart,
    Dice,
    Spade
}

[System.Serializable]
public class CardInfo
{
    public CardType CardType;
    public int cardNumber;
    public Sprite cardTexture;
}

[System.Serializable]
public class SignWiseCard
{
    public CardType CardType;
    public Sprite[] sprites;
}

public class CardManager : MonoBehaviour
{
    public GameObject cardPrefab;

    public List<SignWiseCard> signWiseCards;

    public List<CardInfo> cardsInDeck;
    public List<CardInfo> tempDeck;
    public List<CardInfo> cardsInHands;

    public List<Transform> cardsOnHands;

    public ChamberManager chamberManager;
    public BoardManager boardManager;
    public Transform cardSpawnPos;
    public GameObject showCardButton;

    public TMP_Text winText;

    void Start()
    {
        foreach (var cardDeck in signWiseCards)
        {
            foreach (var cardSprite in cardDeck.sprites)
            {
                CardInfo cardInfo = new CardInfo
                {
                    cardTexture = cardSprite,
                    CardType = cardDeck.CardType,
                    cardNumber = cardDeck.sprites.ToList().IndexOf(cardSprite) + 1
                };
                cardsInDeck.Add(cardInfo);
            }
        }
        tempDeck = new List<CardInfo>(cardsInDeck);
        StartCoroutine(DrawCardsForChambers());
        StartCoroutine(DrawCardsOnBoard());
    }

    private CardInfo DrawRandomCard()
    {
        int index = Random.Range(0, tempDeck.Count);
        CardInfo cardInfo = tempDeck[index];
        tempDeck.RemoveAt(index);

        return cardInfo;
    }

    public IEnumerator DrawCardsForChambers()
    {
        foreach (var chamberTransform in chamberManager.chamberTransforms)
        {
            Chamber chamber = chamberTransform.GetComponent<Chamber>();
            CardInfo cardInfo1 = DrawRandomCard();
            CardInfo cardInfo2 = DrawRandomCard();


            cardsInHands.Add(cardInfo1);
            cardsInHands.Add(cardInfo2);

            Vector3 targetDirection = (chamberTransform.parent.position - chamberTransform.position).normalized;
            targetDirection.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

           

            GameObject card1 = Instantiate(cardPrefab, cardSpawnPos.position, targetRotation, chamber.cardParent);
            GameObject card2 = Instantiate(cardPrefab, cardSpawnPos.position, targetRotation, chamber.cardParent);
            cardsOnHands.Add(card1.transform);
            cardsOnHands.Add(card2.transform);
            card1.GetComponent<Card>().InitiateCard(cardInfo1);
            card2.GetComponent<Card>().InitiateCard(cardInfo2);

            chamber.chamberCards.Add(card2.GetComponent<Card>());
            chamber.chamberCards.Add(card1.GetComponent<Card>());

            Vector3 pos1 = chamber.cardParent.position;
            Vector3 pos2 = pos1 - chamberTransform.forward * 1f + Vector3.up * 0.2f;

            if (chamberTransform.localPosition.x > 0) card1.transform.localRotation = Quaternion.Euler(new Vector3(0, 20, 180));
            else card1.transform.localRotation = Quaternion.Euler(new Vector3(0, -20, 180));


            card1.transform.DOMove(pos1, 1f);
           
            card2.transform.DOMove(pos2, 1f).OnComplete(() => {
               // card2.transform.DORotateQuaternion(targetRotation, 0.5f); // Smoothly rotate towards the target position
            });
            yield return new WaitForSeconds(0.2f);
        }
    }

    public IEnumerator DrawCardsOnBoard()
    {
        for (int i = 0; i < boardManager.boardCardParents.Length; i++)
        {
            CardInfo cardInfo = DrawRandomCard();
            GameObject card = Instantiate(cardPrefab, cardSpawnPos.position, Quaternion.Euler(0, 0, 180), boardManager.boardCardParents[i]);
            card.GetComponent<Card>().InitiateCard(cardInfo);

            Vector3 pos = boardManager.boardCardParents[i].position;
            boardManager.cardsOnBoard.Add(card);
            boardManager.cards.Add(card.GetComponent<Card>());
            int index = i;
            card.transform.DOMove(pos, 1f).OnComplete(() =>
            {
                if (index < 3) card.transform.DOLocalRotate(Vector3.zero, 1f);
            });

            yield return new WaitForSeconds(0.2f);
        }

        yield return new WaitForSeconds(1);
        chamberManager.playerChipsManager.playersTurn = true;
    }

    public void RevealLastTwoCards()
    {
        for (int i = 0; i < boardManager.cardsOnBoard.Count; i++)
        {
            int index = i;
            Transform card = boardManager.cardsOnBoard[i].transform;

            card.DOLocalRotate(Vector3.zero, 0.5f).SetDelay(0.2f).OnComplete(() =>
            {
                if (index == boardManager.cardsOnBoard.Count - 1)
                {
                    RevealEveryOnesCards();
                }
            });
        }
    }

    public void RevealEveryOnesCards()
    {
        foreach (Transform t in cardsOnHands)
        {
            int index = cardsOnHands.IndexOf(t);
            t.DOLocalRotate(Vector3.zero, 0.5f).SetDelay(cardsOnHands.IndexOf(t) * 0.3f).OnComplete(() =>
            {
                if (index == cardsOnHands.Count - 1)
                {
                    CheckPokerLogics();
                }
            });
        }
    }
    void CheckPokerLogics()
    {
        List<Card> cardsOnBoard = boardManager.cards;
        Chamber winningChamber = null;
        List<Chamber> winningChambers;
        int highestHandValue = -1;
        List<Chamber> tiedChambers = new List<Chamber>();

        foreach (Transform t in chamberManager.chamberTransforms)
        {
            List<Card> chamberCards = new List<Card>(cardsOnBoard);
            Chamber chamberSelected = t.GetComponent<Chamber>();
            chamberCards.AddRange(chamberSelected.chamberCards);

            HandEvaluation handEvaluation = EvaluateHand(chamberCards);
            int handValue = handEvaluation.HandValue;
            print("Hand Value: " + handValue);
            foreach (Card card in handEvaluation.HighValueCards)
            {
                GameObject newCard = Instantiate(card.gameObject, card.gameObject.transform.position, card.transform.rotation, chamberSelected.bestCardsHolders[handEvaluation.HighValueCards.IndexOf(card)]);
                newCard.transform.DOScale(Vector3.one, 0.5f);
                newCard.transform.DOLocalMove(Vector3.zero, 1f).OnComplete(() => {

                    newCard.transform.DOLocalRotate(Vector3.zero, 0.5f);
                });
            }
            if (handValue > highestHandValue)
            {
                highestHandValue = handValue;
                winningChamber = chamberSelected;
                tiedChambers.Clear();
                tiedChambers.Add(winningChamber);
            }
            else if (handValue == highestHandValue)
            {
                tiedChambers.Add(chamberSelected);
            }
            print("Highest Hand Value: " + highestHandValue);
        }

        if (tiedChambers.Count == 1)
        {
            HandEvaluation winningHand = EvaluateHand(new List<Card>(cardsOnBoard.Concat(winningChamber.chamberCards)));
            string highValueCards = string.Join(", ", winningHand.HighValueCards.Select(card => card.cardInfo.cardNumber.ToString()).ToArray());
            winText.text = $"{winningChamber.index} Wins by {winType(highestHandValue)} with High Cards: {highValueCards}";
        }
        else
        {
            string tiedChambersIndices = string.Join(", ", tiedChambers.Select(chamber => chamber.index.ToString()).ToArray());
            winningChambers = BreakTie(tiedChambers, highestHandValue);
            if (winningChambers.Count == 1)
            {
                HandEvaluation winningHand = EvaluateHand(new List<Card>(cardsOnBoard.Concat(winningChambers[0].chamberCards)));


                string highValueCards = string.Join(", ", winningHand.HighValueCards.Select(card => card.cardInfo.cardNumber.ToString()).ToArray());
                winText.text = $"{winningChambers[0].index} Wins by Tie-Breaker among Chambers: {tiedChambersIndices} with High Cards: {highValueCards}\nWins by {winType(highestHandValue)}";
            }
            else
            {
                string message = string.Join(", ", winningChambers.Select(chamber => chamber.index.ToString()).ToArray());
                message += " Wins by " + winType(highestHandValue).ToString();
                winText.text = message;
            }
        }
    }
    List<Chamber> BreakTie(List<Chamber> tiedChambers, int handValue)
    {
        List<Chamber> tiedWinners = new List<Chamber> { tiedChambers[0] };

        // Get the sorted list of cards from the first tied chamber
        List<Card> winningCards = new List<Card>(boardManager.cards);
        winningCards.AddRange(tiedWinners[0].chamberCards);
        winningCards = winningCards.OrderByDescending(card => card.cardInfo.cardNumber).ToList();

        // Iterate through the rest of the tied chambers to compare their hands
        foreach (var chamber in tiedChambers.Skip(1))
        {
            List<Card> currentCards = new List<Card>(boardManager.cards);
            currentCards.AddRange(chamber.chamberCards);
            currentCards = currentCards.OrderByDescending(card => card.cardInfo.cardNumber).ToList();

            bool isTie = true;
            for (int i = 0; i < winningCards.Count; i++)
            {
                int winningCardValue = winningCards[i].cardInfo.cardNumber;
                int currentCardValue = currentCards[i].cardInfo.cardNumber;

                if (winningCardValue > currentCardValue)
                {
                    isTie = false;
                    break;
                }
                else if (winningCardValue < currentCardValue)
                {
                    tiedWinners.Clear();
                    tiedWinners.Add(chamber);
                    winningCards = currentCards;
                    isTie = false;
                    break;
                }
            }

            if (isTie)
            {
                tiedWinners.Add(chamber);
            }
        }

        return tiedWinners;
    }

    public class HandEvaluation
    {
        public int HandValue { get; set; }
        public List<Card> HighValueCards { get; set; }

        public HandEvaluation(int handValue, List<Card> highValueCards)
        {
            HandValue = handValue;
            HighValueCards = highValueCards;
        }
    }


    HandEvaluation EvaluateHand(List<Card> cards)
    {
        // Order cards by number, treating Ace as the highest card by default
        cards = cards.OrderBy(card => card.cardInfo.cardNumber).ToList();

        //bool isFlush = cards.All(card => card.cardInfo.CardType == cards[0].cardInfo.CardType);
        bool isFlush = true;
        for (int i = 1; i < cards.Count; i++)
        {
            if (cards[i].cardInfo.CardType != cards[i - 1].cardInfo.CardType)
            {
                isFlush = false;
                break;
            }
        }
        bool isRoyalFlush = cards[0].cardInfo.cardNumber == 1 && cards[1].cardInfo.cardNumber == 10 && cards[2].cardInfo.cardNumber == 11 && cards[3].cardInfo.cardNumber == 12 && cards[4].cardInfo.cardNumber == 13 && isFlush;

        bool isStraight = true;
        for (int i = 1; i < cards.Count; i++)
        {
            if (cards[0].cardInfo.cardNumber == 1 )
            {
                if ((cards[1].cardInfo.cardNumber != 10) || cards[1].cardInfo.cardNumber != 2)
                {
                    isStraight = false;
                    break;
                }
            }
            else if (cards[i].cardInfo.cardNumber != cards[i - 1].cardInfo.cardNumber + 1)
            {
                isStraight = false;
                break;
            }
        }

        // Check for Ace-low straight (Ace-2-3-4-5)
        bool isAceLowStraight = cards[0].cardInfo.cardNumber == 1 &&
                                cards[1].cardInfo.cardNumber == 2 &&
                                cards[2].cardInfo.cardNumber == 3 &&
                                cards[3].cardInfo.cardNumber == 4 &&
                                cards[4].cardInfo.cardNumber == 5;

        Dictionary<int, int> cardCounts = new Dictionary<int, int>();
        foreach (var card in cards)
        {
            int cardNumber = card.cardInfo.cardNumber; // Treat Ace as 14
            if (!cardCounts.ContainsKey(cardNumber))
            {
                cardCounts[cardNumber] = 0;
            }
            cardCounts[cardNumber]++;
        }

        bool isFourOfAKind = cardCounts.ContainsValue(4);
        bool isThreeOfAKind = cardCounts.ContainsValue(3);
        bool isPair = cardCounts.Values.Count(count => count == 2) == 1;
        bool isTwoPair = cardCounts.Values.Count(count => count == 2) == 2;
        bool isFullHouse = isThreeOfAKind && isPair;

        List<Card> highValueCards = new List<Card>();
        int handValue = 0;

        if (isRoyalFlush)
        {
            handValue = 9;
            highValueCards = cards;
        }
        else if (isFlush && (isStraight || isAceLowStraight))
        {
            handValue = 8;
            highValueCards = cards;
        }
        else if (isFourOfAKind)
        {
            handValue = 7;
            highValueCards = cards.Where(card => cardCounts[card.cardInfo.cardNumber] == 4).ToList();
        }
        else if (isFullHouse)
        {
            handValue = 6;
            highValueCards = cards.Where(card => cardCounts[card.cardInfo.cardNumber] >= 2).ToList();
        }
        else if (isFlush)
        {
            handValue = 5;
            highValueCards = cards;
        }
        else if (isStraight || isAceLowStraight)
        {
            handValue = 4;
            highValueCards = cards;
            print(handValue + " straight");
        }
        else if (isThreeOfAKind)
        {
            handValue = 3;
            highValueCards = cards.Where(card => cardCounts[card.cardInfo.cardNumber] == 3).ToList();
        }
        else if (isTwoPair)
        {
            handValue = 2;
            highValueCards = cards.Where(card => cardCounts[card.cardInfo.cardNumber] == 2).ToList();
        }
        else if (isPair)
        {
            handValue = 1;
            highValueCards = cards.Where(card => cardCounts[card.cardInfo.cardNumber] == 2).ToList();
        }
        else
        {
            handValue = 0;
            highValueCards = cards.OrderByDescending(card => card.cardInfo.cardNumber).Take(1).ToList();
        }

        return new HandEvaluation(handValue, highValueCards);
    }


    WinType winType(int strength)
    {
        switch (strength)
        {
            case 0:
                return WinType.HighCard;
            case 1:
                return WinType.Pair;
            case 2:
                return WinType.TwoPair;
            case 3:
                return WinType.ThreeOfAKind;
            case 4:
                return WinType.Straight;
            case 5:
                return WinType.Flush;
            case 6:
                return WinType.FullHouse;
            case 7:
                return WinType.FourOfAKind;
            case 8:
                return WinType.StraightFlush;
            case 9:
                return WinType.RoyalFlush;
            default:
                return WinType.HighCard;
        }
    }
}

public enum WinType
{
    RoyalFlush,
    StraightFlush,
    FourOfAKind,
    FullHouse,
    Flush,
    Straight,
    ThreeOfAKind,
    TwoPair,
    Pair,
    HighCard
}
