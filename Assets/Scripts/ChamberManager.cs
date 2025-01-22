using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChamberManager : MonoBehaviour
{
    public Transform[] chamberTransforms;
    public List<Chamber> chambers;
    public Chamber rangerChosenChamber;

    public PlayerManager playerHandManager;
    public RangerManager rangerManager;

    public float totalSelectionTime; // Total time for the selection process
    public float initialDelay; // Initial delay between each selection

    public bool allChambersCardsRevealed;

    public bool winLosePredicted;
    public int winOrLoseSelectionInt;
    private void Awake()
    {
        InitiateChambers();
    }
    public void InitiateChambers()
    {
        foreach (var chamber in chambers)
        {
            chamber.chamberIndex = chambers.IndexOf(chamber) + 1;

        }

    }
    public void PickRangersHand()
    {
        StartCoroutine(PickRangerHandCoroutine());
    }
   

    public void RevealAllHandCardsAtOnce()
    {
        TutorialManager.Instance.ShowTutorial(TutorialType.ShowDown);
        StartCoroutine(RevealHandCards());
    }
    private IEnumerator RevealHandCards()
    {
        foreach (var chamber in chambers)
        {
            int index = chambers.IndexOf(chamber);
            chamber.chamberCards[0].transform
                .DOLocalRotate(Vector3.zero, 0.5f)
                .OnComplete(() => {
                    chamber.chamberCards[0].backFaceRend.enabled = false;

                     chamber.chamberCards[0].frontFaceRend.enabled = true;
                     chamber.chamberCards[0].transform.DOLocalMoveZ(0.8f, 0.2f);
                     PokerEvaluator.Instance.CheckHand(chamber);

                });
            yield return new WaitForSeconds(0.5f);
        }
        PokerEvaluator.Instance.CheckPokerLogics();

        yield return new WaitForSeconds(0.5f);
        allChambersCardsRevealed = true;
    }

    private IEnumerator PickRangerHandCoroutine()
    {
        //Pre ranger selection

        float elapsedTime = 0.0f;
        int currentIndex = 0;
        int totalChambers = chamberTransforms.Length;
        totalSelectionTime = Random.Range(3f, 4f);
        initialDelay = Random.Range(0.18f, 0.22f);
        yield return new WaitForSeconds(2f);
        while (elapsedTime < totalSelectionTime)
        {
            rangerChosenChamber = chamberTransforms[currentIndex].GetComponent<Chamber>();
            // rangerManager.SelectRangerChamber(rangerChosenChamber);
            rangerChosenChamber.chamberCards[0].transform.localScale = Vector3.one * 1.2f;

            // Calculate the next delay (increase the delay gradually)
            float delay = initialDelay * (1 + (elapsedTime / totalSelectionTime));
            rangerChosenChamber.chamberCards[0].transform.DOScale(Vector3.one, delay);
            yield return new WaitForSeconds(delay);
            // Move to the next chamberIndex
            currentIndex = (currentIndex + 1) % totalChambers;
            elapsedTime += delay;
            //rangerChosenChamber.chamberCards[0].rangerSelectionAura.SetActive(false);
        }

        // Final selection
        rangerChosenChamber = chamberTransforms[currentIndex].GetComponent<Chamber>();
        rangerManager.SelectRangerChamber(rangerChosenChamber);
        // Print the final chamberIndex

    }

    public bool AllHandRevealed()
    {
        foreach (Chamber chamber in chambers) if (!chamber.handRevealed) return false;
        return true;
    }
    public void PlayHandChoosingAnimation()
    {
        StartCoroutine(PlayHandChoosingAnimationDelay());
    }
    private IEnumerator PlayHandChoosingAnimationDelay()
    {
        foreach (var item in chambers)
        {
            item.LiftCards();
            yield return new WaitForSeconds(0.1f);

            item.LowerCards();
        }

        yield return new WaitForSeconds(0.1f);
        TutorialManager.Instance.ShowTutorial(TutorialType.PlayersTurn);
        yield return new WaitForSeconds(0.2f);
        playerHandManager.playersTurn = true;
    }
}

[System.Serializable]
public class ChamberUIScript
{
    public GameObject playerSelectionVisualUI;
    public GameObject rangerSelectionVisualUI;
    public GameObject winningHandVisualUI;


    [Header("Hand Rank")]
    public CanvasGroup rankUICanvasGroup;
    public Image rankTextBG;

    public TMP_Text rankText;
    public Image[] topFiveCards;


    
}
