using UnityEngine;
using UnityEngine.UI;

public class EventEndReaction : DelayedReaction
{
	//raycast m_raycast;
	//private UICaching uiCaching;

    // ui 관리자
    private ActiveObjectInEvent uiManager;

    protected override void SpecificInit()
	{
        // UI 관리자 캐싱
        uiManager = FindObjectOfType<ActiveObjectInEvent>();

    }

    protected override void ImmediateReaction()
    {
        // UI 표시
        uiManager.ActiveTrueObjects();

        FSLocator.textDisplayer.reactionButton.enabled = false;
        FSLocator.textDisplayer.reactionButton.GetComponent<Image>().raycastTarget = false;
    }
}