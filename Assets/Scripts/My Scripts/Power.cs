using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Power : MonoBehaviour
{
    public PowerInfo powerInfo;
    public Image lockedOverlay;
    public Button powerUseButton;
    public PowerManager powerManager;
    public TMP_Text costText;
    private void Start()
    {
        InitiatePower();
    }
    public void InitiatePower()
    {
        PowerData powerData = GameManager.GetInstance().GetPowerData();
        if (powerData != null)
        {
            if (powerData.GetPowerState(powerInfo.powerType))
            {
                lockedOverlay.enabled = false;
                UpdateUI();
                powerUseButton.onClick.RemoveAllListeners();
                powerUseButton.onClick.AddListener(ActivatePowerToUse);
            }
            else
            {
                lockedOverlay.enabled = true;
                UpdateUI();

                powerUseButton.onClick.RemoveAllListeners();
                powerUseButton.onClick.AddListener(GamblePowerToActivate);



            }
        }
    }

    private void UpdateUI()
    {
        costText.text = powerInfo.powerCost.ToString();
    }

    private void ActivatePowerToUse()
    {
       
        if (GameplayManager.currentRoundState == GameState.PlayersTurn)
        {
            if (powerManager.activePower == null)
            {
                powerManager.activePower = this;
                if (powerInfo.powerType == PowerType.Crystal_Ball)
                {

                    GameplayManager.GetInstance().cardManager.DrawAnotherCardOnBoard();
                }
                else
                {

                GameplayManager.currentRoundState = GameState.PowerInUse;
                }
            }
        }
    }

    private void GamblePowerToActivate()
    {
        if (powerManager.choosenPowerToGamble == null && GameplayManager.currentRoundState == GameState.PlayersTurn)
        {
            EconomyData economyData = GameManager.GetInstance().GetEconomyData();
            if (economyData != null)
            {

                if (economyData.coinCount >= powerInfo.powerCost)
                {
                    PlayerEconomyManager.Instance.UpdateCredit(-powerInfo.powerCost);
                    powerManager.choosenPowerToGamble = this;

                }
                else
                {
                    ToastMessageManager.GetInstance().ShowToastMessage("Not Enough Coins!!", 1);

                }
            }
        }
    }
}
