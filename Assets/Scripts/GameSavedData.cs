using System.Collections.Generic;


[System.Serializable]
public class PreferenceData
{
    public bool isBgmOn;
    public bool isSfxOn;
    public bool isVibrationOn;

    public PreferenceData(bool isBgmOn, bool isSfxOn, bool isVibrationOn)
    {
        this.isBgmOn = isBgmOn;
        this.isSfxOn = isSfxOn;
        this.isVibrationOn = isVibrationOn;
    }
}

[System.Serializable]
public class EconomyData
{
    public int coinCount;

    public EconomyData(int coinCount)
    {
        this.coinCount = coinCount;
    }
}

[System.Serializable]
public class PlayerData
{
    public int currentLevelId;
    // public Dictionary<PowerupType, int> powerupData;

    public PlayerData(int currentLevelId)
    {
        this.currentLevelId = currentLevelId;
        // this.powerupData = powerupData;
    }
}

[System.Serializable]
public class GameMetaData
{
    public System.DateTime freeLuckySpinTime;
    //public System.DateTime freeCoinCollectionTime;
   
    public GameMetaData()
    {
        freeLuckySpinTime = Statics.resetDateTime;
        //freeCoinCollectionTime = Statics.resetDateTime;
    }
}
