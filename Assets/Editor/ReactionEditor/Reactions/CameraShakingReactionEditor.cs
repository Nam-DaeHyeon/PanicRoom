#if UNITY_EDITOR 
using UnityEditor;
#endif 

[CustomEditor(typeof(CameraShakingReaction))]
public class CameraShakingReactionEditor : ReactionEditor
{
	protected override string GetFoldoutLabel ()
	{
		return "Camera Shaking Reaction";
	}
}
