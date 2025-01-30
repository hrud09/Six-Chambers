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


    [Header("Health Section")]
    public static int maxHealth = 25;
    public int health;
    public TMP_Text healthText, healthTextShadow;
    public Image healthFill;
    public TMP_Text damageTakenText;


    [Header("Revolver & Bullets")]
    public List<GameObject> playersBulletsToShoot;
    public static int maxBullets = 6;
    public static int totalBulletsShotThisRound;
    public bool isShooting;
    public Weapon weapon;

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
        healthFill.fillAmount = (float)health / maxHealth;

        // Shake the health fill image
        ShakeHealthFill();
    }

    private void ShakeHealthFill()
    {
        // Shake the health fill image
        RectTransform healthFillRect = healthFill.GetComponent<RectTransform>();
        if (healthFillRect != null)
        {
            // Reset the position before shaking (to avoid cumulative offsets)
            healthFillRect.anchoredPosition = Vector2.zero;

            // Shake the fill image
            healthFillRect.DOShakeAnchorPos(
                duration: 0.5f, // Duration of the shake
                strength: 10f,  // Strength of the shake
                vibrato: 10,    // Vibrato (how much it shakes)
                randomness: 90, // Randomness of the shake
                snapping: false // Whether to snap to whole pixel values
            ).SetEase(Ease.OutQuad); // Easing for the shake effect
        }
    }

    public void SelectRangerChamber(Chamber _selectedChamber)
    {
      
        rangerSelectedChamber = _selectedChamber;
        rangerSelectedChamber.chamberCards[0].playerSelectionAura.SetActive(false);
        rangerSelectedChamber.chamberCards[0].rangerSelectionAura.SetActive(true);
        if (playerHandManager.playerChosenChamber == rangerSelectedChamber)
        {
            GameManager.GetInstance().SetGameState(GameState.SameChamberSelected);
            rangerSelectedChamber.WinOrLoseSelectionPopUp.SetActive(true);

        }
        else
        {
            TutorialManager.Instance.ShowTutorial(TutorialType.DealNextCard);

            GameManager.GetInstance().SetGameState(GameState.DealingBoardCards2);
        }
    }
}
