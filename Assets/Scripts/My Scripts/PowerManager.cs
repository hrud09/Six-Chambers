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

   

    public void UsePower(PowerType powerType, int chamberIndex = -1)
    {
        Debug.Log($"Attempting to use power: {powerType}");

        switch (powerType)
        {
            case PowerType.Ghost:
                ApplyGhost(chamberIndex);
                break;
            case PowerType.Portal_Gun:
                ApplyPortalGun(chamberIndex);
                break;
            case PowerType.Money_Printer:
            case PowerType.Investigate:
                ApplyMoneyPrinter(chamberIndex);
                break;
            case PowerType.Crystal_Ball:
                ApplyCrystalBall();
                break;
            case PowerType.Confetti_Blaster:
                ApplyConfettiBlaster(chamberIndex);
                break;
            case PowerType.Joker:
                ApplyJoker();
                break;
            default:
                Debug.LogError($"Unknown power type: {powerType}");
                break;
        }
    }

    private void ApplyGhost(int chamberIndex)
    {
        Debug.Log($"Applying Ghost power to chamber {chamberIndex}. This chamber cannot win.");
        // pokerEvaluator.DisableWinningForChamber(chamberIndex);
    }

    private void ApplyPortalGun(int chamberIndex)
    {
        Debug.Log($"Applying Portal Gun to chamber {chamberIndex}. Collecting 2x gold, receiving 2x damage if lost.");
        // pokerEvaluator.DoubleGoldForChamber(chamberIndex);
        // pokerEvaluator.DoubleDamageForLoss(chamberIndex);
    }

    private void ApplyMoneyPrinter(int chamberIndex)
    {
        Debug.Log($"Applying Money Printer/Investigate to chamber {chamberIndex}. Revealing all cards.");
        // pokerEvaluator.RevealCardsInChamber(chamberIndex);
    }

    private void ApplyCrystalBall()
    {
        Debug.Log("Applying Crystal Ball. Adding an extra (4th) card to the flop.");
        // pokerEvaluator.AddExtraFlopCard();
    }

    private void ApplyConfettiBlaster(int chamberIndex)
    {
        Debug.Log($"Applying Confetti Blaster to chamber {chamberIndex}. Turning face-up cards into a wild suit.");
        // pokerEvaluator.ConvertFaceUpCardsToWild(chamberIndex);
    }

    private void ApplyJoker()
    {
        Debug.Log("Applying Joker. Replacing a card in the flop with a wild card.");
        // pokerEvaluator.ReplaceFlopCardWithWild();
    }
}

[System.Serializable]
public class PowerInfo
{
    public string powerName;
    public PowerType powerType;
    public int powerCost;
}
