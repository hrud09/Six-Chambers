using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public CardInfo cardInfo;

    public SpriteRenderer cardFaceSpriteRend, cardBackSpriteRend;
    public GameObject playerSelectionAura, rangerSelectionAura;
    public GameObject topCardsAura;
    public void InitiateCard(CardInfo _cardInfo, bool faceUp)
    {
        cardInfo = _cardInfo;
        cardFaceSpriteRend.sprite = cardInfo.cardFaceSprite;
        if (faceUp)
        {

            cardFaceSpriteRend.enabled = true;
            cardBackSpriteRend.enabled = false;
        }
        else {

            cardFaceSpriteRend.enabled = false;
            cardBackSpriteRend.enabled = true;
        }
    }

    public void EnableTopCardVisual()
    {
        if(topCardsAura) topCardsAura.SetActive(true);
    }
}
