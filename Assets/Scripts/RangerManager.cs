using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RangerManager : MonoBehaviour
{
    public int chipsCount;
    public List<Transform> availableChips;
    public PokerEvaluator pokerEvaluator;
    public CardManager cardManager;
    public Chamber rangerSelectedChamber;
    public PlayerManager playerHandManager;
    public bool chamberSelected, sameChamberSelected;


    [Header("Health Section")]
    public static int maxHealth = 25;
    public int health;
    public TMP_Text healthText, healthTextShadow;
    public Image healthFill;


    private void Start()
    {
        health = maxHealth;
        healthFill.fillAmount = 1;
    }

    public void TakeDamage(int damageAmount)
    {
        health -= damageAmount;
        healthText.text = health.ToString();
        healthTextShadow.text = health.ToString();
        healthFill.fillAmount = (health / maxHealth);
    }


    public void SelectRangerChamber(Chamber _selectedChamber)
    {
      
        rangerSelectedChamber = _selectedChamber;
        rangerSelectedChamber.chamberCards[0].playerSelectionAura.SetActive(false);
        rangerSelectedChamber.chamberCards[0].rangerSelectionAura.SetActive(true);
        if (playerHandManager.playerChosenChamber == rangerSelectedChamber)
        {
            chamberSelected = true;
            sameChamberSelected = true;
            rangerSelectedChamber.WinOrLoseSelectionPopUp.SetActive(true);

        }
        else
        {
            TutorialManager.Instance.ShowTutorial(TutorialType.DealNextCard);

            chamberSelected = true;
        }
    }
}
