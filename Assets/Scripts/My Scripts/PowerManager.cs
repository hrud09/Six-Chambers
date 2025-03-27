using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum PowerType
{
    Ghost, // Choose a chamber, that chamber can’t win
    Investigate, // Choose a chamber, reveals all cards in that chamber 
    Crystal_Ball, // Adds the turn (4th card) to the flop 
   
    //Later---------------------------------------------------------------
    
    Portal_Gun, // Switches the bullets between two chambers.
    Money_Printer, // Collect 2x  gold from the chosen chamber, receive 2x damage from lost hands
    Confetti_Blaster, // Turns face-up card(s) in the chosen chamber into a wild suit *
    Joker // Replaces a card in the flop with a wild card *
}

public class PowerManager : MonoBehaviour
{
    public List<Power> purchasedPowers = new List<Power>();
    public Power choosenPowerToGamble;
    public Power activePower;

    public PokerEvaluator pokerEvaluator;

    public Transform unlockedPowersParent;
    public Transform shopPowersParent;

    public List<Power> lockedPowers = new List<Power>();


    // Method to hide all power buttons except the selected one
    public void HideOtherPowers(Power selectedPower)
    {
        foreach (Power power in purchasedPowers)
        {
            if (power != selectedPower)
            {
                power.gameObject.SetActive(false);
            }
        }
    }

    // Method to unhide all power buttons
    public void UnhideAllPowers()
    {
        foreach (Power power in purchasedPowers)
        {
            power.gameObject.SetActive(true);
        }
    }
    public void RefreshPowerLists()
    {
        lockedPowers.Clear();
        purchasedPowers.Clear();

        foreach (Power power in GetComponentsInChildren<Power>(true))
        {
            power.InitiatePower();
        }
    }

}

[System.Serializable]
public class PowerInfo
{
    public string powerName;
    public PowerType powerType;
    public int powerCost;
}
