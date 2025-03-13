using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class ToastMessageManager : MonoBehaviour
{
    private static ToastMessageManager instance;

    [SerializeField]
    private RectTransform holderRect;
    [SerializeField]
    private TMP_Text toastMessageText;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static ToastMessageManager GetInstance()
    {
        return instance;
    }

    public void ShowToastMessage(string message, float duration)
    {
        DOTween.Kill("toast");
        toastMessageText.text = message;
        holderRect.gameObject.SetActive(true);
        holderRect.anchoredPosition3D = Vector3.up * -450;
        holderRect.DOAnchorPos3DY(0, 0.2f).SetId("toast").OnComplete(()=> {
            CancelInvoke();
            Invoke("AutoDisable", duration);
        });
    }

    private void AutoDisable()
    {
        holderRect.gameObject.SetActive(false);
    }
}
