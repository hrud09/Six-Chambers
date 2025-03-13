using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SettingsPopup : PopupBase
{
    [Space]
    public Sprite toggleOnSprite;
    public Sprite toggleOffSprite;

    [SerializeField]
    private Slider soundSlider, musicSlider, hepticSlider;
    [SerializeField]
    private Image soundImg, musicImg, hepticImg;

    private AudioManager audioManager;
    private VibrationManager vibrationManager;

    private void Start()
    {
        PreferenceData preferenceData = GameManager.GetInstance().GetPreferenceData();

        bool isBgmOn = preferenceData.isBgmOn;
        bool isSfxOn = preferenceData.isSfxOn;
        bool isVibrationOn = preferenceData.isVibrationOn;

        soundImg.sprite = isSfxOn == true ? toggleOnSprite : toggleOffSprite;
        soundSlider.value = isSfxOn == true ? 1 : 0;

        musicImg.sprite = isBgmOn == true ? toggleOnSprite : toggleOffSprite;
        musicSlider.value = isBgmOn == true ? 1 : 0;

        hepticImg.sprite = isVibrationOn == true ? toggleOnSprite : toggleOffSprite;
        hepticSlider.value = isVibrationOn == true ? 1 : 0;
    }

    public void DisableView()
    {
        AudioManager.CallPlaySFX(Sound.ButtonClick);
        SetView(false);
    }

    public void SoundButtonPressed()
    {
        GameManager gameManager = GameManager.GetInstance();
        PreferenceData preferenceData = gameManager.GetPreferenceData();
        preferenceData.isSfxOn = !preferenceData.isSfxOn;
        gameManager.UpdatePreferenceData(preferenceData);

        UpdateSoundView(preferenceData.isSfxOn);
    }

    public void MusicButtonPressed()
    {
        GameManager gameManager = GameManager.GetInstance();
        PreferenceData preferenceData = gameManager.GetPreferenceData();
        preferenceData.isBgmOn = !preferenceData.isBgmOn;
        gameManager.UpdatePreferenceData(preferenceData);

        UpdateMusicView(preferenceData.isBgmOn);
    }

    public void HepticButtonPressed()
    {
        GameManager gameManager = GameManager.GetInstance();
        PreferenceData preferenceData = gameManager.GetPreferenceData();
        preferenceData.isVibrationOn = !preferenceData.isVibrationOn;
        gameManager.UpdatePreferenceData(preferenceData);

        UpdateHepticView(preferenceData.isVibrationOn);
    }

    protected override void OnPopupDisabled()
    {
        base.OnPopupDisabled();
    }

    protected override void OnPopupEnabled()
    {
        base.OnPopupEnabled();
    }


    public void CloseButtonPressed()
    {
        AudioManager.CallPlaySFX(Sound.ButtonClick);
        SetView(false);
    }

    private void UpdateSoundView(bool flag)
    {
        soundImg.sprite = flag == true ? toggleOnSprite : toggleOffSprite;
        DOTween.To(() => soundSlider.value, x => soundSlider.value = x, flag == true ? 1 : 0, 0.1f).SetEase(Ease.InQuint);
        GetAudioManager().SetSFXPermission(flag);
    }

    private void UpdateMusicView(bool flag)
    {
        musicImg.sprite = flag == true ? toggleOnSprite : toggleOffSprite;
        DOTween.To(() => musicSlider.value, x => musicSlider.value = x, flag == true ? 1 : 0, 0.1f).SetEase(Ease.InQuint);
        GetAudioManager().SetBGMPermission(flag);
    }

    private void UpdateHepticView(bool flag)
    {
        hepticImg.sprite = flag == true ? toggleOnSprite : toggleOffSprite;
        DOTween.To(() => hepticSlider.value, x => hepticSlider.value = x, flag == true ? 1 : 0, 0.1f).SetEase(Ease.InQuint);
        GetVibrationManager().ToggleVibration(flag);
    }

    private AudioManager GetAudioManager()
    {
        if (audioManager == null)
            audioManager = AudioManager.GetInstance();
        return audioManager;
    }

    private VibrationManager GetVibrationManager()
    {
        if (vibrationManager == null)
            vibrationManager = VibrationManager.instance;
        return vibrationManager;
    }

    public void OnRestartClicked()
    {
        AudioManager.CallPlaySFX(Sound.ButtonClick);
        SetView(false);

        TransitionManager.GetInstance().ChangeLevel(SCENE_NUM.GAME_SCENE);

        // TransitionManager.GetInstance().FadeOut(() =>
        // {
        //     GameplayController.GetInstance().OnRestartClicked();
        //     TransitionManager.GetInstance().FadeIn(null);
        // });
    }

    public void OnResumeClicked()
    {
        AudioManager.CallPlaySFX(Sound.ButtonClick);
        SetView(false);
    }

    public void OnMainMenuClicked()
    {
        AudioManager.CallPlaySFX(Sound.ButtonClick);
        SetView(false);
        TransitionManager.GetInstance().ChangeLevel(SCENE_NUM.MAIN_MENU);
    }

    public void OnSkipClicked()
    {
        AudioManager.CallPlaySFX(Sound.ButtonClick);
        SetView(false);
        //GameplayController.GetInstance().OnLevelWin();
    }
}
