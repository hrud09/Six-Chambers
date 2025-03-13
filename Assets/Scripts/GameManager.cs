using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    private const int InitialCoinAmount = 0;

    private PreferenceData preferenceData;
    private EconomyData economyData;
    private PlayerData playerData;
    private GameMetaData gameMetaData;
    private PowerData powerData;

    public bool isDebug = true;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        Application.targetFrameRate = 60;

        InitializeSavedData();
    }

    private void Start()
    {
        OnPreferenceDataLoaded();
    }

    public static GameManager GetInstance()
    {
        return instance;
    }

    private void InitializeSavedData()
    {
        //Load preference Data
        preferenceData = SaveLoadManager.LoadPreferenceData();
        if (preferenceData == null)
        {
            //Load data is NULL, initialize with Default data and save
            preferenceData = new PreferenceData(true, true, true);
            SaveLoadManager.SavePreference(preferenceData);
        }


        //Load Economy Data
        economyData = SaveLoadManager.LoadEconomyDataData();
        if (economyData == null)
        {
            //Load data is NULL, initialize with Default data and save
            economyData = new EconomyData(InitialCoinAmount);
            SaveLoadManager.SaveEconomyData(economyData);
        }

        //Load Player Data
        playerData = SaveLoadManager.LoadPlayerDataData();
        if (playerData == null)
        {
            playerData = new PlayerData(1);

            playerData = new PlayerData(1);
            SaveLoadManager.SavePlayerData(playerData);
        }

        //Load Meta Data
        gameMetaData = SaveLoadManager.LoadGameMetaData();
        if (gameMetaData == null)
        {
            gameMetaData = new GameMetaData();
            SaveLoadManager.SaveGameMetaData(gameMetaData);
        }

        // Load Power Data
        powerData = SaveLoadManager.LoadPowerData();
        if (powerData == null)
        {
            powerData = new PowerData();
            SaveLoadManager.SavePowerData(powerData);
        }
    }

    #region Preference Data

    public PreferenceData GetPreferenceData()
    {
        return preferenceData;
    }

    public void UpdatePreferenceData(PreferenceData pData)
    {
        preferenceData = pData;
        SaveLoadManager.SavePreference(preferenceData);
    }

    private void OnPreferenceDataLoaded()
    {
        bool isBgmOn = preferenceData.isBgmOn;
        bool isSfxOn = preferenceData.isSfxOn;
        bool isVibrationOn = preferenceData.isVibrationOn;

        AudioManager.GetInstance().SetBGMPermissionValue(isBgmOn);
        AudioManager.GetInstance().SetSFXPermissionValue(isSfxOn);
        //VibrationManager.instance.SetVibration(isVibrationOn);
        AudioManager.GetInstance().Init();
    }

    #endregion


    #region Economy Data
    public int GetCurrentCoinAmount()
    {
        return economyData.coinCount;
    }

    public void AddCoins(int amount)
    {
        economyData.coinCount += amount;
        SaveLoadManager.SaveEconomyData(economyData);
        PlayerEconomyManager.Instance.UpdateCreditUI(economyData.coinCount);
    }

    public EconomyData GetEconomyData()
    {
        return economyData;
    }
    #endregion


    #region Player Data
    public PlayerData GetPlayerData()
    {
        return playerData;
    }

    public void UpdateCurrentLevel(int levelId)
    {
        playerData.currentLevelId = levelId;
        SaveLoadManager.SavePlayerData(playerData);
    }
    
    #endregion

    #region GameMetaData

    public GameMetaData GetGameMetaData()
    {
        return gameMetaData;
    }

    public void UpdateGameMetadata(GameMetaData metaData)
    {
        gameMetaData = metaData;
        SaveLoadManager.SaveGameMetaData(gameMetaData);
    }

    #endregion

    #region Power Data

    public PowerData GetPowerData()
    {
        return powerData;
    }

    public void UnlockPower(PowerType powerType)
    {
        powerData.SetPowerState(powerType, true);
        SaveLoadManager.SavePowerData(powerData);

        foreach (Power item in GameplayManager.GetInstance().powerManager.purchasedPowers)
        {
            if (item.powerInfo.powerType == powerType)
            {
                item.InitiatePower();
            }
        }
       
    }

    public bool IsPowerUnlocked(PowerType powerType)
    {
        return powerData.GetPowerState(powerType);
    }

    public void UpdatePowerData(PowerData pData)
    {
        powerData = pData;
        SaveLoadManager.SavePowerData(powerData);
    }

    #endregion
   
}
