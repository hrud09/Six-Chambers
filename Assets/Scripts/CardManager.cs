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

            chamber.chamberCards.Add(cardInfo1);
            chamber.chamberCards.Add(cardInfo2);

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
            boardManager.cards.Add(cardInfo);
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
        List<CardInfo> cardsOnBoard = boardManager.cards;
        Chamber winningChamber = null;
        List<Chamber> winningChambers;
        int highestHandValue = -1;
        List<Chamber> tiedChambers = new List<Chamber>();

        foreach (Transform t in chamberManager.chamberTransforms)
        {
            List<CardInfo> chamberCards = new List<CardInfo>(cardsOnBoard);
            chamberCards.AddRange(t.GetComponent<Chamber>().chamberCards);

            int handValue = EvaluateHand(chamberCards);
            if (handValue > highestHandValue)
            {
                highestHandValue = handValue;
                winningChamber = t.GetComponent<Chamber>();
                tiedChambers.Clear();
                tiedChambers.Add(winningChamber);
            }
            else if (handValue == highestHandValue)
            {
                tiedChambers.Add(t.GetComponent<Chamber>());
            }
        }

        if (tiedChambers.Count == 1)
        {
            winText.text = winningChamber.index.ToString() + " Wins by " + winType(highestHandValue).ToString();
        }
        else
        {
            string tiedChambersIndices = string.Join(", ", tiedChambers.Select(chamber => chamber.index.ToString()).ToArray());
            winningChambers = BreakTie(tiedChambers, highestHandValue);
            if (winningChambers.Count == 1) winText.text = winningChambers[0].index.ToString() + " Wins by Tie-Breaker against Chambers: " + tiedChambersIndices + "\nWins by " + winType(highestHandValue).ToString();
            else
            {
                string message = "";
                foreach (var item in winningChambers)
                {
                    message += item.index.ToString() + ",";
                }
                message += " Wins by" + winType(highestHandValue).ToString();
            }

        }

    }
    List<Chamber> BreakTie(List<Chamber> tiedChambers, int handValue)
    {
        List<Chamber> tiedWinners = new List<Chamber>();
        tiedWinners.Add(tiedChambers[0]);

        List<CardInfo> winningCards = new List<CardInfo>(boardManager.cards);
        winningCards.AddRange(tiedWinners[0].chamberCards);
        winningCards = winningCards.OrderByDescending(card => card.cardNumber).ToList();

        foreach (var chamber in tiedChambers.Skip(1))
        {
            List<CardInfo> currentCards = new List<CardInfo>(boardManager.cards);
            currentCards.AddRange(chamber.chamberCards);
            currentCards = currentCards.OrderByDescending(card => card.cardNumber).ToList();

            bool isTie = true;
            for (int i = 0; i < winningCards.Count; i++)
            {
                if (winningCards[i].cardNumber > currentCards[i].cardNumber)
                {
                    isTie = false;
                    break;
                }
                else if (winningCards[i].cardNumber < currentCards[i].cardNumber)
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


    int EvaluateHand(List<CardInfo> cards)
    {
        // Sort cards by number
        cards = cards.OrderBy(card => card.cardNumber).ToList();

        // Check for flush
        bool isFlush = cards.All(card => card.CardType == cards[0].CardType);
        bool isRoyalFlush = cards[0].cardNumber == 1 && cards[1].cardNumber == 10 && cards[2].cardNumber == 11 && cards[3].cardNumber == 12 && cards[4].cardNumber == 13;

        // Check for straight
        bool isStraight = true;
        for (int i = 1; i < cards.Count; i++)
        {
            if (cards[i].cardNumber != cards[i - 1].cardNumber + 1)
            {
                isStraight = false;
                break;
            }
        }

        // Check for other hand types
        Dictionary<int, int> cardCounts = new Dictionary<int, int>();
        foreach (var card in cards)
        {
            if (!cardCounts.ContainsKey(card.cardNumber))
            {
                cardCounts[card.cardNumber] = 0;
            }
            cardCounts[card.cardNumber]++;
        }

        bool isFourOfAKind = cardCounts.ContainsValue(4);
        bool isThreeOfAKind = cardCounts.ContainsValue(3);
        bool isPair = cardCounts.Values.Count(count => count == 2) == 1;
        bool isTwoPair = cardCounts.Values.Count(count => count == 2) == 2;
        bool isFullHouse = isThreeOfAKind && isPair;

        if (isRoyalFlush)
            return 9;
        if (isFlush && isStraight)
            return 8;
        if (isFourOfAKind)
            return 7;
        if (isFullHouse)
            return 6;
        if (isFlush)
            return 5;
        if (isStraight)
            return 4;
        if (isThreeOfAKind)
            return 3;
        if (isTwoPair)
            return 2;
        if (isPair)
            return 1;

        return 0;
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
