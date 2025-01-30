using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public ChamberManager chamberManager;
    public Chamber playerChosenChamber;
    public bool mouseOverChambers;

    public SetAndRoundManager setAndRoundManager;
    public GameManager gameManager;

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

    public void SelectPlayerChamber(Chamber _chosenChamber)
    {
        playerChosenChamber = _chosenChamber;
        chamberManager.PickRangersHand();
      
    }

    public void CheckSelectedChamber(Chamber winningChamber)
    {
        if (playerChosenChamber == chamberManager.rangerChosenChamber)
        {
            // Player and Ranger chose the same chamber
            if (chamberManager.winOrLoseSelectionInt == 1)
            {
                // Player predicted a WIN
                if (winningChamber == playerChosenChamber)
                {
                    // Prediction was correct
                    HandlePlayerWin(winningChamber.GetChambersAllBullets(), 2);
                }
                else
                {
                    // Prediction was wrong
                    HandlePlayerLose(winningChamber.GetChambersAllBullets(), 2);
                }
            }
            else if (chamberManager.winOrLoseSelectionInt == 0)
            {
                // Player predicted a LOSE
                if (winningChamber != playerChosenChamber)
                {
                    // Prediction was correct
                    HandlePlayerWin(winningChamber.GetChambersAllBullets(), 2);
                }
                else
                {
                    // Prediction was wrong
                    HandlePlayerLose(winningChamber.GetChambersAllBullets(), 2);
                }
            }
        }
        else if (playerChosenChamber == winningChamber)
        {
            // Player's chosen chamber won
            HandlePlayerWin(winningChamber.GetChambersAllBullets());
        }
        else if (chamberManager.rangerChosenChamber == winningChamber)
        {
            // Ranger's chosen chamber won
            List<GameObject> bullets = new List<GameObject>();
            bullets.AddRange(playerChosenChamber.GetChambersAllBullets());
            bullets.AddRange(winningChamber.GetChambersAllBullets());
            HandlePlayerLose(bullets);
        }
        else
        {
            // Player chose a losing chamber
            print("Normal Lose");
            HandlePlayerLose(playerChosenChamber.GetChambersAllBullets());
        }


    }

    public void CheckSelectedChamber(List<Chamber> winningChambers)
    {
        List<GameObject> playerChamberBullets = playerChosenChamber.GetChambersAllBullets();
        bool playerWon = winningChambers.Contains(playerChosenChamber);


        if (playerWon)
        {
            HandlePlayerWin(playerChamberBullets);
        }
        else
        {
            HandlePlayerLose(playerChamberBullets);
        }
    }

    private void HandlePlayerWin(List<GameObject> bulletsReceived, int damagePerBullet = 1)
    {
        chamberManager.rangerManager.damageTakenText.enabled = true;
        chamberManager.rangerManager.damageTakenText.text = "-" + (damagePerBullet * bulletsReceived.Count).ToString();
        print(bulletsReceived.Count);
        // Animate text once (before taking damage)
        AnimateDamageText(chamberManager.rangerManager.damageTakenText, -1);

        Sequence damageSequence = DOTween.Sequence();
        float initialDelay = 1f; // Delay before first damage
        float perBulletDelay = 1f; // Delay between each bullet hit

        damageSequence.AppendInterval(initialDelay);

        for (int i = 0; i < bulletsReceived.Count; i++)
        {
            damageSequence.AppendCallback(() =>
            {
                chamberManager.rangerManager.TakeDamage(damagePerBullet);
            });

            if (i < bulletsReceived.Count - 1)
            {
                damageSequence.AppendInterval(perBulletDelay);
            }
        }
    }

    private void HandlePlayerLose(List<GameObject> bulletsReceived, int damagePerBullet = 1)
    {
        damageTakenText.enabled = true;
        damageTakenText.text = "-" + (damagePerBullet * bulletsReceived.Count).ToString();
        print(bulletsReceived.Count);
        // Animate text once (before taking damage)
        AnimateDamageText(damageTakenText, 1);

        Sequence damageSequence = DOTween.Sequence();
        float initialDelay = 1f; // Delay before first damage
        float perBulletDelay = 1f; // Delay between each bullet hit

        damageSequence.AppendInterval(initialDelay);

        for (int i = 0; i < bulletsReceived.Count; i++)
        {
            damageSequence.AppendCallback(() =>
            {
                TakeDamage(damagePerBullet);
            });

            if (i < bulletsReceived.Count - 1)
            {
                damageSequence.AppendInterval(perBulletDelay);
            }
        }
    }

    private void AnimateDamageText(TMP_Text damageText, int yDirection)
    {
        RectTransform textTransform = damageText.rectTransform;

        // Reset before animation (to avoid overlap issues)
        textTransform.DOKill();

        textTransform.localScale = Vector3.one * 0.0f; // Start slightly smaller

        // Sequence for pop-up effect
        Sequence textSequence = DOTween.Sequence();
        textSequence.Append(textTransform.DOScale(1.5f, 0.3f).SetEase(Ease.Linear)); // Pop up with bounce effect

        textSequence.AppendInterval(3); // Wait for a while
        // Hide text after animation
        textSequence.OnComplete(() => {

            GameManager.GetInstance().SetGameState(GameState.CollectingAllCards);
            damageText.enabled = false; 

        });
    }

}
