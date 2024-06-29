using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chamber : MonoBehaviour
{


    public ChamberManager chamberManager;
    public Transform chipsHolderForChamber;
    public Transform chipsHolderForChoosing;
    public Transform chipsHolderForDeposit;

    public List<CardInfo> chamberCards;
    public int index;

    public Transform cardParent;
    private void Awake()
    {
        chamberManager = GetComponentInParent<ChamberManager>();
    }
    private void OnMouseDown()
    {
        if (chamberManager.playerChosenChamber == null)
        {
            chamberManager.playerChosenChamber = this;
            chamberManager.playerChipsManager.DepositChips(chipsHolderForDeposit);
            StartCoroutine(chamberManager.PickRangersHand());   
        }
    }

    private void OnMouseOver()
    {
        if (!chamberManager.playerChipsManager.deposited && chamberManager.playerChipsManager.playersTurn && !chamberManager.playerChipsManager.mouseOverChambers)
        {
            chamberManager.playerChipsManager.mouseOverChambers = true;
            chamberManager.playerChipsManager.closestChamber = this;
        }
    }
    private void OnMouseExit()
    {
        chamberManager.playerChipsManager.mouseOverChambers = false;
    }
}
