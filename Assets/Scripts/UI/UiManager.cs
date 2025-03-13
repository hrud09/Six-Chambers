using System.Collections.Generic;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    private static UiManager instance;

    [Space]
    [Header("Reward Icons")]
    [SerializeField] private Sprite coinIcon;
    [SerializeField] private Sprite extraPin, extraCardSlot, hammer;

    [Space]
    public Sprite rewardCoinPile;

    private Stack<IOverlayUI> overlayUI = new Stack<IOverlayUI>();

    private UiController uiController;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public static UiManager GetInstance()
    {
        return instance;
    }

    #region Overlay Control
    public void OnOverlayUiEnabled(IOverlayUI item)
    {
        overlayUI.Push(item);
    }

    public void OnOverlayUiDisabled(IOverlayUI item)
    {
        try
        {
            IOverlayUI ui = overlayUI.Peek();
            if (ui == item)
            {
                overlayUI.Pop();
            }
            else
            {
                BbsLog.LogError("Disabled Overlay UI ref misMatched!");
            }
        }
        catch
        {
            BbsLog.LogError("No Overlay UI to Pop!");
        }
    }

    public bool IsOverlayUiActive()
    {
        return overlayUI.Count > 0;
    }

    #endregion

    public UiController GetUiController()
    {
        return uiController;
    }

    public void AttachUIController(UiController controller)
    {
        uiController = controller;
    }

    public void UpdateTopHudCoins()
    {
        
    }

    public Sprite GetFlyObjectIcon(FlyObjectType rewardType)
    {
        //TODO: Update icons and types according to game
        Sprite returnSprite = null;
        switch (rewardType)
        {
            case FlyObjectType.Coins:
                returnSprite = coinIcon;
                break;
            case FlyObjectType.ExtraPin:
                returnSprite = extraPin;
                break;
            case FlyObjectType.ExtraCardSlot:
                returnSprite = extraCardSlot;
                break;
            case FlyObjectType.Hammer:
                returnSprite = hammer;
                break;
        }
        return returnSprite;
    }
}
