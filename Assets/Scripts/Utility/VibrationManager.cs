using MoreMountains.NiceVibrations;
using UnityEngine;

public class VibrationManager : MonoBehaviour
{
    public static VibrationManager instance;

    public float DelayBetweenVibration = 100f;
    private float LastVibrationStartedAt = -1;
    private bool VibrationAllowed = true;

    //private AudioManager _audioManager;
    
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        
        DontDestroyOnLoad(this);
        
        // int defaultValue = 1;
// #if UNITY_ANDROID
//         defaultValue = 0;
// #elif UNITY_IOS
//             defaultValue = 1;
// #endif
    }

    private void Start()
    {
        //gameManager = GameManager.GetInstance();
        // TO DO: Get the current state of the settings and set the toggle accordingly
        // VibrationAllowed = gameManager.GetCanPlayHaptic();
        //_audioManager = AudioManager.GetInstance();
    }

    public void ToggleVibration(int value)
    {
        VibrationAllowed = value == 0 ? false : true;
        // set global settings Vibration/Haptic value here ...
    }
    
    public void ToggleVibration(bool value)
    {
        VibrationAllowed = value;
        //TO DO: Save the value in the Vibration data
        // gameManager.SetCanPlayHaptic(value);
        //try
        //{
        //    _audioManager.PlaySFX(Sound.ButtonClick);
        //}
        //catch
        //{
        //    _audioManager = AudioManager.GetInstance();
        //    _audioManager.PlaySFX(Sound.ButtonClick);
        //}
        
        // set global settings Vibration/Haptic value here ...
    }

    public void SetVibration(bool isVibrationOn)
    {
        VibrationAllowed = isVibrationOn;
    }

    public bool IsVibrationOn()
    {
        return VibrationAllowed;
    }

    public void PlayHapticLight()
    {
        if(!VibrationAllowed) return;
        if (LastVibrationStartedAt == -1) LastVibrationStartedAt = Time.time * 1000;
        else
        {
            float diff = Time.time * 1000 - LastVibrationStartedAt;
            if(diff <= DelayBetweenVibration) return;
        }
        MMVibrationManager.Haptic(HapticTypes.LightImpact);
        LastVibrationStartedAt = Time.time * 1000;
        
    }
    
    /// <summary>
    /// CAUTION : Use only if will be called once, not frequently...
    /// </summary>
    public void PlayHapticLightForced()
    {
        if(!VibrationAllowed) return;
        MMVibrationManager.Haptic(HapticTypes.LightImpact);
        LastVibrationStartedAt = Time.time * 1000;
    }
    
    public void PlayHapticMedium()
    {
        if(!VibrationAllowed) return;
        if (LastVibrationStartedAt == -1) LastVibrationStartedAt = Time.time * 1000;
        else
        {
            float diff = Time.time * 1000 - LastVibrationStartedAt;
            if(diff <= DelayBetweenVibration) return;
        }
        MMVibrationManager.Haptic(HapticTypes.MediumImpact);
        LastVibrationStartedAt = Time.time * 1000;
    }
    
    /// <summary>
    /// CAUTION : Use only if will be called once, not frequently...
    /// </summary>
    public void PlayHapticMediumForced()
    {
        if(!VibrationAllowed) return;
        MMVibrationManager.Haptic(HapticTypes.MediumImpact);
        LastVibrationStartedAt = Time.time * 1000;
    }
    
    public void PlayHapticHeavy()
    {
        if(!VibrationAllowed) return;
        if (LastVibrationStartedAt == -1) LastVibrationStartedAt = Time.time * 1000;
        else
        {
            float diff = Time.time * 1000 - LastVibrationStartedAt;
            if(diff <= DelayBetweenVibration) return;
        }
        MMVibrationManager.Haptic(HapticTypes.HeavyImpact);
        LastVibrationStartedAt = Time.time * 1000;
    }
    
    /// <summary>
    /// CAUTION : Use only if will be called once, not frequently...
    /// </summary>
    public void PlayHapticHeavyForced()
    {
        if(!VibrationAllowed) return;
        MMVibrationManager.Haptic(HapticTypes.HeavyImpact);
        LastVibrationStartedAt = Time.time * 1000;
    }
}