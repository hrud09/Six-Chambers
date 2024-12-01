using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
public class ChamberUIScript : MonoBehaviour
{
    public Chamber chamber;

    public GameObject playerSelectionVisualUI;
    public GameObject rangerSelectionVisualUI;
    public GameObject winningHandVisualUI;


    public TMP_Text chipsCountText;
    public GameObject changeAmountParent;
    public TMP_Text chipChangeAmountText;

    [Header("Rank Visualize")]
    public CanvasGroup rankUICanvasGroup;
    public Image rankTextBG;

    public TMP_Text rankText;

    public Color[] availablityColors;
    public Image innerBG;
   
}
