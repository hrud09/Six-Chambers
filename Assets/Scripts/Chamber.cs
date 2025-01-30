using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using System;

public class Chamber : MonoBehaviour
{
    // Core Dependencies
    public int chamberIndex;
    public TMP_Text chamberNumberText;
    private ChamberManager chamberManager;
    public ChamberUIScript chamberUI;
    private PokerEvaluator pokerEvaluator;

    public bool handRevealed;
    // Cards
    [Header("Cards")]
    private bool cardLifted;
    public List<Card> chamberCards, chamberRankCards;
    public Transform cardParent;

    private List<Tween> cardTweens = new List<Tween>();



    [Header("Bullet Area")]
    public GameObject bulletPrefab;
    public Transform[] bulletParents;
    public int minimumBulletCount;
    public int maxBulletCount = 6;
    public List<GameObject> currentBullets;
    public ParticleSystem bulletSpawnVFX, bulletDeactivateVFX;

    public GameObject WinOrLoseSelectionPopUp;
    private void Awake()
    {
        chamberManager = GetComponentInParent<ChamberManager>();
        pokerEvaluator = FindObjectOfType<PokerEvaluator>();
        chamberNumberText.text = chamberIndex.ToString();
    }

    public List<GameObject> GetChambersAllBullets()
    {

        List<GameObject> _allBullets = new List<GameObject>();
        _allBullets.AddRange(currentBullets);
        foreach (GameObject bullet in currentBullets)
        {
            //bulletDeactivateVFX.transform.position = bullet.transform.position;
            //bulletDeactivateVFX.Play();
            bullet.SetActive(false);
        }
        currentBullets.Clear();
        return _allBullets;
    }

    public void AddOneBullet()
    {
        StartCoroutine(AddBulletWithDelay());
    }
   
    private IEnumerator AddBulletWithDelay()
    {

        if (currentBullets.Count < maxBulletCount)
        {
            yield return new WaitForSeconds(0.5f);
            bulletPrefab.transform.localScale = Vector3.zero;
            GameObject newBullet = Instantiate(bulletPrefab, bulletParents[currentBullets.Count]);
            newBullet.transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutElastic);
            bulletSpawnVFX.gameObject.transform.position = newBullet.transform.position - Vector3.up;
            currentBullets.Add(newBullet);
            bulletSpawnVFX.Play();
        }
    }
    public void SetWinLosePrediction(int win = 1)
    {
        chamberManager.winOrLoseSelectionInt = win;
        TutorialManager.Instance.ShowTutorial(TutorialType.DealNextCard);
        WinOrLoseSelectionPopUp.SetActive(false);
        GameManager.GetInstance().SetGameState(GameState.DealingBoardCards2);
    }

    // Mouse Events


    public void RevealHand()
    {
        handRevealed = true;
        chamberCards[0].transform.DOLocalMoveZ(0.8f, 0.2f);
        chamberCards[0].RevealCard();
        pokerEvaluator.CheckHand(this);
        if (chamberManager.AllHandRevealed())
        {
            GameManager.GetInstance().SetGameState(GameState.EvaluatingHands);
            pokerEvaluator.CheckPokerLogics();
        }

    }

    private void OnMouseEnter()
    {
        if ((GameManager.GetInstance().GetCurrentGameState() == GameState.PlayersTurn) && !cardLifted && !handRevealed)
        {
           // playerSelectionAura.SetActive(true);
            chamberManager.playerHandManager.mouseOverChambers = true;

            LiftCards();
        }

    }

    private void OnMouseOver()
    {
        if ((GameManager.GetInstance().GetCurrentGameState() == GameState.PlayersTurn) && !cardLifted && !handRevealed)
        {
             // playerSelectionAura.SetActive(true);
              chamberManager.playerHandManager.mouseOverChambers = true;
              LiftCards();
        }
    }
    private void OnMouseDown()
    {
        if (GameManager.GetInstance().GetCurrentGameState() == GameState.PlayersTurn)
        {
            chamberManager.playerHandManager.SelectPlayerChamber(this);
            chamberCards[0].playerSelectionAura.SetActive(true);
            LowerCards();
        }
        else if (GameManager.GetInstance().GetCurrentGameState() == GameState.RevealingChamberCards)
        {
            RevealHand();
        }
    }

    private void OnMouseExit()
    {
        if (GameManager.GetInstance().GetCurrentGameState() == GameState.PlayersTurn)
        {
            if (chamberCards.Count < 2) return;

            chamberManager.playerHandManager.mouseOverChambers = false;

            LowerCards();
        }
    }

    // Card Lift/Lower
    public void LiftCards()
    {
        if (chamberCards.Count < 2) return;
        cardLifted = true;
        KillCardTweens();
        cardTweens.Add(chamberCards[0].transform.DOLocalMoveX(0.5f - (0.2f * (chamberIndex - 1)), 0.15f));
        cardTweens.Add(chamberCards[1].transform.DOLocalMoveY(0.5f, 0.15f).OnComplete(() => { 
        

        cardTweens.Add(chamberCards[1].transform.DOLocalMoveY(0.8f, 0.5f).SetLoops(-1, LoopType.Yoyo));
        
        }));
        cardTweens.Add(chamberCards[1].transform.DOLocalRotate(Vector3.right * - 15 + Vector3.up *( - 18 + (7.2f * (chamberIndex - 1))), 0.25f));
        cardTweens.Add(chamberCards[1].transform.DOLocalMoveX(-0.125f + (0.05f * (chamberIndex - 1)), 0.15f));
       
       // cardTweens.Add(bulletMark.DOScale(Vector3.one * 1.5f, 0.6f).SetLoops(-1, LoopType.Yoyo));
    }

    public void LowerCards()
    {
        if (chamberCards.Count < 2) return;
        KillCardTweens();
        cardLifted = false;
        cardTweens.Add(chamberCards[0].transform.DOLocalMove(chamberCards[0].initialPosition, 0.2f));
        cardTweens.Add(chamberCards[1].transform.DOLocalMove(chamberCards[1].initialPosition, 0.2f));

        cardTweens.Add(chamberCards[0].transform.DOLocalRotate(chamberCards[0].initialRotation, 0.2f));
        cardTweens.Add(chamberCards[1].transform.DOLocalRotate(chamberCards[1].initialRotation, 0.2f));
       // cardTweens.Add(bulletMark.DOScale(Vector3.one, 0.8f));
    }

    private void KillCardTweens()
    {
        foreach (var tween in cardTweens)
        {
            tween.Kill();
        }
        cardTweens.Clear();
    }

    public void PaintTopCards(List<Card> cards)
    {

        StartCoroutine(PaintTopCardsDelay(cards));
    }

    private IEnumerator PaintTopCardsDelay(List<Card> cards)
    {
        for (int i = 0; i < chamberUI.topFiveCards.Length; i++)
        {
            chamberUI.topFiveCards[i].enabled = true;
            chamberUI.topFiveCards[i].sprite = cards[i].cardInfo.cardTexture;
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void ResetChamber()
    {
        handRevealed = false;
        chamberUI.rankTextBG.color = pokerEvaluator.winLoseColors[1];
        chamberUI.rankText.enabled = false;
        chamberUI.rankUICanvasGroup.alpha = 0;

        for (int i = 0; i < chamberUI.topFiveCards.Length; i++)
        {
            chamberUI.topFiveCards[i].enabled = false;
        }

        chamberCards.Clear();
        foreach (var item in cardTweens)
        {
            item.Kill();
        }
        cardTweens.Clear();
       

        chamberManager.winOrLoseSelectionInt = 0;
    }
}
