#if UNITY_EDITOR 
using UnityEditor;
#endif 

[CustomEditor(typeof(AnimationTriggerWithReaction))]
public class AnimationTriggerWithReactionEditor : ReactionEditor
{

    protected override string GetFoldoutLabel()
    {
        return "Animation Trigger With Reaction";
    }
}
