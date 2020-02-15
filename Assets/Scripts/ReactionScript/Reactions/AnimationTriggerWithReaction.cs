using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTriggerWithReaction : DelayedReaction
{

    public Animator target;
    public string triggerName;

    // 코루틴 사용을 도와줄 객체
    private GameObject coroutineHelper;

    protected override void ImmediateReaction()
    {
        target.SetTrigger(triggerName);
    }
}
