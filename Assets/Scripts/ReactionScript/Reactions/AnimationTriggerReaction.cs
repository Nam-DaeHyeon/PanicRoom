using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTriggerReaction : DelayedReaction
{

    public Animator target;
    public string triggerName;

    // 애니메이션 최소 시간
    public float time;

    // 코루틴 사용을 도와줄 객체
    private GameObject coroutineHelper;

    protected override void ImmediateReaction()
    {
        coroutineHelper = CoroutineHelper.StartNewCoroutine(Animating());
    }

    IEnumerator Animating()
    {
        FSLocator.textDisplayer.reactionButton.enabled = false;

        target.SetTrigger(triggerName);
        yield return new WaitForSeconds(time);

        FSLocator.textDisplayer.reactionButton.enabled = true;
        FSLocator.textDisplayer.reactionButton.onClick.Invoke();

        Destroy(coroutineHelper);
    }
}
