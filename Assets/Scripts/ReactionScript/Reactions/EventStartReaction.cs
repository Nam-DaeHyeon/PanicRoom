using UnityEngine;
using UnityEngine.UI;

public class EventStartReaction : DelayedReaction
{
    // ui 관리자
    private ActiveObjectInEvent uiManager;

    protected override void SpecificInit()
	{
        // UI 관리자 캐싱
        uiManager = FindObjectOfType<ActiveObjectInEvent>();
    }


    protected override void ImmediateReaction()
    {
        // 이벤트의 무분별한 진행을 조절
        FSLocator.textDisplayer.reactionButton.GetComponent<Image>().raycastTarget = true;
        FSLocator.textDisplayer.reactionButton.enabled = false;

        // UI 제거
        uiManager.ActiveFalseObjects();
    }
}