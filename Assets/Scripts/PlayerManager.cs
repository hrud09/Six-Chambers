using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class PlayerManager : MonoBehaviour
{
    public ChamberManager chamberManager;
 
    public bool mouseOverChambers;
    public Chamber playerChosenChamber;

    Vector3 targetPosition;
    public bool chamberSelected;
    public bool playersTurn;


    public int currentSetPoint;
    public TMP_Text pointText;

    public GameObject chooseAgainPopUp;
    public SetAndRoundManager setAndRoundManager;
    public GameManager gameManager;
    public Image pointFill;
    private void Start()
    {
        currentSetPoint = PlayerPrefs.GetInt("PlayerPoint", setAndRoundManager.pointsQuota);
        pointText.text = currentSetPoint.ToString();
        pointFill.fillAmount = (currentSetPoint / setAndRoundManager.pointsQuota);
    }
    public void SelectPlayerChamber(Chamber _choosenChamber)
    {
        chamberSelected = true;
        playerChosenChamber = _choosenChamber;
        chamberManager.PickRangersHand();
    }

    public void CheckSelectedChamber(Chamber winningChamber, int point)
    {
        print("Here Less");
        if (playerChosenChamber != chamberManager.rangerChosenChamber)
        {
            if (winningChamber == playerChosenChamber)
            {

                currentSetPoint -= point;

            }
            else if(winningChamber == chamberManager.rangerChosenChamber)
            {
                currentSetPoint += point;
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
                currentSetPoint -= 2 * point;
            }
            else if (winningChamber == playerChosenChamber)
            {
                currentSetPoint += 2 * point;
            }
        }

        PlayerPrefs.SetInt("PlayerPoint", currentSetPoint);
        pointText.text = currentSetPoint.ToString();

       setAndRoundManager.EndRound();

    }
    public void CheckSelectedChamber(List<Chamber> winningChambers, int point)
    {
        print("Here Many");
        if (playerChosenChamber != chamberManager.rangerChosenChamber)
        {
            if (winningChambers.Contains(playerChosenChamber))
            {

                currentSetPoint += point;

            }
            else if (winningChambers.Contains(chamberManager.rangerChosenChamber))
            {
                currentSetPoint -= point;
            }
            else if (winningChambers.Contains(playerChosenChamber) && winningChambers.Contains(chamberManager.rangerChosenChamber))
            {
                currentSetPoint += 3 * point;
            }
            else
            {
                //Nothing
            }
        }
        else if (playerChosenChamber == chamberManager.rangerChosenChamber)
        {
            if (winningChambers.Contains(chamberManager.rangerChosenChamber))
            {
                currentSetPoint -= point;
            }

            else
            {
                //Nothing
            }
        }
        PlayerPrefs.SetInt("PlayerPoint", currentSetPoint);
        pointFill.fillAmount = (currentSetPoint / setAndRoundManager.pointsQuota);
        pointText.text = currentSetPoint.ToString();
        setAndRoundManager.EndRound();
    }
}
