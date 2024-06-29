using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayersChipsManager : MonoBehaviour
{
    public Transform chipsTransform;
    public List<Transform> playersChips;
    public List<Transform> thisGamesChips;
    public int thisGamesChipsCount = 4;
    public ChamberManager chamberManager;
    public float proximityThreshold = 1.0f; // Adjust this value as needed

    public bool mouseArroundChambers;
    public Chamber chamberCloseToMouse;

    public float chipsMovementSpeed;
    public float chipsMovementGap;
    Vector3 targetPosition;
    public bool deposited;

    public bool playersTurn;
    void Start()
    {
        SetThisRoundChips();
    }

    public void SetThisRoundChips()
    {
        thisGamesChips = new List<Transform>();
        if (thisGamesChipsCount <= playersChips.Count)
        {
            for (int i = 0; i < thisGamesChipsCount; i++)
            {
                thisGamesChips.Add(playersChips[i]);
            }

        }
    }

    public void DepositChips(Transform _parent)
    {
        deposited = true;
        foreach (var item in thisGamesChips)
        {
            item.parent = _parent;
            item.DOLocalJump(Vector3.zero + Vector3.up * 0.2f * (thisGamesChips.Count - thisGamesChips.IndexOf(item)), 1, 1, 0.5f).SetDelay((thisGamesChips.Count - thisGamesChips.IndexOf(item)) * 0.2f);
            playersChips.Remove(item);
     
        }
        thisGamesChips.Clear();
    }
 
    void Update()
    {
        if (!playersTurn) return;
        if (!CheckMouseProximity())
        {

            for (int i = 0; i < thisGamesChips.Count; i++)
            {
                Transform chip = thisGamesChips[i];
                targetPosition = chipsTransform.position + Vector3.up * 0.2f * (thisGamesChips.Count - i);
                chip.position = Vector3.Lerp(chip.position, targetPosition, (thisGamesChips.Count - i + chipsMovementGap) * chipsMovementSpeed * Time.deltaTime);
            }
        }
        else
        {
            for (int i = 0; i < thisGamesChips.Count; i++)
            {
                Transform chip = thisGamesChips[i];
                if (mouseArroundChambers)
                {
                    targetPosition = chamberCloseToMouse.chipsHolderForChoosing.position + Vector3.up * 0.2f * (thisGamesChips.Count - i);
                }
                chip.position = Vector3.Lerp(chip.position, targetPosition, (thisGamesChips.Count - i + chipsMovementGap) * chipsMovementSpeed * Time.deltaTime);
            }
        }


    }
    bool CheckMouseProximity()
    {
        foreach (var item in chamberManager.chamberTransforms)
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = Camera.main.WorldToScreenPoint(item.position).z; // Maintain z position of the chips

            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

            float distanceX = Mathf.Abs(mouseWorldPosition.x - item.position.x);
            float distanceY = Mathf.Abs(mouseWorldPosition.y - item.position.y);
            float distanceZ = Mathf.Abs(mouseWorldPosition.z - item.position.z);

            if (distanceX <= proximityThreshold && distanceY <= proximityThreshold && distanceZ <= proximityThreshold)
            {
                return true;
            }
        }
        return false;
    }

}
