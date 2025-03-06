using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    [Header("AudioSources")]
    public AudioSource bgmSource;
    public GameObject sfxSourcePrefab;
    private AudioSource sfxSource;


    private float DefaultAudioVolumeMin = 0.7f;
    private float DefaultAudioVolumeMax = 0.7f;


    //Bools
    private bool hearAudibleSFX;
    private bool hearAudibleBGM;

    private static AudioManager Instance;

    [Header("AudioClip")]
    public AudioClip BGM;
    [Range(0f, 0.1f)]
    public float bgmVolume = 0.7f;
    [Space]
    [SerializeField] private SoundAudioClipPair[] soundAudioClipPairs;

    private static readonly Dictionary<Sound, SoundAudioClipPair> SoundToClip = new Dictionary<Sound, SoundAudioClipPair>();
    private AudioSource audioSource;

    private bool isStrikerMoving = false;
    private int objectHitCountForHitSfx = 0;
    private int objectHitCountForStrikeSfx = 0;
    private bool canPlayHitSfx = true;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void Init()
    {
        PlayBGM();

        PreInstantiateAudioSourcePrefab();

        foreach (var pair in soundAudioClipPairs)
        {
            if (SoundToClip.ContainsKey(pair.sound))
            {
                SoundToClip[pair.sound] = pair;
                continue;
            }

            SoundToClip.Add(pair.sound, pair);
        }
    }


    public static AudioManager CallPlaySFX(Sound sound)
    {
        Instance.PlaySFX(sound);
        return Instance;
    }

    public AudioManager PlaySFX(Sound sound)
    {
        TriggerSFX(sound);
        return this;
    }

    public AudioManager SetSound(float volumeMin, float volumeMax)
    {
        if (sfxSource == null)
            return this;
        sfxSource.volume = GetSFXVolume(volumeMin, volumeMax);
        return this;
    }


    private void TriggerSFX(Sound sound)
    {
        if (!hearAudibleSFX)
            return;
        if (SoundToClip.ContainsKey(sound))
        {
            try
            {
                sfxSource = ObjectPool.instance.GetObject(sfxSourcePrefab, true, default, default, this.transform).GetComponent<AudioSource>();
            }
            catch
            {
                // BbsLog.LogError("Error Set");
            }

            try
            {
                // sfxSource.volume = 0;
                // sfxSource.Stop();
                // sfxSource.gameObject.SetActive(true);
                sfxSource.volume = GetSFXVolume(SoundToClip[sound].volumeRange, SoundToClip[sound].volumeRange);
                sfxSource.PlayOneShot(SoundToClip[sound].audioClip);
                StartCoroutine(HideSFXSourceAfterPlayback(SoundToClip[sound].audioClip, sfxSource.gameObject));
            }
            catch
            {
                // BbsLog.LogError("Source not playing");
            }
        }

    }

    IEnumerator HideSFXSourceAfterPlayback(AudioClip clip, GameObject sourceSFX)
    {
        yield return new WaitForSeconds(clip.length);
        ObjectPool.instance.ReturnToPool(sourceSFX);
    }

    private void PlayBGM()
    {
        if (!hearAudibleBGM)
            return;

        if (!bgmSource.isPlaying)
        {
            bgmSource.clip = BGM;
            bgmSource.volume = GetBGMVolume();
            bgmSource.Play();
        }
    }

    #region Internal Methods

    private float GetSFXVolume(float _value1, float _value2)
    {
        // if (!hearAudibleSFX)
        //     return 0.0f;
        float level;
        level = Random.Range(_value1, _value2);
        // level = 0;
        return level;
    }

    private float GetBGMVolume()
    {
        // if (!hearAudibleBGM)
        //     return 0.0f;
        return bgmVolume;
    }

    #endregion

    #region Setters

    public void SetSFXPermission(bool canPlaySFX)
    {
        SetSFXPermissionValue(canPlaySFX);
        if (!canPlaySFX)
        {
            sfxSource.Stop();
            sfxSource.volume = 0.0f;
        }
        TriggerSFX(Sound.ButtonClick);
    }

    public void SetSFXPermissionValue(bool canPlaySFX)
    {
        hearAudibleSFX = canPlaySFX;
        //TO DO: Save Can Play Audio SFX
    }

    public void SetBGMPermission(bool canPlayBGM)
    {
        SetBGMPermissionValue(canPlayBGM);
        TriggerSFX(Sound.ButtonClick);
    }

    public void SetBGMPermissionValue(bool canPlayBGM)
    {
        hearAudibleBGM = canPlayBGM;
        // TO DO: Save Can Play Audio BGM

        if (!canPlayBGM)
        {
            bgmSource.Stop();
            bgmSource.volume = 0.0f;
        }
        else
        {
            PlayBGM();
        }
    }

    #endregion

    private void PreInstantiateAudioSourcePrefab()
    {
        for (int i = 0; i < 9; i++)
        {
            GameObject obj = ObjectPool.instance.GetObject(sfxSourcePrefab, false, default, default, this.transform);
            obj.transform.parent = this.transform;
        }

        sfxSource = ObjectPool.instance.GetObject(sfxSourcePrefab, true, default, default, this.transform).GetComponent<AudioSource>();
    }

    public static AudioManager GetInstance()
    {
        return Instance;
    }


    [Serializable]
    private class SoundAudioClipPair
    {
        public Sound sound;
        public AudioClip audioClip;
        [Range(0, 1)]
        public float volumeRange = 0.7f;
    }

    #region Gameplay Audio Control

    #endregion
}
