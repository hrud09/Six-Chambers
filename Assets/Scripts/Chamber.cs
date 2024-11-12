using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class Chamber : MonoBehaviour
{
    public ChamberManager chamberManager;
    public List<Card> chamberCards;
    public int index;

    public Transform cardParent;
    public Transform[] bestCardsHolders;

    [SerializeField]
    private Vector3[] originalPositions;
    private List<Tween> cardTweens = new List<Tween>();
    public GameObject layerSelectionAura;
    public GameObject topRankUI;


    [Header("Winning Text")]
    public TMP_Text rankText;


    [Header("Chips Corner")]
    public Transform chipsParent;
    public List<GameObject> existingChips;
   // public GameObject existingChipsCountObject;
    public TMP_Text existingChipsCountText;

    public List<GameObject> wagerredChips;
    public Transform wagerredChipsParent;

    public int chipsCount;
    private void Awake()
    {
        chamberManager = GetComponentInParent<ChamberManager>();
        chipsCount = PlayerPrefs.GetInt($"{index}_ChamberChipsCount", 2);
        for (int i = 0; i < chipsCount; i++)
        {
            GameObject _chip = Instantiate(chamberManager.chipPrefab, chipsParent);
            _chip.transform.localPosition = Vector3.back * 5;
            _chip.transform.localScale = Vector3.zero;
            _chip.transform.DOScale(Vector3.one, 0.3f).OnComplete(() =>
            {


                _chip.transform.DOLocalJump(Vector3.up * i * 0.2f, 2, 1, 0.5f);

            });
            existingChips.Add(_chip);
            /* _chip.transform.position = chamber.chipsParent*/

        }
    }
    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        UpdateChipsCount();
    }
    public void InitializeOriginalPositions()
    {
        layerSelectionAura = chamberCards[0].playerSelectionAura;
        originalPositions = new Vector3[chamberCards.Count];
        for (int i = 0; i < chamberCards.Count; i++)
        {
            if (chamberCards[i] != null)
            {
                originalPositions[i] = chamberCards[i].transform.position;
            }
        }
    }

    private void OnMouseDown()
    {
        if (chamberManager.playerHandManager.playerChosenChamber == null && chamberManager.playerHandManager.playersTurn)
        {
           // layerSelectionAura.SetActive(false);
            chamberManager.playerHandManager.SelectPlayerChamber(this);
        }
    }

    private void OnMouseOver()
    {
       // existingChipsCountObject.SetActive(true);
        if (!chamberManager.playerHandManager.chamberSelected && chamberManager.playerHandManager.playersTurn && !chamberManager.playerHandManager.mouseOverChambers)
        {
            layerSelectionAura.SetActive(true);
            chamberManager.playerHandManager.mouseOverChambers = true;
            LiftCards();
        }
    }

    private void OnMouseExit()
    {
       // existingChipsCountObject.SetActive(false);
        if(chamberManager.playerHandManager.chamberSelected != this) layerSelectionAura.SetActive(false);
        chamberManager.playerHandManager.mouseOverChambers = false;
        LowerCards();
    }

    private void LiftCards()
    {
        KillCardTweens();
        for (int i = 0; i < chamberCards.Count; i++)
        {
            if (i < originalPositions.Length && chamberCards[i] != null)
            {
                cardTweens.Add(chamberCards[i].transform.DOMoveY(originalPositions[i].y + 0.2f, 0.2f));
            }
        }
    }

    private void LowerCards()
    {
        KillCardTweens();
        for (int i = 0; i < chamberCards.Count; i++)
        {
            if (i < originalPositions.Length && chamberCards[i] != null)
            {
                cardTweens.Add(chamberCards[i].transform.DOMoveY(originalPositions[i].y, 0.1f));
            }
        }
    }

    private void KillCardTweens()
    {
        foreach (var tween in cardTweens)
        {
            tween.Kill();
        }
        cardTweens.Clear();
    }

    public void UpdateChipsCount()
    {
        PlayerPrefs.SetInt($"{index}_ChamberChipsCount", existingChips.Count);
        existingChipsCountText.text = existingChips.Count.ToString();
    }
}
