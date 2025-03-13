using System.Collections;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance { get; private set; }

    [SerializeField] private TMP_Text roundCountText;
    [SerializeField] private TMP_Text newRoundText;
    [SerializeField] private RoundChangePopUp roundChangePopUp;
    [SerializeField] private PowerManager powerManager;

    private const string ROUND_KEY = "roundCount";
    public int CurrentRound { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        LoadProgress();
        UpdateUI();
    }

    public void EndRound()
    {
        GameplayManager.GetInstance().SetGameState(GameState.RoundEnded);

        // Increment round count before any UI updates
        CurrentRound++;
        SaveProgress();
        UpdateUI();
        RoundChangeSequence();
     /*   if ((CurrentRound - 1) % 3 == 0) // If it's a break round
        {
            TakeABreak();
        }
        else
        {
            RoundChangeSequence();
        }*/
    }

    public void RoundChangeSequence()
    {
        roundChangePopUp.EnableView();
        newRoundText.text = $"Round {CurrentRound}"; // Updated to show the correct round number

        newRoundText.DOFade(1, 0.5f).OnComplete(() =>
        {
            newRoundText.DOFade(0, 0.5f).SetDelay(2f).OnComplete(() =>
            {
                foreach (Power item in GameplayManager.GetInstance().powerManager.purchasedPowers)
                {

                    item.InitiatePower();

                }
                roundChangePopUp.DisableView();
                GameplayManager.GetInstance().SetGameState(GameState.DealingChamberCards);
                TutorialManager.Instance.ShowTutorial(TutorialType.NextRoundStarter);
            });
        });
    }

    public void StartNextRound()
    {
        // Logic to start a new round after a break if needed
    }

    private void UpdateUI()
    {
        roundCountText.text = CurrentRound.ToString();
    }

    private void SaveProgress()
    {
        PlayerPrefs.SetInt(ROUND_KEY, CurrentRound);
        PlayerPrefs.Save();
    }

    private void LoadProgress()
    {
        CurrentRound = PlayerPrefs.GetInt(ROUND_KEY, 1);
    }

    public void ResetProgress()
    {
       
        CurrentRound = 0;
        PlayerPrefs.DeleteKey(ROUND_KEY);
        PlayerPrefs.Save();
        UpdateUI();
    }
}
