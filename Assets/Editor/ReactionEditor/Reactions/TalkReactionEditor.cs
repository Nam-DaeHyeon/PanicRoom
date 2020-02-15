#if UNITY_EDITOR 
using UnityEditor;
#endif 

[CustomEditor(typeof(TalkReaction))]
public class TalkReactionEditor : ReactionEditor
{
    protected override string GetFoldoutLabel()
    {
        return "Talk Reaction";
    }
}
