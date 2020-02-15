#if UNITY_EDITOR 
using UnityEditor;
#endif 

[CustomEditor(typeof(ObjectActiveReaction))]
public class ObjectActiveReactionEditor : ReactionEditor
{

    protected override string GetFoldoutLabel()
    {
        return "Object Active Reaction";
    }
}
