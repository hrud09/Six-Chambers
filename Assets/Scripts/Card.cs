using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public CardInfo cardInfo;

    public SpriteRenderer frontFaceRend, backFaceRend;
    public GameObject playerSelectionAura, rangerSelectionAura;
    public GameObject topCardsAura;
    public void InitiateCard(CardInfo _cardInfo, bool faceUp)
    {
        cardInfo = _cardInfo;
        frontFaceRend.sprite = cardInfo.cardTexture;
        if (faceUp)
        {
            frontFaceRend.enabled = true;
            backFaceRend.enabled = false;
        }
        else
        {
            frontFaceRend.enabled = faceUp;
            backFaceRend.enabled = true;
        }
       
    }

    public void EnableTopCardVisual()
    {
        if(topCardsAura) topCardsAura.SetActive(true);
    }


}
