using UnityEngine;
using System.Collections;
public class CameraShakingReaction : DelayedReaction
{
	private CameraShaking mCamera;

    // 이리저리 흔들리나?
	public bool shakePosition;
    // 돌아가며 흔들리나?
	public bool shakeRotation;
    // 흔들기 강도
	public float shakeIntensity;
    // 흔들기 시간
	public float shakeDecay;

	protected override void SpecificInit ()
	{
		mCamera = FindObjectOfType<CameraShaking> ();
	}

	protected override void ImmediateReaction()
	{
		mCamera.Init (shakePosition, shakeRotation, shakeIntensity, shakeDecay);
		mCamera.DoShake ();
	}
}