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
    public void SelectPlayerChamber(Chamber _choosenChamber)
    {
        chamberSelected = true;
        playerChosenChamber = _choosenChamber;
        chamberManager.PickRangersHand();
    }

    public void CheckSelectedChamber(Chamber winningChamber, int point)
    {
        if (playerChosenChamber != chamberManager.rangerChosenChamber)
        {
            if (winningChamber == playerChosenChamber)
            {

                setPoint += point;

            }
            else if(winningChamber == chamberManager.rangerChosenChamber)
            {
                setPoint -= point;
            }
            else
            {
                //nothing

            }
        }
        else if (playerChosenChamber == chamberManager.rangerChosenChamber)
        {
            if (winningChamber != playerChosenChamber)
            {
                setPoint += 2 * point;
            }
            else if (winningChamber == playerChosenChamber)
            {
                setPoint -= 2 * point;
            }
        }

        pointText.text = setPoint.ToString();


    }
    public void CheckSelectedChamber(List<Chamber> winningChambers, int point)
    {
        print("Here");
        if (playerChosenChamber != chamberManager.rangerChosenChamber)
        {
            if (playerChosenChamber)
            {

                setPoint += point;

            }
            else if (chamberManager.rangerChosenChamber)
            {
                setPoint -= point;
            }
            else
            {
                //nothing

            }
        }
        else if (playerChosenChamber == chamberManager.rangerChosenChamber)
        {
          /*  if ( != playerChosenChamber)
            {
                setPoint += 2 * point;
            }
            else if (winningChamber == playerChosenChamber)
            {
                setPoint -= 2 * point;
            }*/
        }

        pointText.text = setPoint.ToString();

    }
}
