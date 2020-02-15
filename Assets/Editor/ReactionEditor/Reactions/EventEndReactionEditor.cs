#if UNITY_EDITOR 
using UnityEditor;
#endif 

[CustomEditor(typeof(EventEndReaction))]
public class EventEndReactionEditor : ReactionEditor
{
    protected override string GetFoldoutLabel()
    {
        return "=== Event End ===";
    }
}
