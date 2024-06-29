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
                CardInfo cardInfo = new CardInfo();
                cardInfo.cardTexture = cardSprite;
                cardInfo.CardType = cardDeck.CardType;
                cardInfo.cardNumber = cardDeck.sprites.ToList().IndexOf(cardSprite) + 1;
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
            CardInfo cardInfo1 = DrawRandomCard();
            CardInfo cardInfo2 = DrawRandomCard();

            chamberTransform.GetComponent<Chamber>().chamberCards.Add(cardInfo1);
            chamberTransform.GetComponent<Chamber>().chamberCards.Add(cardInfo2);

            cardsInHands.Add(cardInfo1);
            cardsInHands.Add(cardInfo2);

            GameObject card1 = Instantiate(cardPrefab, cardSpawnPos.position, Quaternion.identity, chamberTransform);
            GameObject card2 = Instantiate(cardPrefab, cardSpawnPos.position, Quaternion.identity, chamberTransform);
            cardsOnHands.Add(card1.transform);
            cardsOnHands.Add(card2.transform);
            card1.GetComponent<Card>().InitiateCard(cardInfo1);
            card2.GetComponent<Card>().InitiateCard(cardInfo2);

            Vector3 pos1 = chamberTransform.position;
            Vector3 pos2 = chamberTransform.position + Vector3.back * 1f + Vector3.up * 1f;

            card1.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 180));
            card1.transform.DOMove(pos1, 1f);
            card2.transform.DOMove(pos2, 1f);
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
        int highestHandValue = -1;

        foreach (Transform t in chamberManager.chamberTransforms)
        {
            List<CardInfo> chamberCards = new List<CardInfo>(cardsOnBoard);
            chamberCards.AddRange(t.GetComponent<Chamber>().chamberCards);

            int handValue = EvaluateHand(chamberCards);
            if (handValue > highestHandValue)
            {
                highestHandValue = handValue;
                winningChamber = t.GetComponent<Chamber>();
            }
        }

        winText.text = winningChamber.index.ToString() + " Wins by " + winType(highestHandValue).ToString();
    }

    int EvaluateHand(List<CardInfo> cards)
    {
        // Sort cards by number
        cards = cards.OrderBy(card => card.cardNumber).ToList();

        // Check for flush
        bool isFlush = cards.All(card => card.CardType == cards[0].CardType);
        bool isRoyalFlash = true;

        for (int i = 1; i < cards.Count; i++)
        {
            if (cards[i].cardNumber < 11 && cards[i].cardNumber != 1)
            {
                isRoyalFlash = false;
                break;
            }

        }

        // Check for straight
        bool isStraight = true;
        for (int i = 1; i < cards.Count; i++)
        {
            if (cards[i].CardType != cards[i - 1].CardType)
            {
                isStraight = false;
                break;
            }
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

        if (isRoyalFlash)
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
                return WinType.HightCard;
            case 1:
                return WinType.Pair;
            case 2:
                return WinType.TwoPair;
            case 3:
                return WinType.ThreeOfAKind;
            case 4:
                return WinType.Straight;
            case 5:
                return WinType.FourOfAKind;
            case 6:
                return WinType.FullHouse;
            case 7:
                return WinType.Straight_Flash;
            case 8:
                return WinType.Royal_Flash;
            default:
                return WinType.HightCard;

        }
    }
}
public enum WinType
{
    Royal_Flash,
    Straight_Flash,
    FullHouse,
    FourOfAKind,
    Straight,
    ThreeOfAKind,
    TwoPair,
    Pair,
    HightCard
}