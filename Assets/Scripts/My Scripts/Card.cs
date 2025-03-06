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
    public Vector3 initialPosition, initialRotation;
    public void InitiateCard(CardInfo _cardInfo, bool faceUp = false)
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
    public void ResetPositionAndRotation()
    {

    }
    public void EnableTopCardVisual()
    {
        if(topCardsAura) topCardsAura.SetActive(true);
    }
    public void RevealCard()
    {
        transform.rotation = Quaternion.Euler(0, 0, 180);
        transform.DOLocalMoveY(0.5f, 0.5f);
        Vector3 rot = transform.rotation.eulerAngles;
        rot.z = 30;
        transform.DORotate(rot, 0.3f).OnComplete(() =>
        {
            rot.z = 0;
            frontFaceRend.enabled = true;
            backFaceRend.enabled = false;
            transform.DOLocalMoveY(0f, 0.5f);
            transform.DORotate(rot, 0.2f).SetEase(Ease.OutBounce);
            });

    }

}
