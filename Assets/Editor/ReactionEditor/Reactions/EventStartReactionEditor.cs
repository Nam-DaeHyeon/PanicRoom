#if UNITY_EDITOR 
using UnityEditor;
#endif 

[CustomEditor(typeof(EventStartReaction))]
public class EventStartReactionEditor : ReactionEditor
{
    protected override string GetFoldoutLabel()
    {
        return "=== Event Start ===";
    }
}