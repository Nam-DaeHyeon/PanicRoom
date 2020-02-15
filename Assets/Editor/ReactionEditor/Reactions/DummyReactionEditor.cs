#if UNITY_EDITOR 
using UnityEditor;
#endif 

[CustomEditor(typeof(DummyReaction))]
public class DummyReactionEditor : ReactionEditor
{

    protected override string GetFoldoutLabel()
    {
        return "Dummy Reaction";
    }
}
