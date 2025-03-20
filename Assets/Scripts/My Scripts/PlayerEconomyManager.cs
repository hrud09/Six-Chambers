using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class PlayerEconomyManager : MonoBehaviour
{
   // public static PlayerEconomyManager Instance;
    public TMP_Text currentCreditText;
    public float incrementSpeed = 0.01f; // Time between increments
    public UnityEvent onCreditValueChange;

    private EconomyData economyData;

    void Start()
    {
        LoadEconomyData();
    }

    private void LoadEconomyData()
    {
        economyData = GameManager.GetInstance().GetEconomyData();
        if (economyData == null)
        {
            economyData = new EconomyData(0); // Default starting credit
            SaveLoadManager.SaveEconomyData(economyData);
        }
        UpdateCreditUI(economyData.coinCount);
    }

    public void UpdateCredit(int toAdd)
    {
        int targetCredit = economyData.coinCount + toAdd;
        economyData.coinCount = targetCredit;
        SaveLoadManager.SaveEconomyData(economyData);
        StartCoroutine(AnimateCreditCount(economyData.coinCount - toAdd, targetCredit));
        onCreditValueChange.Invoke();
    }

    private IEnumerator AnimateCreditCount(int startCredit, int targetCredit)
    {
        int _currentCredit = startCredit;
        int step = startCredit < targetCredit ? 1 : -1; // Determine increment or decrement
        while (_currentCredit != targetCredit)
        {
            _currentCredit += step;
            UpdateCreditUI(_currentCredit);
            yield return new WaitForSeconds(incrementSpeed);
        }
    }

    public void UpdateCreditUI(int _currentCredit)
    {
        currentCreditText.text = FormatCredit(_currentCredit);
    }

    private string FormatCredit(int value)
    {
        if (value >= 1_000_000_000_000) return (value / 1_000_000_000_000f).ToString("0.##") + "t";
        else if (value >= 1_000_000_000) return (value / 1_000_000_000f).ToString("0.##") + "b";
        else if (value >= 1_000_000) return (value / 1_000_000f).ToString("0.##") + "m";
        else if (value >= 1_000) return (value / 1_000f).ToString("0.##") + "k";
        else return value.ToString();
    }

    public void ResetEconomy()
    {
        economyData = GameManager.GetInstance().GetEconomyData();
        economyData.coinCount = 0;
        SaveLoadManager.SaveEconomyData(economyData);
        UpdateCreditUI(economyData.coinCount);
    }
}
