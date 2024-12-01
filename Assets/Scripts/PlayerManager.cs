using DG.Tweening;
using TMPro;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public ChamberManager chamberManager;
    public Chamber playerChosenChamber;

    public bool chamberSelected;
    public bool playersTurn;

    public GameObject chooseAgainPopUp;
    public SetAndRoundManager setAndRoundManager;
    public GameManager gameManager;

    [Header("Chips System")]
    public TMP_Text potChipsText;
    public TMP_Text currentChipsCountText;

    public int chipsCount;
    public int wageredChipsCount; // Variable to store wagered chips count
    public PowerManager powerManager;
    public bool mouseOverChambers;


    public TMP_Text playerChipsChangeCountText;
    private void Start()
    {
        chipsCount = PlayerPrefs.GetInt("PlayerChipsCount", 6);
        UpdateChipsCount();
    }

    public void SelectPlayerChamber(Chamber _chosenChamber)
    {
        powerManager.ActivatePowerCards(PowerType.MidRound);
        chamberSelected = true;
        playerChosenChamber = _chosenChamber;

        wageredChipsCount = CalculateChipsAmountToWager(_chosenChamber); // Update wagered chips
        chipsCount -= wageredChipsCount;

        if (wageredChipsCount > 1)
        {
          //  _chosenChamber.currentChipsCount -= wageredChipsCount;
            _chosenChamber.UpdateChipsChangeText(-wageredChipsCount, Color.yellow);
        }

        _chosenChamber.UpdateChipsCount();
        UpdateChipsCount();
        chamberManager.PickRangersHand();
    }

    private int CalculateChipsAmountToWager(Chamber _chosenChamber)
    {
        if (_chosenChamber.currentChipsCount == 0) return 1;
        return Mathf.Min(2, _chosenChamber.currentChipsCount);
    }

    public void CheckSelectedChamber(Chamber winningChamber, int point)
    {
        if (playerChosenChamber == chamberManager.rangerChosenChamber) // Live Chamber case
        {
            HandleLiveChamber(winningChamber);
        }
        else
        {
            HandlePlayerChosen(winningChamber);
        }

        UpdateChipsCount();
        setAndRoundManager.EndRound();
    }

    private void HandlePlayerChosen(Chamber winningChamber)
    {
        if (playerChosenChamber == winningChamber)
        {
            chipsCount += wageredChipsCount * 2;
            winningChamber.currentChipsCount -= wageredChipsCount;
            winningChamber.UpdateChipsChangeText(-wageredChipsCount, Color.red);
        }
        else if (chamberManager.rangerChosenChamber == winningChamber)
        {
            chipsCount -= (1);

            winningChamber.currentChipsCount += (1 + wageredChipsCount);
            winningChamber.UpdateChipsChangeText(+(1 + wageredChipsCount), Color.green);
            winningChamber.UpdateChipsCount();

           // playerChosenChamber.currentChipsCount += wageredChipsCount;
            playerChosenChamber.UpdateChipsChangeText(+wageredChipsCount, Color.yellow);
            playerChosenChamber.UpdateChipsCount();
        }
        else
        {
            playerChosenChamber.UpdateChipsChangeText(+wageredChipsCount, Color.yellow);
            //playerChosenChamber.currentChipsCount += wageredChipsCount;
            DistributeChipsToWinningChamber(winningChamber, wageredChipsCount);
        }
        winningChamber.UpdateChipsCount();
        playerChosenChamber.UpdateChipsCount();
    }

    private void HandleLiveChamber(Chamber winningChamber)
    {
        if (winningChamber == playerChosenChamber)
        {
            chipsCount -= (3);
            winningChamber.currentChipsCount += (3);
            winningChamber.UpdateChipsChangeText(+(3), Color.green);
            winningChamber.UpdateChipsCount();
        }
        else
        {
            chipsCount += wageredChipsCount;
            //playerChosenChamber.currentChipsCount += wageredChipsCount;
            playerChosenChamber.UpdateChipsChangeText(wageredChipsCount, Color.yellow);
            playerChosenChamber.UpdateChipsCount();
            foreach (var chamber in chamberManager.chambers)
            {/*
                if (chamber != playerChosenChamber && chamber != chamberManager.rangerChosenChamber)
                {*/
                    int cCount = Mathf.Min(1, chamber.currentChipsCount);
                    if (cCount > 0)
                    {
                        chipsCount += cCount;
                        chamber.currentChipsCount -= cCount;
                        chamber.UpdateChipsChangeText(-cCount, Color.red);
                        chamber.UpdateChipsCount();
                    }
                    
               // }
            }
        }

       
    }

    private void DistributeChipsToWinningChamber(Chamber winningChamber, int chipsToAdd)
    {
        if (winningChamber != null)
        {
            winningChamber.currentChipsCount += chipsToAdd;
            winningChamber.UpdateChipsChangeText(+chipsToAdd, Color.green);
            Debug.Log($"Chips added to winning chamber: {winningChamber.name}");
        }
    }

   private int lastChipsCount; // Track previous chips count for comparison

    private void UpdateChipsCount(bool chipsForBoard = false)
    {
        int change = chipsCount - lastChipsCount; // Calculate the change in chips count
        lastChipsCount = chipsCount; // Update the last chips count

        // Update PlayerPrefs and current chips count text
        PlayerPrefs.SetInt("PlayerChipsCount", chipsCount);
        chamberManager.InitiateChambers();
        currentChipsCountText.text = chipsCount.ToString();

        // Skip if no change
        if (change == 0) return;

        // Kill previous tweens for playerChipsChangeCountText
        playerChipsChangeCountText.DOKill();

        // Update and display the change text
        playerChipsChangeCountText.text = change > 0 ? $"+{change}" : $"{change}";
        playerChipsChangeCountText.color = change > 0 ? Color.green : Color.red;
        if (chipsForBoard) playerChipsChangeCountText.color = Color.yellow;

        playerChipsChangeCountText.gameObject.SetActive(true); // Activate the text
        playerChipsChangeCountText.DOFade(0, 0).OnComplete(() =>
        {
            playerChipsChangeCountText.DOFade(1, 0.3f) // Fade in
                .OnComplete(() =>
                {
                    DOVirtual.DelayedCall(3, () =>
                    {
                        playerChipsChangeCountText.DOFade(0, 0.3f) // Fade out
                            .OnComplete(() =>
                            {
                                playerChipsChangeCountText.gameObject.SetActive(false);
                            });
                    });
                });
        });
    }



}
