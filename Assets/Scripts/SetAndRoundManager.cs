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

    // Fixed point quotas for each set
    public int[] pointsQuotaSet;

    public TMP_Text thisSetQuotaText;
    public int pointsQuota;
    public PlayerManager playerHandManager;
    private void Awake()
    {
        
        LoadProgress();
        SetPointsQuota();
        UpdateSetCount();
        UpdateRoundCount();
    }
    void Start()
    {
    }

    void SetPointsQuota()
    {
        pointsQuota = pointsQuotaSet[currentSetNumber - 1];
    }

    void UpdateSetCount()
    {
        setCountText.text = "set - " + currentSetNumber.ToString();
        //thisSetQuotaText.text =pointsQuota.ToString();
    }

    void UpdateRoundCount()
    {
        roundCountText.text = "round - " + roundCount.ToString();
    }

    public void EndRound()
    {
        roundCount++;

        playerHandManager.gameManager.nextRoundButton.SetActive(true);
       /* if (roundCount >= 6)
        {
          *//*  if (playerHandManager.currentSetPoint >= pointsQuota)
            {
                PromoteToNextSet();
            }
            else
            {
                ResetRound();
            }*//*
        }
*/
        SaveProgress();
        Invoke("NextRound", 3);
    }

    public void NextRound()
    {
        playerHandManager.gameManager.RestartGameForLoadingNextLevel();
    }

    void PromoteToNextSet()
    {
        currentSetNumber++;
        PlayerPrefs.SetInt("currentSet", currentSetNumber);
       // playerHandManager.currentSetPoint = 0;
       // playerHandManager.pointText.text = playerHandManager.currentSetPoint.ToString();
        SetPointsQuota();
        roundCount = 0;
        UpdateSetCount();
        UpdateRoundCount();
    }

    void ResetRound()
    {
        roundCount = 0;
       // playerHandManager.currentSetPoint = 0;
       // playerHandManager.pointText.text = playerHandManager.currentSetPoint.ToString();
    }

    void SaveProgress()
    {
        PlayerPrefs.SetInt("currentSet", currentSetNumber);
        PlayerPrefs.SetInt("roundCount", roundCount);
        PlayerPrefs.Save(); // Ensure data is saved
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

        SetPointsQuota();
        UpdateSetCount();
        UpdateRoundCount();

        //playerHandManager.pointText.text = playerHandManager.currentSetPoint.ToString();
    }
}
