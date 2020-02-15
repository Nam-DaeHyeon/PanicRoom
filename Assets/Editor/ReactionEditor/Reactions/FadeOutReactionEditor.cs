#if UNITY_EDITOR 
using UnityEditor;
#endif 

[CustomEditor(typeof(FadeOutReaction))]
public class FadeOutReactionEditor : ReactionEditor
{
    protected override string GetFoldoutLabel()
    {
        return "FadeOut Reaction";
    }
}