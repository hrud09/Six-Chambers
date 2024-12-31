using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChamberManager : MonoBehaviour
{
    public Transform[] chamberTransforms;
    public List<Chamber> chambers;
    public Chamber rangerChosenChamber;

    public PlayerManager playerHandManager;
    public RangerManager rangerManager;

    public float totalSelectionTime; // Total time for the selection process
    public float initialDelay; // Initial delay between each selection

    public GameObject chipPrefab;

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

    private IEnumerator PickRangerHandCoroutine()
    {
        float elapsedTime = 0.0f;
        int currentIndex = 0;
        int totalChambers = chamberTransforms.Length;
        totalSelectionTime = Random.Range(3f, 4f);
        initialDelay = Random.Range(0.18f, 0.22f);
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
