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
    public ChamberManager chamberManager;
    private RangerManager rangerManager;
    private PlayerManager playerManager;
    public BoardManager boardManager;
    public Transform cardSpawnPos;

    public PokerEvaluator pokerEvaluator;


    private bool cardsDistributedToChambers;

    public Transform[] deckCards;
    void Start()
    {
        // Initialization and card drawing routines
        InitializeCards();
        rangerManager = FindObjectOfType<RangerManager>();
        playerManager = FindObjectOfType<PlayerManager>();
        TutorialManager.Instance.ShowTutorial(TutorialType.DealCardsToChamber);
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
    public void DrawCardsForChambers()
    {
        TutorialManager.Instance.HideTutorial(TutorialType.DealCardsToChamber);
        StartCoroutine(DrawCardsForChambersDelay());
    }
    public IEnumerator DrawCardsForChambersDelay()
    {
        for (int i = 0; i < chamberManager.chamberTransforms.Length * 2; i++)
        {

            int index = 0;
            if (i > 5) index = i - 6;
            else index = i;
            Chamber chamber = chamberManager.chamberTransforms[index].GetComponent<Chamber>();

            CardInfo cardInfo1 = DrawRandomCard();


            cardsInHands.Add(cardInfo1);


            GameObject card1 = Instantiate(cardPrefab, cardSpawnPos.position, Quaternion.identity, chamber.cardParent);
            cardsOnHands.Add(card1.transform);
            card1.GetComponent<Card>().InitiateCard(cardInfo1, i > 5);

            chamber.chamberCards.Add(card1.GetComponent<Card>());
            Vector3 pos1 = Vector3.zero;
            chamber.InitializeOriginalPositions();
            if (i <= 5) pos1.z = +0.5f;


            if (i <= 5) card1.transform.localRotation = Quaternion.Euler(new Vector3(0, -20, 180));
            else card1.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 180));

            card1.transform.DOLocalMove(pos1, 1f).SetEase(Ease.InOutQuad);
            yield return new WaitForSeconds(0.2f);
        }
        yield return new WaitForSeconds(1f);
        TutorialManager.Instance.ShowTutorial(TutorialType.DealCardsOnTable);
    }

    public void DrawCardsOnBoard()
    {
        TutorialManager.Instance.HideTutorial(TutorialType.DealCardsOnTable);
        StartCoroutine(DrawCardsOnBoardDelay());
    }

    public IEnumerator DrawCardsOnBoardDelay()
    {
        for (int i = 0; i < boardManager.boardCardParents.Length - 2; i++)
        {
            CardInfo cardInfo = DrawRandomCard();
            Card card = Instantiate(cardPrefab, cardSpawnPos.position, Quaternion.Euler(0, 0, 180), boardManager.boardCardParents[i]).GetComponent<Card>();
            card.InitiateCard(cardInfo, i < 3);

            Vector3 pos = boardManager.boardCardParents[i].position;
            boardManager.cardsOnBoard.Add(card);
            boardManager.cards.Add(card);
            int index = i;
            card.transform.DOMove(pos, 1f).OnComplete(() =>
            {

            });

            yield return new WaitForSeconds(0.2f);
        }

        yield return new WaitForSeconds(1);
      
        // chamberManager.playerHandManager.boardChipsCountObj.SetActive(true);
        chamberManager.PlayHandChoosingAnimation();
    }
    public void DrawAnotherCardOnBoard()
    {
        CardInfo cardInfo = DrawRandomCard();
        Card card = Instantiate(cardPrefab, cardSpawnPos.position, Quaternion.Euler(0, 0, 180), boardManager.boardCardParents[boardManager.cardsOnBoard.Count]).GetComponent<Card>();
        card.InitiateCard(cardInfo, true);

        Vector3 pos = boardManager.boardCardParents[boardManager.cardsOnBoard.Count].position;
        boardManager.cardsOnBoard.Add(card);
        boardManager.cards.Add(card);
        card.transform.DOMove(pos, 1f).OnComplete(() =>
        {

        });

    }
    private void OnMouseEnter()
    {
        if (boardManager.cardsOnBoard.Count < 5 && playerManager.chamberSelected && rangerManager.chamberSelected)
        {
            MouseOverDeck();
        }
        else if (!cardsDistributedToChambers)
        {
            MouseOverDeck();
        }
        else if (boardManager.cardsOnBoard.Count < 1)
        {
            MouseOverDeck();
        }
    }
    private void OnMouseExit()
    {
        foreach (var item in deckCards)
        {
            item.DOScale(Vector3.one, 0.1f);
            item.DOLocalRotate(Vector3.zero, 0.2f);
        }
    }
    private void MouseOverDeck()
    {
        foreach (var item in deckCards)
        {
            item.localRotation = Quaternion.identity;
            item.DOScale(Vector3.one * 1.2f, 0.2f);
            item.DOLocalRotate(Vector3.up * Random.Range(-10f, 10f), 0.3f);
        }
    }
    private void OnMouseDown()
    {
        MouseDownAction();
    }
    void MouseDownAction()
    {

            if (boardManager.cardsOnBoard.Count < 5 && playerManager.chamberSelected && rangerManager.chamberSelected)
            {
                DrawAnotherCardOnBoard();
                if (boardManager.cardsOnBoard.Count == 5)
                {

                    pokerEvaluator.CallForRevealAction();

                }
            }
            else if (!cardsDistributedToChambers)
            {
                cardsDistributedToChambers = true;
                DrawCardsForChambers();


            }
            else if (boardManager.cardsOnBoard.Count < 1)
            {
                DrawCardsOnBoard();
            }
        }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) MouseDownAction();
    }
}
