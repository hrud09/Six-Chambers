using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChamberManager : MonoBehaviour
{
    public Transform[] chamberTransforms;
    public List<Chamber> chambers;
    public Chamber rangerChosenChamber;
    public Chamber playerChosenChamber;

    public PlayersChipsManager playerChipsManager;
    public RangerManager rangerManager;

    public IEnumerator PickRangersHand()
    {
        yield return new WaitForSeconds(1);
        rangerChosenChamber = chamberTransforms[Random.Range(0, chamberTransforms.Length)].GetComponent<Chamber>();
        rangerManager.DepositeChips(4, rangerChosenChamber.chipsHolderForDeposit);
    }

    
}
