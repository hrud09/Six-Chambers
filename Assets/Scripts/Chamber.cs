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
    public List<Card> chamberCards, chamberRankCards;
    public Transform cardParent;

    private Vector3[] originalPositions;
    private List<Tween> cardTweens = new List<Tween>();
    private GameObject playerSelectionAura;


    [Header("Bullet Area")]
    public GameObject bulletPrefab;
    public Transform[] bulletParents;
    public int minimumBulletCount;
    public int maxBulletCount;
    public List<GameObject> currentBullets;
    public ParticleSystem bulletSpawnVFX, bulletDeactivateVFX;

    public GameObject WinOrLoseSelectionPopUp;
    private void Awake()
    {
        chamberManager = GetComponentInParent<ChamberManager>();
        pokerEvaluator = FindObjectOfType<PokerEvaluator>();
        chamberNumberText.text = chamberIndex.ToString();
    }
    public void InitializeOriginalPositions()
    {
        playerSelectionAura = chamberCards[0].playerSelectionAura;
        originalPositions = new Vector3[chamberCards.Count];
        for (int i = 0; i < chamberCards.Count; i++)
        {
            if (chamberCards[i] != null)
            {
                originalPositions[i] = chamberCards[i].transform.position;
            }
        }

        AddOneBullet();
    }
    public void AddOneBullet()
    {
        StartCoroutine(AddBulletWithDelay());
    }

    private IEnumerator AddBulletWithDelay()
    {
        if (currentBullets.Count < maxBulletCount)
        {
            yield return new WaitForSeconds(4f);
            GameObject newBullet = Instantiate(bulletPrefab, bulletParents[currentBullets.Count - 1]);
            bulletSpawnVFX.gameObject.transform.position = bulletParents[currentBullets.Count - 1].position;
            currentBullets.Add(newBullet);
            bulletSpawnVFX.Play();
        }
    }
    public GameObject GetABullet()
    {
        GameObject bullet = currentBullets[0];

        currentBullets.Remove(bullet);
        return bullet;
    }
    public void SetWinLosePrediction(int win = 1)
    {
        chamberManager.winOrLoseSelectionInt = win;
        chamberManager.winLosePredicted = true;
        TutorialManager.Instance.ShowTutorial(TutorialType.DealNextCard);
        WinOrLoseSelectionPopUp.SetActive(false);
    }

    // Mouse Events

    private void OnMouseDown()
    {
        if (chamberManager.playerHandManager.playerChosenChamber == null && chamberManager.playerHandManager.playersTurn)
        {
            chamberManager.playerHandManager.SelectPlayerChamber(this);
            LowerCards();
        }
        else if (pokerEvaluator.ReadyToRevealChamberCards)
        {
            RevealHand();
        }
    }
    private void OnMouseEnter()
    {
        if (!chamberManager.playerHandManager.chamberSelected && chamberManager.playerHandManager.playersTurn/* && !chamberManager.playerHandManager.mouseOverChambers*/)
        {
            playerSelectionAura.SetActive(true);
            chamberManager.playerHandManager.mouseOverChambers = true;
           
            LiftCards();
        }

    }

    private void RevealHand()
    {
        handRevealed = true;

        Transform card1 = chamberCards[0].transform;
        Transform card2 = chamberCards[1].transform;

        card1.DOLocalMoveX(1f, 0.4f);
        card1.DOLocalRotate(Vector3.up * 20f, 0.4f).OnComplete(
            () =>
            {
                card1.DOLocalMoveX(0.5f, 0.4f);
                card1.DOLocalRotate(Vector3.down * -20, 0.2f).SetEase(Ease.OutQuad)
          .OnComplete(() =>
          {

              chamberCards[0].backFaceRend.enabled = false;
              chamberCards[0].frontFaceRend.enabled = true;
              card1.DOLocalMoveZ(0.2f, 0.5f).SetEase(Ease.OutBounce);
              pokerEvaluator.CheckHand(this);
              if (chamberManager.AllHandRevealed()) pokerEvaluator.CheckPokerLogics();
          });
            }

            );

    }
    private void OnMouseOver()
    {
        if (!chamberManager.playerHandManager.chamberSelected && chamberManager.playerHandManager.playersTurn && !chamberManager.playerHandManager.mouseOverChambers)
        {
            //  playerSelectionAura.SetActive(true);
            //  chamberManager.playerHandManager.mouseOverChambers = true;
            // LiftCards();
        }
    }

    private void OnMouseExit()
    {
        if (chamberCards.Count == 0) return;
        if (chamberManager.playerHandManager.chamberSelected != this) playerSelectionAura.SetActive(false);
        chamberManager.playerHandManager.mouseOverChambers = false;

        LowerCards();
    }

    // Card Lift/Lower
    public void LiftCards()
    {
        if (chamberCards.Count < 2) return;
        KillCardTweens();
        cardTweens.Add(chamberCards[0].transform.DOLocalMoveX(0.5f - (0.2f * (chamberIndex - 1)), 0.15f));
        cardTweens.Add(chamberCards[1].transform.DOLocalMoveY(1f, 0.15f).OnComplete(() => { 
        

        cardTweens.Add(chamberCards[1].transform.DOLocalMoveY(1.5f, 0.6f).SetLoops(-1, LoopType.Yoyo));
        
        }));
        cardTweens.Add(chamberCards[1].transform.DOLocalRotate(Vector3.right * - 15 + Vector3.up *( - 18 + (7.2f * (chamberIndex - 1))), 0.25f));
        cardTweens.Add(chamberCards[1].transform.DOLocalMoveX(-0.125f + (0.05f * (chamberIndex - 1)), 0.15f));
       // cardTweens.Add(bulletMark.DOScale(Vector3.one * 1.5f, 0.6f).SetLoops(-1, LoopType.Yoyo));
    }

    public void LowerCards()
    {
        if (chamberCards.Count < 2) return;
        KillCardTweens();

        cardTweens.Add(chamberCards[0].transform.DOLocalMoveX(0, 0.2f));
        cardTweens.Add(chamberCards[1].transform.DOLocalMoveY(0, 0.2f));
        cardTweens.Add(chamberCards[1].transform.DOLocalRotate(Vector3.zero, 0.2f));
        cardTweens.Add(chamberCards[1].transform.DOLocalMoveX(0, 0.2f));
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
        playerSelectionAura = null;

        chamberManager.rangerManager.sameChamberSelected = false;
        chamberManager.winOrLoseSelectionInt = 0;
        chamberManager.winLosePredicted = true;
    }
}
