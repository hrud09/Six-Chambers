using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class PlayerEconomyManager : MonoBehaviour
{

    public static PlayerEconomyManager Instance;
    public float startingCredit;
    public float currentCredit;
    public TMP_Text currentCreditText;
    public float incrementSpeed = 0.01f; // Time between increments
    public UnityEvent onCreditValueChange;
    private void Awake()
    {
      if(Instance == null) Instance = this;
      else Destroy(this);
    }
    // Start is called before the first frame update
    void Start()
    {
        startingCredit = PlayerPrefs.GetFloat("Credit", 100);
        currentCredit = startingCredit;
        UpdateCreditUI(startingCredit);
    }

    public void UpdateCredit(float toAdd)
    {
        float targetCredit = currentCredit + toAdd;
        PlayerPrefs.SetFloat("Credit", targetCredit);
        StartCoroutine(AnimateCreditCount(currentCredit, targetCredit));
        currentCredit += toAdd;
        onCreditValueChange.Invoke();
    }

    private IEnumerator AnimateCreditCount(float startCredit, float targetCredit)
    {
        float _currentCredit = startCredit;
        int step = startCredit < targetCredit ? 1 : -1; // Determine whether to increment or decrement
        while (_currentCredit != targetCredit)
        {
            _currentCredit += step;
            UpdateCreditUI(_currentCredit);
            yield return new WaitForSeconds(incrementSpeed);
        }
    }

    public void UpdateCreditUI(float _currentCredit)
    {
        currentCreditText.text = FormatCredit(_currentCredit);
    }

    private string FormatCredit(float value)
    {
        if (value >= 1_000_000_000_000) // Trillions
            return (value / 1_000_000_000_000f).ToString("0.##") + "t";
        else if (value >= 1_000_000_000) // Billions
            return (value / 1_000_000_000f).ToString("0.##") + "b";
        else if (value >= 1_000_000) // Millions
            return (value / 1_000_000f).ToString("0.##") + "m";
        else if (value >= 1_000) // Thousands
            return (value / 1_000f).ToString("0.##") + "k";
        else
            return value.ToString(); // Less than 1k, display as is
    }
}
