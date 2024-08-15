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
    private int pointsQuota;
    public PlayerHandManager playerHandManager;
    void Start()
    {
        currentSetNumber = PlayerPrefs.GetInt("currentSet", 1);
        roundCount = PlayerPrefs.GetInt("roundCount", 1);  
        SetPointsQuota();
        UpdateSetCount();
        UpdateRoundCount();
    }

    void SetPointsQuota()
    {

        pointsQuota = pointsQuotaSet[currentSetNumber - 1];

    }

    void UpdateSetCount()
    {
        setCountText.text = currentSetNumber.ToString();
        thisSetQuotaText.text = pointsQuota.ToString();
    }

    void UpdateRoundCount()
    {
        roundCountText.text = roundCount.ToString();
    }

    public void EndRound()
    {
        
        roundCount++;

        playerHandManager.gameManager.nextRoundButton.SetActive(true);
        if (roundCount >= 6)
        {
            if (playerHandManager.currentSetPoint >= pointsQuota)
            {
                PromoteToNextSet();
            }
            else
            {
                ResetRound();
            }
        }

        SaveProgress();
       // UpdateRoundCount();
       
    }

    public void NextRound()
    {
        playerHandManager.gameManager.RestartGame();
    }
    void PromoteToNextSet()
    {
        currentSetNumber++;
        PlayerPrefs.SetInt("currentSet", currentSetNumber);
        playerHandManager.currentSetPoint = 0; 
        playerHandManager.pointText.text = playerHandManager.currentSetPoint.ToString();
        SetPointsQuota();
        roundCount = 0;
        UpdateSetCount();
        UpdateRoundCount();
    }

    void ResetRound()
    {
        roundCount = 0;
        playerHandManager.currentSetPoint = 0;
        playerHandManager.pointText.text = playerHandManager.currentSetPoint.ToString();
    }

    void SaveProgress()
    {
        PlayerPrefs.SetInt("currentSet", currentSetNumber);
        PlayerPrefs.SetInt("roundCount", roundCount);
        PlayerPrefs.Save(); // Ensure data is saved
    }
}
