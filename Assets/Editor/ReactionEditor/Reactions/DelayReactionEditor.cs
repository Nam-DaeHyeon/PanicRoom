#if UNITY_EDITOR 
using UnityEditor;
#endif 

[CustomEditor(typeof(DelayReaction))]
public class DelayReactionEditor : ReactionEditor
{

    protected override string GetFoldoutLabel()
    {
        return "Delay Reaction";
    }
}
