using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;


public class PlayerManager : MonoBehaviour
{
    public ChamberManager chamberManager;
 
    public bool mouseOverChambers;
    public Chamber playerChosenChamber;

    Vector3 targetPosition;
    public bool chamberSelected;
    public bool playersTurn;


    public GameObject chooseAgainPopUp;
    public SetAndRoundManager setAndRoundManager;
    public GameManager gameManager;


    [Header("Chips System")]
    public Transform chipsParent;
    public TMP_Text currentChipsCountText;
    public List<GameObject> currentChips;

    public List<GameObject> wagerredChips;

    public int chipsCount;
    public GameObject chipPrefab;
    private void Start()
    {
        chipsCount = PlayerPrefs.GetInt("PlayerChipsCount", 6);
        for (int i = 0; i < chipsCount; i++)
        {
            GameObject chip = Instantiate(chipPrefab, chipsParent);
            chip.transform.localPosition = Vector3.up * i * 0.2f;
            currentChips.Add(chip);
        }   
        UpdateChipsCount();
    }
    public void SelectPlayerChamber(Chamber _choosenChamber)
    {
        chamberSelected = true;
        playerChosenChamber = _choosenChamber;
        int chipsAmountToWager = 1;

        if (playerChosenChamber.existingChips.Count == 0) chipsAmountToWager = 1;
        else if (currentChips.Count < playerChosenChamber.existingChips.Count) chipsAmountToWager = currentChips.Count;
        else if (playerChosenChamber.existingChips.Count > 0 && currentChips.Count > 0) chipsAmountToWager = playerChosenChamber.existingChips.Count;

        //Player hand
        for (int i = chipsAmountToWager - 1; i >= 0 ; i--)
        {
            Transform t = currentChips[i].transform;
            wagerredChips.Add(currentChips[i]);
            currentChips.Remove(currentChips[i]);
            t.parent = playerChosenChamber.wagerredChipsParent;
            t.DOLocalJump(Vector3.up * i * 0.2f, 2, 1, 0.5f).SetDelay(i * 0.3f);
        }

        //Chamber hand
        if (playerChosenChamber.existingChips.Count > 0)
        {
            /*  GameObject _chip = Instantiate(playerChosenChamber.chamberManager.chipPrefab, playerChosenChamber.chipsParent.position, Quaternion.identity);
              playerChosenChamber.existingChips.Add(_chip);*/
            for (int i = chipsAmountToWager - 1; i >= 0; i --)
            {
                Transform t = playerChosenChamber.existingChips[i].transform;
                wagerredChips.Add(playerChosenChamber.existingChips[i]);
                playerChosenChamber.existingChips.Remove(playerChosenChamber.existingChips[i]);
                t.DOLocalJump(Vector3.up * i * 0.2f, 2, 1, 0.5f).SetDelay(i * 0.3f + 0.5f);
                t.parent = playerChosenChamber.wagerredChipsParent;
            }
        }
        UpdateChipsCount();
        chamberManager.PickRangersHand();
    }

    public void CheckSelectedChamber(Chamber winningChamber, int point)
    {
        if (playerChosenChamber != chamberManager.rangerChosenChamber)
        {
            if (playerChosenChamber == winningChamber)
            {
                foreach (var chip in wagerredChips)
                {
                    Transform _chipTransform = chip.transform;
                    _chipTransform.parent = chipsParent;
                    _chipTransform.DOLocalJump(Vector3.zero, 5, 1, 1.5f);
                    if (!currentChips.Contains(chip)) currentChips.Add(chip);

                }


            }
            else if (chamberManager.rangerChosenChamber == winningChamber)
            {
                // If the Live Chamber wins, but it's not the player's chosen chamber, the player loses one chip from their personal stash.
                if (wagerredChips.Count == 1)
                {
                    if (currentChips.Count > 0)
                    {
                        wagerredChips.Add(currentChips[currentChips.Count - 1]);
                        currentChips.Remove(currentChips[currentChips.Count - 1]);
                        foreach (var chip in wagerredChips)
                        {
                            Transform _chipTransform = chip.transform;
                            _chipTransform.parent = winningChamber.chipsParent;
                            _chipTransform.DOLocalJump(Vector3.zero, 5, 1, 1.5f).SetDelay(wagerredChips.IndexOf(chip) * 0.1f);
                            winningChamber.existingChips.Add(chip);
                        }
                    }
                    else
                    {
                        //Lose
                        print("You Lost!");
                        return;
                    }
                }
                else if (currentChips.Count == 0)
                {

                    //Loses
                    print("You Lost!");
                    return;
                }
                else if(currentChips.Count > 0)
                {
                    wagerredChips.Add(currentChips[currentChips.Count - 1]);
                    currentChips.Remove(currentChips[currentChips.Count - 1]);
                    for (int i = 0; i < wagerredChips.Count; i++)
                    {
                        GameObject _chip = wagerredChips[i];
                        if (i < wagerredChips.Count/2)
                        {
                            _chip.transform.parent = playerChosenChamber.chipsParent;
                            playerChosenChamber.existingChips.Add(_chip);
                            _chip.transform.DOLocalJump(Vector3.zero, 5, 1, 1.5f).SetDelay(i * 0.1f);
                        }
                        else
                        {
                            _chip.transform.parent = winningChamber.chipsParent;
                            winningChamber.existingChips.Add(_chip);
                            _chip.transform.DOLocalJump(Vector3.zero, 5, 1, 1.5f).SetDelay(i * 0.1f);
                        }
                    }
                }
              
            }
            else
            {
                //player's hand doesn't win, different hand wins
                if (wagerredChips.Count == 1)
                {
                    Transform t = wagerredChips[0].transform;
                    t.parent = winningChamber.chipsParent;
                    t.DOLocalJump(Vector3.zero, 5, 1, 1.5f);
                    winningChamber.existingChips.Add(wagerredChips[0]);
                   
                }
                else
                {

                    for (int i = 0; i < wagerredChips.Count; i++)
                    {
                        Transform _chipTransform = wagerredChips[i].transform;
                        if (i % 2 == 0)
                        {
                            _chipTransform.parent = playerChosenChamber.chipsParent;
                            _chipTransform.DOLocalJump(Vector3.zero, 5, 1, 1f).SetDelay(wagerredChips.IndexOf(wagerredChips[i]) * 0.1f);
                            playerChosenChamber.existingChips.Add(_chipTransform.gameObject);
                        }
                        else
                        {

                            _chipTransform.parent = winningChamber.chipsParent;
                            _chipTransform.DOLocalJump(Vector3.zero, 5, 1, 1f).SetDelay(wagerredChips.IndexOf(wagerredChips[i]) * 0.1f);
                            winningChamber.existingChips.Add(wagerredChips[i]);
                           
                        }

                    }
                }
            }
        }
        else if (playerChosenChamber == chamberManager.rangerChosenChamber)
        {
            if (winningChamber == playerChosenChamber)
            {
                //If the Live Chamber is the chosen chamber and wins, the player loses three chips from their stash as an additional penalty for the increased risk.

                if (wagerredChips.Count == 1)
                {
                    if (currentChips.Count >= 3)
                    {
                        for (int i = currentChips.Count - 1; i >= currentChips.Count - 3; i--)
                        { 
                            wagerredChips.Add(currentChips[i]);
                            currentChips.Remove(currentChips[i]);
                        }
                        foreach (var chip in wagerredChips)
                        {
                            Transform _chipTransform = chip.transform;
                            _chipTransform.parent = winningChamber.chipsParent;
                            _chipTransform.DOLocalJump(Vector3.zero, 5, 1, 1f).SetDelay(wagerredChips.IndexOf(chip) * 0.1f);
                            winningChamber.existingChips.Add(chip);
                        }
                    }
                    else
                    {
                        //Lose
                        print("You Lost!");
                        return;
                    }
                }
                else if (currentChips.Count == 0)
                {

                    //Loses
                    print("You Lost!");
                    return;
                }
                else if (currentChips.Count >= 3)
                {
                    for (int i = currentChips.Count - 1; i >= currentChips.Count - 3; i--)
                    {
                        wagerredChips.Add(currentChips[i]);
                        currentChips.Remove(currentChips[i]);
                    }
                    for (int i = 0; i < wagerredChips.Count; i++)
                    {
                        GameObject _chip = wagerredChips[i];
                        if (i < wagerredChips.Count / 2)
                        {
                            _chip.transform.parent = playerChosenChamber.chipsParent;
                            playerChosenChamber.existingChips.Add(_chip);
                            _chip.transform.DOLocalJump(Vector3.zero, 5, 1, 1f).SetDelay(i * 0.1f);
                        }
                        else
                        {
                            _chip.transform.parent = winningChamber.chipsParent;
                            winningChamber.existingChips.Add(_chip);
                            _chip.transform.DOLocalJump(Vector3.zero, 5, 1, 1f).SetDelay(i * 0.1f);
                        }
                    }
                }
            }
            else if (winningChamber != playerChosenChamber)
            {
                // if the Live Chamber loses and it was the player's chosen chamber, the player is rewarded with one chip from every other chamber, in addition to the chips they wagered on that chamber.
                foreach (var chamber in playerChosenChamber.chamberManager.chambers)
                {

                    if (chamber.existingChips.Count > 0)
                    {
                        GameObject chip = chamber.existingChips[chamber.existingChips.Count - 1];
                        wagerredChips.Add(chip);
                        chamber.existingChips.Remove(chip);
                    }
                }
                for (int i = 0; i < wagerredChips.Count; i++) {

                    GameObject chip = wagerredChips[i];
                    chip.transform.parent = chipsParent;
                    chip.transform.DOLocalJump(Vector3.zero, 5, 1, 1f).SetDelay(i * 0.1f);
                }

                        
            }
        }

        // PlayerPrefs.SetInt("PlayerPoint", currentSetPoint);
        // pointText.text = currentSetPoint.ToString();

        UpdateChipsCount();
        playerChosenChamber.chamberManager.UpdateAllChipsCount();
        wagerredChips.Clear();
        setAndRoundManager.EndRound();

    }

    private void UpdateChipsCount()
    {
        chipsCount = currentChips.Count;
        PlayerPrefs.SetInt("PlayerChipsCount", chipsCount);
        currentChipsCountText.text = currentChips.Count.ToString();
    }
    public void CheckSelectedChamber(List<Chamber> winningChambers, int point)
    {
        print("Here Many");
        if (playerChosenChamber != chamberManager.rangerChosenChamber)
        {
            if (winningChambers.Contains(playerChosenChamber))
            {

             //   currentSetPoint += point;

            }
            else if (winningChambers.Contains(chamberManager.rangerChosenChamber))
            {
              //  currentSetPoint -= point;
            }
            else if (winningChambers.Contains(playerChosenChamber) && winningChambers.Contains(chamberManager.rangerChosenChamber))
            {
             //   currentSetPoint += 3 * point;
            }
            else
            {
                //Nothing
            }
        }
        else if (playerChosenChamber == chamberManager.rangerChosenChamber)
        {
            if (winningChambers.Contains(chamberManager.rangerChosenChamber))
            {
               // currentSetPoint -= point;
            }

            else
            {
                //Nothing
            }
        }
      //  PlayerPrefs.SetInt("PlayerPoint", currentSetPoint);
       // pointFill.fillAmount = (currentSetPoint / setAndRoundManager.pointsQuota);
       // pointText.text = currentSetPoint.ToString();
        setAndRoundManager.EndRound();
    }
}
