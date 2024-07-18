using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class PlayerHandManager : MonoBehaviour
{
    public ChamberManager chamberManager;
 
    public bool mouseOverChambers;
    public Chamber playerChosenChamber;

    Vector3 targetPosition;
    public bool chamberSelected;
    public bool playersTurn;


    public int setPoint;
    public TMP_Text pointText;
    public void SelectChamber(Chamber _choosenChamber)
    {
        chamberSelected = true;
        playerChosenChamber = _choosenChamber;
        StartCoroutine(chamberManager.PickRangersHand());
    }
 
    public void CheckSelectedChamber(Chamber winningChamber, int point)
    {
        if (playerChosenChamber == winningChamber)
        {
            setPoint += point;
            pointText.text = setPoint.ToString();
        }

    }
    public void CheckSelectedChamber(List<Chamber> winningChambers, int point)
    {
        if (winningChambers.Contains(playerChosenChamber))
        {
            setPoint += point;
            pointText.text = setPoint.ToString();
        }

    }
}
