using DG.Tweening;
using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetAndRoundManager : MonoBehaviour
{
    public int currentSetNumber;
    public TMP_Text setCountText;

    public int roundCount;
    public TMP_Text roundCountText;

    public TMP_Text newRoundText;

    public PlayerManager playerHandManager;

    public GameObject roundChangeUIObject;
    private void Awake()
    {
        
        LoadProgress();
        UpdateSet_RoundCount();
    }
    void Start()
    {
    }

    void UpdateSet_RoundCount()
    {
        setCountText.text = currentSetNumber.ToString();
        roundCountText.text = roundCount.ToString();
    }

    public void EndRound()
    {
        GameManager.GetInstance().SetGameState(GameState.RoundEnded);
        SaveProgress();
        PlayRoundChangeAnimation();
              
      
    }
    private void PlayRoundChangeAnimation()
    {
        roundChangeUIObject.SetActive(true);
        roundCount++;
        newRoundText.DOFade(1, 0.5f);
        newRoundText.text =  "Round " + roundCount.ToString();

        newRoundText.DOFade(0, 0.5f).SetDelay(2f).OnComplete(() =>
        {
          GameManager.GetInstance().SetGameState(GameState.DealingChamberCards);
            ShowNextRoundStarter();
            roundChangeUIObject.SetActive(false);
        });
    }
    public void ShowNextRoundStarter()
    {
        TutorialManager.Instance.ShowTutorial(TutorialType.NextRoundStarter);
    }

    void SaveProgress()
    {
        PlayerPrefs.SetInt("currentSet", currentSetNumber);
        PlayerPrefs.SetInt("roundCount", roundCount);
        PlayerPrefs.Save();
    }

    void LoadProgress()
    {
        currentSetNumber = PlayerPrefs.GetInt("currentSet", 1);
        roundCount = PlayerPrefs.GetInt("roundCount", 1);
    }

    // Method to reset everything including sets, rounds, and saved data
    public void ResetAll()
    {
        currentSetNumber = 1;
        roundCount = 0;
       // playerHandManager.currentSetPoint = 0;

        PlayerPrefs.DeleteKey("currentSet");
        PlayerPrefs.DeleteKey("roundCount");
        PlayerPrefs.Save();
        UpdateSet_RoundCount();

        //playerHandManager.pointText.text = playerHandManager.currentSetPoint.ToString();
    }
}
