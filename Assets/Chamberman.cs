using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum AnimationType { 

    Idle1,
    Idle2,
    Idle3,
    Thinking,
    Angry,
    Scared

}

[System.Serializable]
public class AnimationTriggers {

    public AnimationType animationType;
    public string triggerName;
}
public class Chamberman : MonoBehaviour
{
    public List<AnimationTriggers> triggers;
    public Animator chrAnimator;


    public void TriggerAnimation(AnimationType animationType)
    {
        chrAnimator.SetTrigger(GetAnimationTrigger(animationType).triggerName);
    }
    public AnimationTriggers GetAnimationTrigger(AnimationType _animationType)
    {
        foreach (var trigger in triggers) if (trigger.animationType == _animationType) return trigger;
        return null;
    }
}
