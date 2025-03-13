using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;



public class Mission : MonoBehaviour
{
    [SerializeField] protected int id;
    [SerializeField] protected string missionString;
    [SerializeField] protected int progressMaxValue;
    [SerializeField] protected int rewardAmount;
    [SerializeField] protected int connectedBuildingId;
    [SerializeField] protected List<int> dependendBuildingIds;
    protected int currentProgress = 0;
    protected const string progressStr = "Current Progress ";
    protected const string endCheckStr = "Is Ended ";

    protected bool isFocusing = false;
    protected bool canShowHand = false;

    protected Sequence arrowSeq;
    protected Sequence handSeq;

    protected float elapsedTimeMissionEnd = 0;
    protected float handShowWaitTime = 4f;

    private WaitForSeconds waitTime = new WaitForSeconds(0.75f);
    public delegate void MissionEndDeleagte();
    public MissionEndDeleagte CallMissionEnd;
    public delegate void OnMissionButtonEndClickDelegate();
    public OnMissionButtonEndClickDelegate CallMissionEndButtonClick;
    protected virtual void OnEnable()
    {
        CallMissionEnd += OnMissionEnd;
        CallMissionEndButtonClick += OnMissionButtonEndClick;
    }
    protected virtual void OnDisable()
    {
        CallMissionEnd -= OnMissionEnd;
        CallMissionEndButtonClick -= OnMissionButtonEndClick;
    }

    public virtual void OnMissionStart()
    {
        isFocusing = false;

        StopCoroutine(OnMissionButtonShowed());

        if (PlayerPrefs.GetInt(endCheckStr + id, 0) == 1)
        {
            MissionManager.instance.ShowMissionButton();
            OnMissionEnd();
        }

        else
        {
            ShowArrow();
            currentProgress = PlayerPrefs.GetInt(progressStr + id, 0);
            UpdateText();
            MissionManager.instance.SetButtonShine(false);
            MissionManager.instance.ShowMissionButton();
            //MissionManager.instance.missionButton?.onClick.RemoveListener(OnMissionButtonEndClick);
            MissionManager.instance.missionButton?.onClick.AddListener(OnMissionButtonHighlighterClick);
            MissionManager.instance.compassButton?.onClick.AddListener(OnMissionButtonHighlighterClick);
        }
    }

    protected virtual IEnumerator OnMissionButtonShowed()
    {
        yield return null;
    }

    protected virtual void ShowArrow()
    {
        MissionManager.instance.arrow.SetActive(true);

        arrowSeq = DOTween.Sequence();

    }

    protected void PositionArrow(Vector3 pos, float yPosMin, float yPosMax)
    {
        Transform arrow = MissionManager.instance.arrow.transform;
        arrow.position = pos;

        yPosMax += 1.5f;

        arrowSeq
        .Append(arrow.DOMoveY(yPosMax, 0.5f).SetEase(Ease.InOutSine))
        .AppendInterval(0.025f)
        .Append(arrow.DOMoveY(yPosMin, 0.5f).SetEase(Ease.InOutSine))
        .AppendInterval(0.025f)
        .SetLoops(-1);
    }

    protected void HideArrow()
    {
        arrowSeq.Kill();
        MissionManager.instance.arrow.SetActive(false);
    }

    protected virtual void FocusOnCurrentMissionArea(Transform area)
    {
        //        Debug.Log(CameraFollow.instance.IsCameraFocusingOnObject());
        if (isFocusing) return;
        // if (CameraFollow.instance.IsCameraFocusingOnObject()) return;
        isFocusing = true;

        // CameraFollow.instance.FocusOnObject(area, 1.5f, OnMissionFocusStart, OnMissionFocusEnd, false);
    }

    protected virtual void FocusOnMultipleObjects(List<Transform> areas)
    {
        if (isFocusing) return;
        // if (CameraFollow.instance.IsCameraFocusingOnObject()) return;
        isFocusing = true;
        List<Vector3> areapos = new();
        foreach (var item in areas)
        {
            areapos.Add(item.position);
        }
        // CameraFollow.instance.FocusOnObjectsVectors(areapos, 1.5f, OnMissionFocusEnd);
    }


    protected virtual void OnMissionFocusStart()
    {
    }

    protected virtual void OnMissionFocusEnd()
    {
        isFocusing = false;
        StartCoroutine(CheckIfMissionAlreadyDone());
    }

    protected virtual IEnumerator CheckIfMissionAlreadyDone()
    {
        yield return null;
    }


    protected virtual void OnMissionButtonHighlighterClick()
    {
        // SoundManager.instance.PlaySound(4);
        //focus on stuff
    }

    // public virtual void OnMissionProgress(ProgressType progressType, ITEM_TYPE itemType, Building building)
    // {

    // }

    // public virtual void OnMissionProgress(ProgressType progressType, ITEM_TYPE itemType, Building building, bool isDropperByPlayer)
    // {

    // }

    // public virtual void OnMissionProgress(ProgressType progressType, ITEM_TYPE itemType, Building building, int characterStatType)
    // {
    //     //For Player:
    //     //characterStatType:
    //     //0 = capacity
    //     //1 = speed

    //     //For NPC:
    //     //characterStatType:
    //     //3 = hire
    //     //4 = capacity
    //     //5 = speed
    // }

    protected virtual void OnMissionButtonEndClick()
    {
        //  if (CameraFollow.instance.IsCameraFocusingOnObject()) return;
        currentProgress = 0;

        // SoundManager.instance.PlaySound(11);
        VibrationManager.instance.PlayHapticMedium();

        //MissionManager.instance.missionButton?.onClick.RemoveListener(OnMissionButtonEndClick);
        MissionManager.instance.SetButtonShine(false);
        MissionManager.instance.ShowParticle();

        MissionManager.instance.HideMissionButton();

        // GameAnalytics.NewProgressionEvent(progressionStatus: GAProgressionStatus.Complete, $"Mission_id_{id}", id);

        //canShowHand = false;

        if (rewardAmount != 0)
        {
            MissionManager.instance.GiveReward(rewardAmount);
        }

        int required = rewardAmount / 20;
        int left = rewardAmount % 20;

        StartCoroutine(MissionEndSequence((rewardAmount == 0) ? 1f : (required + left) * 0.1f + 0.75f));
    }

    protected IEnumerator MissionEndSequence(float initDelay)
    {
        if (handSeq != null)
        {
            handSeq.Kill();

            MissionManager.instance.handSpriteRenderer.DOFade(0, 0.01f);
        }

        yield return new WaitForSeconds(initDelay);

        // if (CameraFollow.instance.IsCameraFocusingOnObject())
        // {
        //     while (CameraFollow.instance.IsCameraFocusingOnObject())
        //     {
        //         yield return null;
        //     }
        // }


        if (dependendBuildingIds.Count > 0)
        {
            //OnDependentBuildingFocusNeeded();

            yield return new WaitForSeconds(0.1f);
        }

        MissionManager.instance.EndMission();
    }

    // internal virtual void OnDependentBuildingFocusNeeded()
    // {
    //     CameraFollow.instance.FocusOnObjects(dependendBuildingIds, 1.5f, null);
    // }


    protected virtual void UpdateText()
    {
        MissionManager.instance.missionText.text = missionString;
        MissionManager.instance.missionButton.image.sprite = MissionManager.instance.missionActiveImg;
        MissionManager.instance.missionIconBg.sprite = MissionManager.instance.missionActiveIconBg;
        MissionManager.instance.progressSlider.maxValue = progressMaxValue;
        MissionManager.instance.progressSlider.value = currentProgress;
        MissionManager.instance.missionProgressText.text = string.Format("{0} / {1}", currentProgress, progressMaxValue);
    }

    protected virtual void OnMissionEnd()
    {
        MissionManager.instance.missionButton?.onClick.RemoveListener(OnMissionButtonHighlighterClick);
        MissionManager.instance.compassButton?.onClick.RemoveListener(OnMissionButtonHighlighterClick);

        //MissionManager.instance.missionButton?.onClick.AddListener(OnMissionButtonEndClick);

        MissionManager.instance.missionButton.image.sprite = MissionManager.instance.missionEndImg;
        MissionManager.instance.missionIconBg.sprite = MissionManager.instance.missionDoneIconBg;
        MissionManager.instance.missionText.text = "Mission Complete";
        MissionManager.instance.missionProgressText.text = string.Format("{0} / {1}", progressMaxValue, progressMaxValue);
        MissionManager.instance.progressSlider.maxValue = progressMaxValue;
        MissionManager.instance.progressSlider.value = progressMaxValue;
        MissionManager.instance.SetButtonShine(true);
        PlayerPrefs.SetInt(endCheckStr + id, 1);
        HideArrow();
        StartCoroutine(AutoComplete());
        //canShowHand = true;
    }
    
    private IEnumerator AutoComplete()
    {
        yield return waitTime;
        OnMissionButtonEndClick();
    }

    void Update()
    {
        if (!canShowHand) return;

        elapsedTimeMissionEnd += Time.deltaTime;

        if (elapsedTimeMissionEnd >= handShowWaitTime)
        {
            canShowHand = false;
            elapsedTimeMissionEnd = 0;
            ShowHand();
        }
    }
    
    protected void ShowHand()
    {
        handSeq = DOTween.Sequence();

        //Image hand = MissionManager.instance.hand;

        //handSeq
        //.Append(hand.DOFade(1, 0.25f))
        //.AppendInterval(0.15f)
        //.Append(hand.transform.DOScale(0.75f, 0.25f))
        //.AppendInterval(0.15f)
        //.Append(hand.transform.DOScale(1, 0.25f))
        //.AppendInterval(0.15f)
        //.Append(hand.transform.DOScale(0.75f, 0.25f))
        //.AppendInterval(0.15f)
        //.Append(hand.transform.DOScale(1, 0.25f))
        //.AppendInterval(0.15f)
        //.Append(hand.transform.DOScale(0.75f, 0.25f))
        //.AppendInterval(0.15f)
        //.Append(hand.transform.DOScale(1, 0.25f))
        //.AppendInterval(0.15f)
        //.Append(hand.DOFade(0, 0.25f))
        //.AppendInterval(Random.Range(1f, 2f))
        //.SetLoops(-1);

        var handAnimation = MissionManager.instance.handSpriteRenderer;
        handSeq
        .Append(handAnimation.DOFade(1, 0.25f))
        .AppendInterval(2f)
        .Append(handAnimation.DOFade(0, 0.25f))
        .AppendInterval(Random.Range(1f, 2f))
        .SetLoops(-1);
    }

    public bool IsMissionEnded()
    {
        return PlayerPrefs.GetInt(endCheckStr + id, 0) == 1;
    }

    public int GetMissionProgress()
    {
        return PlayerPrefs.GetInt(progressStr + id, 0);
    }

    public int GetMissionId()
    {
        return id;
    }




}
