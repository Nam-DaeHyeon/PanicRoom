using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ReactionCollection : MonoBehaviour
{
    public Reaction[] reactions = new Reaction[0];

    private int startIndex = 0;
    [HideInInspector]
    public float delaytime;






	public void InitIndex()
	{
		startIndex = 0;

		for (int i = 0; i < reactions.Length; i++)
		{
			DelayedReaction delayedReaction = reactions[i] as DelayedReaction;

			if (delayedReaction)
				delayedReaction.Init();
			else
				reactions[i].Init();
		}
	}

    public void React()
    {
        // 들어있는 리액션이 없다.
		if (reactions.Length == 0) {

			return;
		}

        // 들어있는 리액션을 하나식 실행
        for (int i = startIndex; i < reactions.Length; i++)
        {
            // 새로운 리액션을 가져와..
            DelayedReaction delayedReaction = reactions[i] as DelayedReaction;


            // 가져온 리액선이 대화 리액션이면..
            if(reactions[i].GetType().Name == "TalkReaction")
            {
                if (startIndex == reactions.Length - 1)
                    break;
                else
                {
                    // 대화를 실행하며..
                    delayedReaction.React(this);

                    // 다음 리액션의 실행을 잠시 중단한다.
                    startIndex = i + 1;
                    FSLocator.textDisplayer.reactionButton.onClick.RemoveAllListeners();
                    FSLocator.textDisplayer.reactionButton.onClick.AddListener(delegate { this.React(); });
                    return;
                }
            }
            else if(reactions[i].GetType().Name == "FadeOutReaction")
            {
                if (startIndex == reactions.Length - 1)
                    break;
                else
                {
                    // 대화를 실행하며..
                    delayedReaction.React(this);

                    // 다음 리액션의 실행을 잠시 중단한다.
                    startIndex = i + 1;
                    FSLocator.textDisplayer.reactionButton.onClick.RemoveAllListeners();
                    FSLocator.textDisplayer.reactionButton.onClick.AddListener(delegate { this.React(); });
                    return;
                }

            }
            else if (reactions[i].GetType().Name == "SurpriseEffectReaction")
            {
                if (startIndex == reactions.Length - 1)
                    break;
                else
                {
                    // 대화를 실행하며..
                    delayedReaction.React(this);

                    // 다음 리액션의 실행을 잠시 중단한다.
                    startIndex = i + 1;
                    FSLocator.textDisplayer.reactionButton.onClick.RemoveAllListeners();
                    FSLocator.textDisplayer.reactionButton.onClick.AddListener(delegate { this.React(); });
                    return;
                }
            }
            else if (reactions[i].GetType().Name == "CharacterMoveReaction")
            {
                if (startIndex == reactions.Length - 1)
                    break;
                else
                {
                    // 대화를 실행하며..
                    delayedReaction.React(this);

                    // 다음 리액션의 실행을 잠시 중단한다.
                    startIndex = i + 1;
                    FSLocator.textDisplayer.reactionButton.onClick.RemoveAllListeners();
                    FSLocator.textDisplayer.reactionButton.onClick.AddListener(delegate { this.React(); });
                    return;
                }
            }
            else if (reactions[i].GetType().Name == "AnimationTriggerReaction")
            {
                if (startIndex == reactions.Length - 1)
                    break;
                else
                {
                    // 대화를 실행하며..
                    delayedReaction.React(this);

                    // 다음 리액션의 실행을 잠시 중단한다.
                    startIndex = i + 1;
                    FSLocator.textDisplayer.reactionButton.onClick.RemoveAllListeners();
                    FSLocator.textDisplayer.reactionButton.onClick.AddListener(delegate { this.React(); });
                    return;
                }
            }
            else if (reactions[i].GetType().Name == "DelayReaction")
            {
                if (startIndex == reactions.Length - 1)
                    break;
                else
                {
                    // 대화를 실행하며..
                    delayedReaction.React(this);

                    // 다음 리액션의 실행을 잠시 중단한다.
                    startIndex = i + 1;
                    FSLocator.textDisplayer.reactionButton.onClick.RemoveAllListeners();
                    FSLocator.textDisplayer.reactionButton.onClick.AddListener(delegate { this.React(); });
                    return;
                }
            }
            else if (reactions[i].GetType().Name == "ChoiceReaction")
            {
                if (startIndex == reactions.Length - 1)
                    break;
                else
                {
                    // 대화를 실행하며..
                    delayedReaction.React(this);

                    // 다음 리액션의 실행을 잠시 중단한다.
                    startIndex = i + 1;
                    FSLocator.textDisplayer.reactionButton.onClick.RemoveAllListeners();
                    FSLocator.textDisplayer.reactionButton.onClick.AddListener(delegate { this.React(); });
                    return;
                }
            }
            else
            {
                delayedReaction.React(this);
            }
        }
    }

	public void InitAndReact(){
		InitIndex ();
		React ();
	}

	public void MoveAround()
	{
		reactions[0].React(this);
	}
  
}
