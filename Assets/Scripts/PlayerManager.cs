using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public ChamberManager chamberManager;
    public Chamber playerChosenChamber;

    public bool chamberSelected;
    public bool playersTurn;
    public bool mouseOverChambers;

    public SetAndRoundManager setAndRoundManager;
    public GameManager gameManager;


    [Header("Health Section")]
    public static int maxHealth = 25;
    public int health;
    public TMP_Text healthText, healthTextShadow;
    public Image healthFill;


    private void Start()
    {
        health = maxHealth;
        healthFill.fillAmount = 1;
    }

    public void TakeDamage(int damageAmount)
    {
        health -= damageAmount;
        healthText.text = health.ToString();
        healthTextShadow.text = health.ToString();
        healthFill.fillAmount = (health/maxHealth);
    }

    public void SelectPlayerChamber(Chamber _chosenChamber)
    {
        TutorialManager.Instance.ShowTutorial(TutorialType.RangersTurn);
        chamberSelected = true;
        playerChosenChamber = _chosenChamber;

        chamberManager.PickRangersHand();
    }

    public void CheckSelectedChamber(Chamber winningChamber, int point)
    {
        if (playerChosenChamber == chamberManager.rangerChosenChamber)
        {

            if (chamberManager.winOrLoseSelectionInt == 1)
            {
                //Player predicted a WIN

                if (winningChamber == playerChosenChamber)
                {
                    //Player predicted Right
                    HandlePlayerWin(winningChamber.currentBullets.Count * 2);
                }
                else
                {
                    HandlePlayerLose(winningChamber.currentBullets.Count * 2);
                }
            }
            else if (chamberManager.winOrLoseSelectionInt == 0)
            {
                //Player predicted a LOSE

                if (winningChamber != playerChosenChamber)
                {
                    //Player predicted Right
                    HandlePlayerWin(winningChamber.currentBullets.Count * 2);
                }
                else
                {
                    HandlePlayerLose(winningChamber.currentBullets.Count * 2);
                }
            }

        }
        else if (playerChosenChamber == winningChamber)
        {

            HandlePlayerWin(winningChamber.currentBullets.Count);

        }
        else if (chamberManager.rangerChosenChamber == winningChamber)
        {

            HandlePlayerLose(winningChamber.currentBullets.Count + playerChosenChamber.currentBullets.Count);


        }
        else
        {
            HandlePlayerLose(playerChosenChamber.currentBullets.Count);
        }


        winningChamber.AddOneBullet();

        setAndRoundManager.EndRound();
    }
    public void CheckSelectedChamber(List<Chamber> winningChambers, int point)
    {
        if (winningChambers.Contains(playerChosenChamber))
        {
            HandlePlayerWin(playerChosenChamber.currentBullets.Count);
        }
        else
        {
            HandlePlayerLose(playerChosenChamber.currentBullets.Count);
        }

        setAndRoundManager.EndRound();
    }

    private void HandlePlayerWin(int giveDamageAmount)
    {
        chamberManager.rangerManager.TakeDamage(giveDamageAmount);
    }

    private void HandlePlayerLose(int damageCount)
    {
        TakeDamage(damageCount);
    }
    private void HandleLiveChamber(Chamber winningChamber)
    {
        if (winningChamber != playerChosenChamber)
        {
            foreach (var chamber in chamberManager.chambers)
            {
            }
        }
    }

  
}
