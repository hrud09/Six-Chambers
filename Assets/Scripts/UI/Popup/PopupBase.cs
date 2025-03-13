using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;

public class PopupBase : MonoBehaviour, IOverlayUI
{
    [SerializeField] private Image darkBg;
    [SerializeField] private RectTransform bg;
    [SerializeField] private CanvasGroup canvasGroup;

    public void OnUIDisabled()
    {
        UiManager.GetInstance().OnOverlayUiDisabled(this);
    }

    public void OnUIEnabled()
    {
        UiManager.GetInstance().OnOverlayUiEnabled(this);
    }

    public void SetView(bool flag, float darkBgTransitionTime = 0.2f, float bgScaleTime = 0.3f, float darkBgOpacity = 1f)
    {
        if (flag)
        {
            OnUIEnabled();
            SetCanvasGroup(false);
            if (darkBg == null)
            {
                bg.localScale = Vector3.zero;
                SetCanvasGroup(true);
                bg.DOScale(Vector3.one, bgScaleTime).SetUpdate(true).SetEase(Ease.OutQuint).OnComplete(() =>
                {
                    OnPopupEnabled();
                });
            }
            else
            {
                darkBg.color = new Color(darkBg.color.r, darkBg.color.g, darkBg.color.b, 0f);
                bg.localScale = Vector3.zero;
                SetCanvasGroup(true);
                darkBg.DOFade(darkBgOpacity, darkBgTransitionTime).SetUpdate(true).OnComplete(() =>
                {
                    bg.DOScale(Vector3.one, bgScaleTime).SetUpdate(true).SetEase(Ease.OutQuint).OnComplete(() =>
                    {
                        OnPopupEnabled();
                    });
                });
            }
        }
        else
        {
            OnUIDisabled();
            bg.DOScale(Vector3.zero, bgScaleTime).SetUpdate(true).SetEase(Ease.InQuint).OnComplete(() =>
            {
                if (darkBg == null)
                {
                    OnPopupDisabled();
                    SetCanvasGroup(false);
                }
                else
                {
                    darkBg.DOFade(0f, darkBgTransitionTime).SetUpdate(true).OnComplete(() =>
                    {
                        OnPopupDisabled();
                        SetCanvasGroup(false);
                    });
                }
            });
        }
    }

    private void SetCanvasGroup(bool flag)
    {
        canvasGroup.alpha = flag ? 1 : 0;
        canvasGroup.interactable = flag;
        canvasGroup.blocksRaycasts = flag;
    }

    protected virtual void OnPopupEnabled()
    { }

    protected virtual void OnPopupDisabled()
    { }
}
