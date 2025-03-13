using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public int id;
    [SerializeField] protected GameObject tutorialObject;
    [SerializeField] protected Canvas text;


    public virtual void OnTutorialStart()
    {
        // text.enabled = false;
        tutorialObject.SetActive(true);
    }

    public virtual void OnTutorialEnd()
    {
        // text.enabled = true;
        tutorialObject.SetActive(false);
    }
}
