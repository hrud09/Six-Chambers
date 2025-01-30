using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using System.Linq;

public enum CardType
{
    Clubs,
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

    public Transform cardSpawnPos;
    public BoardManager boardManager;
    public ChamberManager chamberManager;

    public PokerEvaluator pokerEvaluator;

    public Transform[] deckCards;

    void Start()
    {
        InitializeCards();
    }

    private void InitializeCards()
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

    public void DrawCardsForChambers()
    {
        StartCoroutine(DrawCardsForChambersDelay());
    }

    private IEnumerator DrawCardsForChambersDelay()
    {
        for (int i = 0; i < chamberManager.chamberTransforms.Length * 2; i++)
        {
            int index = i % 6;
            Chamber chamber = chamberManager.chamberTransforms[index].GetComponent<Chamber>();

            CardInfo cardInfo = DrawRandomCard();
            cardsInHands.Add(cardInfo);

            GameObject card = Instantiate(cardPrefab, cardSpawnPos.position, Quaternion.identity, chamber.cardParent);
            cardsOnHands.Add(card.transform);
            card.GetComponent<Card>().InitiateCard(cardInfo,false);

            chamber.chamberCards.Add(card.GetComponent<Card>());
            Vector3 pos = Vector3.zero;
          //  chamber.InitializeOriginalPositions();

            if (i > 5)
            {
                chamber.AddOneBullet();
                pos.z = -0.5f;
            }
            card.transform.localRotation = (i <= 5)
                ? Quaternion.Euler(new Vector3(0, -18f + (6 * i), 0))
                : Quaternion.identity;

            card.transform.DOLocalJump(pos, 2f, 1, 1f).SetEase(Ease.InOutQuad).OnComplete(() => { 
            
                card.GetComponent<Card>().initialPosition = card.transform.localPosition;
                card.GetComponent<Card>().initialRotation = card.transform.localRotation.eulerAngles;

            });
            yield return new WaitForSeconds(0.2f);
        }
        yield return new WaitForSeconds(0.3f);

        GameManager.GetInstance().SetGameState(GameState.DealingBoardCards1);
        TutorialManager.Instance.ShowTutorial(TutorialType.DealCardsOnTable);
    }

    public void DrawCardsOnBoard()
    {
        StartCoroutine(DrawCardsOnBoardDelay());
    }

    private IEnumerator DrawCardsOnBoardDelay()
    {
        for (int i = 0; i < boardManager.boardCardParents.Length - 2; i++)
        {
            CardInfo cardInfo = DrawRandomCard();
            Card card = Instantiate(cardPrefab, cardSpawnPos.position, Quaternion.Euler(0, 0, 0), boardManager.boardCardParents[i]).GetComponent<Card>();
            card.InitiateCard(cardInfo, i < 3);

            Vector3 pos = boardManager.boardCardParents[i].position;
            boardManager.cardsOnBoard.Add(card);

            card.transform.DOJump(pos, 2f, 1, 1f);
            yield return new WaitForSeconds(0.2f);
        }

        yield return new WaitForSeconds(1f);
        chamberManager.PlayHandChoosingAnimation();
    }

    public void DrawAnotherCardOnBoard()
    {
        CardInfo cardInfo = DrawRandomCard();
        Card card = Instantiate(cardPrefab, cardSpawnPos.position, Quaternion.Euler(0, 0, 0), boardManager.boardCardParents[boardManager.cardsOnBoard.Count]).GetComponent<Card>();
        card.InitiateCard(cardInfo, true);

        Vector3 pos = boardManager.boardCardParents[boardManager.cardsOnBoard.Count].position;
        boardManager.cardsOnBoard.Add(card);

        card.transform.DOJump(pos, 2f, 1, 1f).OnComplete(() =>
        {
            if (boardManager.cardsOnBoard.Count == 5)
            {
                pokerEvaluator.CallForRevealAction();
            }
            else
            {
                GameManager.GetInstance().SetGameState(GameState.DealingBoardCards2);
            }
        });
    }

    public void CollectAllCards()
    {
        StartCoroutine(CollectAllCardsWithDelay());
    }

    private IEnumerator CollectAllCardsWithDelay()
    {
        foreach (Chamber chamber in chamberManager.chambers)
        {
            foreach (Card card in chamber.chamberCards)
            {
                Transform cardTransform = card.transform;
                cardTransform.DOMove(cardSpawnPos.position, 0.5f).SetEase(Ease.Linear).OnComplete(() => Destroy(card.gameObject));
            }
            yield return new WaitForSeconds(0.2f);
        }

        foreach (Card card in boardManager.cardsOnBoard)
        {
            card.transform.DOMove(cardSpawnPos.position, 0.5f).SetEase(Ease.Linear).OnComplete(() => Destroy(card.gameObject));
        }

        yield return new WaitForSeconds(0.1f);
     
        ResetCards();
        GameManager.GetInstance().OnRoundEnd();
    }

    private void ResetCards()
    {
        tempDeck = new List<CardInfo>(cardsInDeck);
        cardsInHands.Clear();
        cardsOnHands.Clear();

        foreach (var chamber in chamberManager.chambers)
        {
            chamber.ResetChamber();
        }

        boardManager.cardsOnBoard.Clear();
    }

    public void MouseOverDeck()
    {
        foreach (var item in deckCards)
        {
            item.localRotation = Quaternion.identity;
            item.DOScale(Vector3.one * 1.2f, 0.2f);
            item.DOLocalRotate(Vector3.up * Random.Range(-10f, 10f), 0.3f);
        }
    }

    public void ResetDeckMouseEffects()
    {
        foreach (var item in deckCards)
        {
            item.DOScale(Vector3.one, 0.1f);
            item.DOLocalRotate(Vector3.zero, 0.2f);
        }
    }
}
