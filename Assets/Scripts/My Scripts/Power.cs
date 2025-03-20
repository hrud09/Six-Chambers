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

        if (GameplayManager.GetInstance().GetCurrentGameState() == GameState.PlayersTurn)
        {
            if (powerManager.activePower == null)
            {
                powerManager.HideOtherPowers(this);
                powerManager.activePower = this;
                if (powerInfo.powerType == PowerType.Crystal_Ball)
                {

                    GameplayManager.GetInstance().cardManager.DrawAnotherCardOnBoard();
                }
                else
                {

                    GameplayManager.GetInstance().SetGameState(GameState.PowerInUse);
                }
            }
        }
    }

    private void GamblePowerToActivate()
    {
        if (powerManager.choosenPowerToGamble == null && GameplayManager.GetInstance().GetCurrentGameState() == GameState.PlayersTurn)
        {
            EconomyData economyData = GameManager.GetInstance().GetEconomyData();
            if (economyData != null)
            {

                if (economyData.coinCount >= powerInfo.powerCost)
                {
                    powerManager.HideOtherPowers(this);
                    GameplayManager.GetInstance().GetPlayerEconomy().UpdateCredit(-powerInfo.powerCost);
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
