using DG.Tweening;
using TMPro;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public ChamberManager chamberManager;
    public Chamber playerChosenChamber;

    public bool chamberSelected;
    public bool playersTurn;
    public bool mouseOverChambers;

    public GameObject outOfChips;
    public SetAndRoundManager setAndRoundManager;
    public GameManager gameManager;

    private void Start()
    {
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
