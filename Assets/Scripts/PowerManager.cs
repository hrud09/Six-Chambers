using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum PowerType { 

    PreRound,
    MidRound,
    PostRound
}
public class PowerManager : MonoBehaviour
{
    public Power[] allPowers;
    public List<PowerCard> powerCards;
    public RectTransform powerCardsTransform;
    public GameObject powerCardPrefab;
    public float cardReloadTime;

    public PokerEvaluator pokerEvaluator;
    void Start()
    {
        ActivatePowerCards(PowerType.PreRound);
    }
    public void ActivatePowerCards(PowerType powerType)
    {
        foreach (PowerCard card in powerCards)
        {
            if (card.power.powerType == powerType)
            {
                card.gameObject.SetActive(true);
            }
            else { 
            
                card.gameObject.SetActive(false);
            }
        }
    }
    public void RevealOneCardPower() { 
    
        pokerEvaluator.RevealRandomChambersSecondCard();
    
    }
    
    public void RevealOneCardOnBoardPower() { 
    
        pokerEvaluator.RevealOneCardFromBoard();
    
    }

    public void RefillPowerCards()
    {
        if (powerCards.Count < 3)
        {
            for (int i = 0; i < 3 - powerCards.Count; i++)
            {

                GameObject powerCard = Instantiate(powerCardPrefab, powerCardsTransform);
                PowerCard powerCardScrpt = powerCard.GetComponent<PowerCard>();
                powerCardScrpt.powerManager = this;
                powerCards.Add(powerCardScrpt);
            }
        }
    }
}

[System.Serializable]
public class Power {

    public PowerType powerType;
    public float powerCost;
    public UnityEvent activatePowerButtonAction;

}