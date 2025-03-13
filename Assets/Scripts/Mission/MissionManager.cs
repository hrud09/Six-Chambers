using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Coffee.UIEffects;
using Random = UnityEngine.Random;
using System;

public class MissionManager : MonoBehaviour
{
    [Header("Main Missions:")]
    [Space]
    [SerializeField] private List<Mission> mainMissions;
    [SerializeField] internal Button missionButton;
    [SerializeField] internal Button compassButton;
    [SerializeField] internal TMP_Text missionText;
    [SerializeField] internal TMP_Text missionProgressText;
    [SerializeField] internal Slider progressSlider;
    [SerializeField] internal Sprite missionActiveImg;
    [SerializeField] internal Sprite missionEndImg;
    [SerializeField] internal GameObject arrow;
    [SerializeField] internal Image missionIconBg;
    [SerializeField] internal Sprite missionActiveIconBg;
    [SerializeField] internal Sprite missionDoneIconBg;
    [SerializeField] internal UIShiny uiShiny;
    [SerializeField] internal ParticleSystem missionEndParticle;
    [SerializeField] internal SpriteRenderer handSpriteRenderer;


    public static MissionManager instance;
    public const string currentFinishedMissionStr = "Current Mission";
    public const string currentDailyMissionStreak = "Daily Mission Steak";
    private int currentMission = 0;
  
    public Mission CetCurrentMissionRef() => mainMissions[currentMission];
    public static event Action<int> MissionStartedAction;
    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        currentMission = PlayerPrefs.GetInt(currentFinishedMissionStr, 0);

        if (currentMission >= mainMissions.Count)
        {
            missionButton.gameObject.SetActive(false);
        }

        CheckForNextMission();
    }

    #region Main Missions

    public void CheckForNextMission()
    {
        if (currentMission >= mainMissions.Count)
        {
            compassButton.gameObject.SetActive(false);
            return;
        }

        ShowMission(currentMission);
    }

    public void ShowMission(int idx)
    {
        mainMissions[idx].OnMissionStart();
        MissionStartedAction?.Invoke(currentMission);
    }

    public void EndMission()
    {
        currentMission++;
        PlayerPrefs.SetInt(currentFinishedMissionStr, currentMission);

        if (currentMission >= mainMissions.Count)
        {
            missionButton.gameObject.SetActive(false);
            return;
        }
        CheckForNextMission();
    }

    internal void GiveReward(int amount)
    {
        //RewardsManager.instance.GiveReward(amount, missionButton.GetComponent<RectTransform>().localPosition);
    }

    public bool IsMissionEnded(int idx) => mainMissions[idx].IsMissionEnded();

    public void SetButtonShine(bool val)
    {
        if (!val)
        {
            uiShiny.Stop();
        }

        else
        {
            uiShiny.Play();
        }
    }

    public void HideMissionButton()
    {
        missionButton.transform.DOScale(0f, 0.5f);

    }

    public void ShowMissionButton()
    {
       // SoundManager.instance.PlaySound(10);

        missionButton.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBounce);
    }

    public void ShowParticle()
    {
        missionEndParticle.Play();
    }

    public int GetMissionProgress(int id)
    {
        return mainMissions[id].GetMissionProgress();
    }

    public int GetCurrentMissionId()
    {
        return currentMission;
    }

    public Mission GetCurrentMission()
    {
        for (int i = 0; i < mainMissions.Count; i++)
        {
            if (mainMissions[i].GetMissionId() == currentMission)
            {
                return mainMissions[i];
            }
        }

        return null;
    }

    #endregion
}


public enum ProgressType
{
    Collection,
    Drop,
    Unlock,
    Upgrade,
    Activate,
    Deliver,
    Fill,
    Maintenance,
    Sell
}
