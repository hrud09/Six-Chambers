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
    public Transform cardSpawnPos;
  
    public List<SignWiseCard> signWiseCards;
    public List<CardInfo> mainDeckOfCards;
    public List<CardInfo> tempDeckOfCards;
    
    public List<Card> cardsInHands;
    public List<Card> cardsOnBoard;

    public ChamberManager chamberManager;

    public PokerEvaluator pokerEvaluator;
    public Transform[] boardCardParents;


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
                    mainDeckOfCards.Add(cardInfo);
                }
            }
        }
        tempDeckOfCards = new List<CardInfo>(mainDeckOfCards);
    }

    private CardInfo DrawRandomCard()
    {
        int index = Random.Range(0, tempDeckOfCards.Count);
        CardInfo cardInfo = tempDeckOfCards[index];
        tempDeckOfCards.RemoveAt(index);
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

            GameObject card = Instantiate(cardPrefab, cardSpawnPos.position, Quaternion.identity, chamber.cardParent);
            cardsInHands.Add(card.GetComponent<Card>());
    
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

        GameplayManager.GetInstance().SetGameState(GameState.DealingBoardCards1);
        TutorialManager.Instance.ShowTutorial(TutorialType.DealCardsOnTable);
    }

    public void DrawCardsOnBoard()
    {
        StartCoroutine(DrawCardsOnBoardDelay());
    }

    private IEnumerator DrawCardsOnBoardDelay()
    {
        for (int i = 0; i < boardCardParents.Length - 2; i++)
        {
            CardInfo cardInfo = DrawRandomCard();
            Card card = Instantiate(cardPrefab, cardSpawnPos.position, Quaternion.Euler(0, 0, 0), boardCardParents[i]).GetComponent<Card>();
            card.InitiateCard(cardInfo, i < 3);

            Vector3 pos = boardCardParents[i].position;
            cardsOnBoard.Add(card);

            card.transform.DOJump(pos, 2f, 1, 1f);
            yield return new WaitForSeconds(0.2f);
        }

        yield return new WaitForSeconds(1f);
        chamberManager.PlayHandChoosingAnimation();
    }

    public void DrawAnotherCardOnBoard()
    {
        CardInfo cardInfo = DrawRandomCard();
        Card card = Instantiate(cardPrefab, cardSpawnPos.position, Quaternion.Euler(0, 0, 0), boardCardParents[cardsOnBoard.Count]).GetComponent<Card>();
        card.InitiateCard(cardInfo, true);

        Vector3 pos = boardCardParents[cardsOnBoard.Count].position;
        cardsOnBoard.Add(card);

        card.transform.DOJump(pos, 2f, 1, 1f).OnComplete(() =>
        {
            if (cardsOnBoard.Count == 5)
            {
                pokerEvaluator.CallForRevealAction();
            }
            else
            {
                GameplayManager.GetInstance().SetGameState(GameState.DealingBoardCards2);
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

        foreach (Card card in cardsOnBoard)
        {
            card.transform.DOMove(cardSpawnPos.position, 0.5f).SetEase(Ease.Linear).OnComplete(() => Destroy(card.gameObject));
        }

        yield return new WaitForSeconds(0.1f);
     
        ResetCards();
        GameplayManager.GetInstance().OnRoundEnd();
    }

    private void ResetCards()
    {
        tempDeckOfCards = new List<CardInfo>(mainDeckOfCards);
        cardsInHands.Clear();

        foreach (var chamber in chamberManager.chambers)
        {
            chamber.ResetChamber();
        }

        cardsOnBoard.Clear();
    }


}
