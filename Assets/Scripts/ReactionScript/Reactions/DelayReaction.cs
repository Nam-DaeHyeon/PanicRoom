using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayReaction : DelayedReaction
{
    public float delayTime;

    // 코루틴 사용을 도와줄 객체
    private GameObject coroutineHelper;

    protected override void ImmediateReaction()
    {
        coroutineHelper = CoroutineHelper.StartNewCoroutine(Delaying());
    }

    IEnumerator Delaying()
    {
        FSLocator.textDisplayer.reactionButton.enabled = false;

        yield return new WaitForSeconds(delayTime);

        // 다음 이벤트 내용이 담긴 코루틴을 실행할 수 있도록 한다.
        FSLocator.textDisplayer.reactionButton.enabled = true;
        FSLocator.textDisplayer.reactionButton.onClick.Invoke();

        Destroy(coroutineHelper);
    }
}
