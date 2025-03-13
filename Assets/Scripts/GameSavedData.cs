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


[System.Serializable]
public class PowerData
{
    public List<PowerType> powerTypes;   // Keys (Power Type)
    public List<bool> powerValues;       // Values (Whether the power is active/unlocked)

    public PowerData()
    {
        powerTypes = new List<PowerType>();
        powerValues = new List<bool>();
    }

    public void SetPowerState(PowerType type, bool state)
    {
        int index = powerTypes.IndexOf(type);
        if (index >= 0)
        {
            powerValues[index] = state;
        }
        else
        {
            powerTypes.Add(type);
            powerValues.Add(state);
        }
    }

    public bool GetPowerState(PowerType type)
    {
        int index = powerTypes.IndexOf(type);
        return index >= 0 ? powerValues[index] : false;
    }
}
