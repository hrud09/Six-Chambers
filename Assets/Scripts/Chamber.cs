using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Chamber : MonoBehaviour
{
    public ChamberManager chamberManager;
    public List<Card> chamberCards;
    public int index;

    public Transform cardParent;
    public Transform[] bestCardsHolders;

    [SerializeField]
    private Vector3[] originalPositions;
    private List<Tween> cardTweens = new List<Tween>();
    public GameObject layerSelectionAura;
    public GameObject topRankUI;
   
    private void Awake()
    {
        chamberManager = GetComponentInParent<ChamberManager>();
       
    }

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

    private void OnMouseDown()
    {
        if (chamberManager.playerHandManager.playerChosenChamber == null)
        {
           // layerSelectionAura.SetActive(false);
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
        if(chamberManager.playerHandManager.chamberSelected != this) layerSelectionAura.SetActive(false);
        chamberManager.playerHandManager.mouseOverChambers = false;
        LowerCards();
    }

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
}
