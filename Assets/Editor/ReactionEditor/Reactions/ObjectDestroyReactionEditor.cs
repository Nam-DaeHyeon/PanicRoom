#if UNITY_EDITOR 
using UnityEditor;
#endif 

[CustomEditor(typeof(ObjectDestroyReaction))]
public class ObjectDestroyReactionEditor : ReactionEditor
{
    protected override string GetFoldoutLabel()
    {
        return "Object Destory Reaction";
    }
}
