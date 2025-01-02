using DG.Tweening;
using TMPro;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public ChamberManager chamberManager;
    public Chamber playerChosenChamber;

    public bool chamberSelected;
    public bool playersTurn;

    public GameObject outOfChips;
    public SetAndRoundManager setAndRoundManager;
    public GameManager gameManager;

    [Header("Chips System")]
    public TMP_Text currentChipsCountText;
    public TMP_Text playerChipsChangeCountText;

    public int chipsCount;
    public int wageredChipsCount;
    public PowerManager powerManager;
    public bool mouseOverChambers;

    private int lastChipsCount;

    private void Start()
    {
        chipsCount = PlayerPrefs.GetInt("PlayerChipsCount", 6);
        UpdateChipsCount();
    }

    public void SelectPlayerChamber(Chamber _chosenChamber)
    {
        TutorialManager.Instance.ShowTutorial(TutorialType.RangersTurn);
        chamberSelected = true;
        playerChosenChamber = _chosenChamber;

        wageredChipsCount = CalculateChipsAmountToWager(_chosenChamber);
        chipsCount -= wageredChipsCount;

        if (wageredChipsCount > 1)
        {
            _chosenChamber.UpdateChipsChangeText(-wageredChipsCount, Color.yellow);
        }

        _chosenChamber.UpdateChipsCount();
        UpdateChipsCount();
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
            chipsCount -= 1;
            winningChamber.currentChipsCount += 1 + wageredChipsCount;
            winningChamber.UpdateChipsChangeText(1 + wageredChipsCount, Color.green);
            winningChamber.UpdateChipsCount();

            playerChosenChamber.UpdateChipsChangeText(wageredChipsCount, Color.yellow);
            playerChosenChamber.UpdateChipsCount();
        }
        else
        {
            playerChosenChamber.UpdateChipsChangeText(wageredChipsCount, Color.yellow);
            DistributeChipsToWinningChamber(winningChamber, wageredChipsCount);
        }
        winningChamber.UpdateChipsCount();
        playerChosenChamber.UpdateChipsCount();
    }

    private void HandleLiveChamber(Chamber winningChamber)
    {
        if (winningChamber == playerChosenChamber)
        {
            chipsCount -= 3;
            winningChamber.currentChipsCount += 3;
            winningChamber.UpdateChipsChangeText(3, Color.green);
            winningChamber.UpdateChipsCount();
        }
        else
        {
            chipsCount += wageredChipsCount;
            playerChosenChamber.UpdateChipsChangeText(wageredChipsCount, Color.yellow);
            playerChosenChamber.UpdateChipsCount();

            foreach (var chamber in chamberManager.chambers)
            {
                int cCount = Mathf.Min(1, chamber.currentChipsCount);
                if (cCount > 0)
                {
                    chipsCount += cCount;
                    chamber.currentChipsCount -= cCount;
                    chamber.UpdateChipsChangeText(-cCount, Color.red);
                    chamber.UpdateChipsCount();
                }
            }
        }
    }

    private void DistributeChipsToWinningChamber(Chamber winningChamber, int chipsToAdd)
    {
        if (winningChamber != null)
        {
            winningChamber.currentChipsCount += chipsToAdd;
            winningChamber.UpdateChipsChangeText(chipsToAdd, Color.green);
            Debug.Log($"Chips added to winning chamber: {winningChamber.name}");
        }
    }

    private int CalculateChipsAmountToWager(Chamber _chosenChamber)
    {
        return _chosenChamber.currentChipsCount == 0 ? 1 : Mathf.Min(2, _chosenChamber.currentChipsCount);
    }

    private void UpdateChipsCount(bool chipsForBoard = false)
    {
        if (chipsCount <= 0)
        {
            outOfChips.SetActive(true);
        }
        else
        {
            outOfChips.SetActive(false);
        }

        int change = chipsCount - lastChipsCount;

        if (change != 0)
        {
            playerChipsChangeCountText.DOKill();
            playerChipsChangeCountText.text = change > 0 ? $"+{change}" : $"{change}";
            playerChipsChangeCountText.color = chipsForBoard ? Color.yellow : (change > 0 ? Color.green : Color.red);
            playerChipsChangeCountText.gameObject.SetActive(true);

            // Fade-in and display the chip change text
            playerChipsChangeCountText.DOFade(1, 0.3f).OnComplete(() =>
            {
                DOVirtual.DelayedCall(1.5f, () =>
                {
                    // Fade-out the chip change text after the delay
                    playerChipsChangeCountText.DOFade(0, 0.3f).OnComplete(() =>
                    {
                        playerChipsChangeCountText.gameObject.SetActive(false);
                    });
                });
            });
        }

        lastChipsCount = chipsCount;

        // Update the PlayerPrefs and UI
        PlayerPrefs.SetInt("PlayerChipsCount", chipsCount);
        chamberManager.InitiateChambers();
        currentChipsCountText.text = chipsCount.ToString();
    }
}
