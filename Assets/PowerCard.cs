using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;
using TMPro;

public class PowerCard : MonoBehaviour
{
    public Power power;
    public bool isSelected;
    public Image cardImages;
    public Sprite cardImageSprites;

    public PowerManager powerManager;
    private Button powerUseButton;

    public TMP_Text powerUseCostText;
    private void Awake()
    {
        powerUseButton = GetComponent<Button>();
    }
    void Start()
    {
        UpdateUI();
    }



    public void UsePower()
    {
        if (PlayerEconomyManager.Instance.currentCredit >= power.powerCost)
        {
            PlayerEconomyManager.Instance.UpdateCredit(-power.powerCost);
            power.powerCost = power.powerCost * 1.2f; 
            UpdateUI();
            Vector3 intialRotation = cardImages.transform.rotation.eulerAngles;
            cardImages.transform.DORotate(Vector3.up * 90 + intialRotation, 0.2f).OnComplete(() =>
            {

                cardImages.sprite = cardImageSprites;
                cardImages.transform.DORotate(intialRotation, 0.3f).OnComplete(() =>
                {
                    powerManager.powerCards.Remove(this);
                    // powerManager.RefillPowerCards();
                    power.activatePowerButtonAction.Invoke();
                    Destroy(gameObject, 0.2f);

                });
            });


        }
    }


    public void UpdateUI()
    {

        powerUseCostText.text = power.powerCost.ToString();
    }
}
