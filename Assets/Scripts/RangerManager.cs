using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangerManager : MonoBehaviour
{
    public int chipsCount;
    public List<Transform> availableChips;
    public CardManager cardManager;
    public void DepositeChips(int amount, Transform _parent)
    {
        for (int i = 0; i < amount; i++)
        {
            availableChips[i].parent = _parent;
            Transform item  = availableChips[i];
            int index = i;
            item.DOLocalJump(Vector3.zero + Vector3.up * 0.2f * (amount - index), 1, 1, 0.5f).SetDelay((amount - index) * 0.2f).OnComplete(() =>{

                if (index == amount - 1)
                {
                    cardManager.pokerEvaluator.RevealLastTwoCards();
                }
            
            });
        }
       
    }
}
