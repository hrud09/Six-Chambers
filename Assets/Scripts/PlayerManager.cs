using DG.Tweening;
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
            HandleLiveChamber(winningChamber);
        }
        else
        {
            HandlePlayerChosen(winningChamber);
        }

        setAndRoundManager.EndRound();
    }

    private void HandlePlayerChosen(Chamber winningChamber)
    {
        if (playerChosenChamber != winningChamber)
        {
            DistributeChipsToWinningChamber(winningChamber, 0);
        }
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

    private void DistributeChipsToWinningChamber(Chamber winningChamber, int chipsToAdd)
    {
        if (winningChamber != null)
        {
            Debug.Log($"Chips added to winning chamber: {winningChamber.name}");
        }
    }

    private void UpdateChipsCount(bool chipsForBoard = false)
    {
        chamberManager.InitiateChambers();
    }
}
