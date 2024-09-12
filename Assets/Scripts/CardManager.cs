using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using System.Linq;

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
    public bool doNotUse;
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

    public PokerEvaluator pokerEvaluator;
    void Start()
    {
        // Initialization and card drawing routines
        InitializeCards();
        StartCoroutine(DrawCardsForChambers());
        StartCoroutine(DrawCardsOnBoard());
    }

    void InitializeCards()
    {
        foreach (var cardDeck in signWiseCards)
        {
            if (!cardDeck.doNotUse)
            {

                foreach (var cardSprite in cardDeck.sprites)
                {
                    CardInfo cardInfo = new CardInfo
                    {
                        cardTexture = cardSprite,
                        CardType = cardDeck.CardType,
                        cardNumber = cardDeck.sprites.ToList().IndexOf(cardSprite) + 2
                    };
                    cardsInDeck.Add(cardInfo);
                }
            }
        }
        tempDeck = new List<CardInfo>(cardsInDeck);
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

            chamber.InitializeOriginalPositions();

            if (chamberTransform.localPosition.x > 0) card1.transform.localRotation = Quaternion.Euler(new Vector3(0, 20, 180));
            else card1.transform.localRotation = Quaternion.Euler(new Vector3(0, -20, 180));

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
            boardManager.cards.Add(card.GetComponent<Card>());
            int index = i;
            card.transform.DOMove(pos, 1f).OnComplete(() =>
            {
                if (index < 3) card.transform.DOLocalRotate(Vector3.zero, 1f);
            });

            yield return new WaitForSeconds(0.2f);
        }

        yield return new WaitForSeconds(1);
        chamberManager.playerHandManager.playersTurn = true;
    }
}
