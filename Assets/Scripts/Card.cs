using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public CardInfo cardInfo;

    public MeshRenderer cardMesh;
    public GameObject playerSelectionAura, rangerSelectionAura;
    public GameObject topCardsAura;
    public void InitiateCard(CardInfo _cardInfo)
    {
        cardInfo = _cardInfo;
        cardMesh.material.mainTexture = cardInfo.cardTexture.texture;
    }

    public void EnableTopCardVisual()
    {
        if(topCardsAura) topCardsAura.SetActive(true);
    }
}
