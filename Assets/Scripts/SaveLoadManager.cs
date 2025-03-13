using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveLoadManager
{
    private static string GetFilePath(string fileName)
    {
        return Path.Combine(Application.persistentDataPath, fileName);
    }

    #region PREFERENCE_DATA
    public static void SavePreference(PreferenceData data)
    {
        SaveData(data, GetFilePath("preference.dat"));
    }

    public static PreferenceData LoadPreferenceData()
    {
        return LoadData<PreferenceData>(GetFilePath("preference.dat"));
    }
    #endregion

    #region ECONOMY_DATA
    public static void SaveEconomyData(EconomyData data)
    {
        SaveData(data, GetFilePath("economy.dat"));
    }

    public static EconomyData LoadEconomyDataData()
    {
        return LoadData<EconomyData>(GetFilePath("economy.dat"));
    }
    #endregion

    #region PLAYER_DATA
    public static void SavePlayerData(PlayerData data)
    {
        SaveData(data, GetFilePath("playerData.dat"));
    }

    public static PlayerData LoadPlayerDataData()
    {
        return LoadData<PlayerData>(GetFilePath("playerData.dat"));
    }
    #endregion

    #region META_DATA
    public static void SaveGameMetaData(GameMetaData data)
    {
        SaveData(data, GetFilePath("gameMetaData.dat"));
    }

    public static GameMetaData LoadGameMetaData()
    {
        return LoadData<GameMetaData>(GetFilePath("gameMetaData.dat"));
    }
    #endregion

    #region POWER_DATA
    public static void SavePowerData(PowerData data)
    {
        SaveData(data, GetFilePath("powerData.dat"));
    }

    public static PowerData LoadPowerData()
    {
        return LoadData<PowerData>(GetFilePath("powerData.dat")) ?? new PowerData();
    }
    #endregion

    #region GENERIC SAVE/LOAD METHODS
    private static void SaveData<T>(T data, string filePath)
    {
        try
        {
            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(stream, data);
            }
            Debug.Log($"Data saved successfully at {filePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save data to {filePath}: {e.Message}");
        }
    }

    private static T LoadData<T>(string filePath) where T : class
    {
        if (File.Exists(filePath))
        {
            try
            {
                using (FileStream stream = new FileStream(filePath, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    return bf.Deserialize(stream) as T;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load data from {filePath}: {e.Message}");
            }
        }
        else
        {
            Debug.LogWarning($"Save file not found at {filePath}");
        }
        return null;
    }
    #endregion
}
