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
    private ChamberManager chamberManager;
    public ChamberUIScript chamberUI;

    // Cards
    [Header("Cards")]
    public List<Card> chamberCards, chamberRankCards;
    public Transform cardParent;
    public Transform[] bestCardsHolders;
    private Vector3[] originalPositions;
    private List<Tween> cardTweens = new List<Tween>();
    public GameObject layerSelectionAura;
    public GameObject topRankUI;

    // Chips System
    [Header("Chips System")]
    public int initialChipsCount, currentChipsCount;

    private void Awake()
    {
       
        chamberManager = GetComponentInParent<ChamberManager>();
    }

    void Start()
    {
      //  yield return new WaitForEndOfFrame();
        currentChipsCount = PlayerPrefs.GetInt($"{chamberIndex}_ChamberChipsCount", initialChipsCount);


        UpdateChipsCount();
    }

    // Card Positioning
    
    public void InitializeOriginalPositions()
    {
        layerSelectionAura = chamberCards[0].playerSelectionAura;
        originalPositions = new Vector3[chamberCards.Count];
        for (int i = 0; i < chamberCards.Count; i++)
        {
            if (chamberCards[i] != null)
            {
                originalPositions[i] = chamberCards[i].transform.position;
            }
        }
    }

    // Mouse Events
    
    private void OnMouseDown()
    {
        if (chamberManager.playerHandManager.playerChosenChamber == null && chamberManager.playerHandManager.playersTurn)
        {
            chamberManager.playerHandManager.SelectPlayerChamber(this);
        }
    }

    private void OnMouseOver()
    {
        if (!chamberManager.playerHandManager.chamberSelected && chamberManager.playerHandManager.playersTurn && !chamberManager.playerHandManager.mouseOverChambers)
        {
            layerSelectionAura.SetActive(true);
            chamberManager.playerHandManager.mouseOverChambers = true;
            LiftCards();
        }
    }

    private void OnMouseExit()
    {
        if (chamberManager.playerHandManager.chamberSelected != this) layerSelectionAura.SetActive(false);
        chamberManager.playerHandManager.mouseOverChambers = false;
        LowerCards();
    }

    // Card Lift/Lower
    private void LiftCards()
    {
        KillCardTweens();
        for (int i = 0; i < chamberCards.Count; i++)
        {
            if (i < originalPositions.Length && chamberCards[i] != null)
            {
                cardTweens.Add(chamberCards[i].transform.DOMoveY(originalPositions[i].y + 0.2f, 0.2f));
            }
        }
    }

    private void LowerCards()
    {
        KillCardTweens();
        for (int i = 0; i < chamberCards.Count; i++)
        {
            if (i < originalPositions.Length && chamberCards[i] != null)
            {
                cardTweens.Add(chamberCards[i].transform.DOMoveY(originalPositions[i].y, 0.1f));
            }
        }
    }

    private void KillCardTweens()
    {
        foreach (var tween in cardTweens)
        {
            tween.Kill();
        }
        cardTweens.Clear();
    }

    // Chips Count Update
    public void UpdateChipsCount()
    {
       
        PlayerPrefs.SetInt($"{chamberIndex}_ChamberChipsCount", currentChipsCount);
        chamberUI.chipsCountText.text = currentChipsCount.ToString();
    }

    public void UpdateChipsChangeText(int changeAmount, Color textColor)
    {
        chamberUI.changeAmountParent.SetActive(true);
        chamberUI.chipChangeAmountText.DOFade(1, 1f);
        if (textColor != Color.yellow)
        {
            if (changeAmount > 0) chamberUI.chipChangeAmountText.text = "+" + changeAmount.ToString();
            else chamberUI.chipChangeAmountText.text = changeAmount.ToString();
        }
        else
        {
            if (changeAmount < 0) chamberUI.chipChangeAmountText.text = (MathF.Abs(changeAmount)).ToString();
            else chamberUI.chipChangeAmountText.text = changeAmount.ToString();
        }
        chamberUI.chipChangeAmountText.color = textColor;
    }
}
