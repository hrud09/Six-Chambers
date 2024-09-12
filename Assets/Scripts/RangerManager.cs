using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangerManager : MonoBehaviour
{
    public int chipsCount;
    public List<Transform> availableChips;
    public CardManager cardManager;
    public Chamber rangerSelectedChamber;
    public PlayerManager playerHandManager;
    public void SelectRangerChamber(Chamber _selectedChamber)
    {
        rangerSelectedChamber = _selectedChamber;
        rangerSelectedChamber.chamberCards[0].playerSelectionAura.SetActive(false);
        rangerSelectedChamber.chamberCards[0].rangerSelectionAura.SetActive(true);
        cardManager.pokerEvaluator.RevealLastTwoCards();
        if (playerHandManager.playerChosenChamber == rangerSelectedChamber)
        {
           // playerHandManager.chooseAgainPopUp.SetActive(true);

        }
    }
}
