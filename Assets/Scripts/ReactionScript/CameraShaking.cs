using UnityEngine; 
using System.Collections;

public class CameraShaking : MonoBehaviour
{
	[HideInInspector]
	public bool shakePosition;
	[HideInInspector]
	public bool shakeRotation;
	[HideInInspector]
	public float shakeIntensity = 0.5f;
	[HideInInspector]
	public float shakeDecay = 0.02f;

	private Vector3 OriginalPos;
	private Quaternion OriginalRot;

	private bool isShakeRunning = false;

	public void Init(bool shakeWithPosition, bool shakeWithRotation, float intensity, float decay)
	{
		shakePosition = shakeWithPosition;
		shakeRotation = shakeWithRotation;
		shakeIntensity = intensity;
		shakeDecay = decay;
	}

	public void DoShake()
	{
		OriginalPos = transform.position;
		OriginalRot = transform.rotation;


	   //transform.parent.GetComponent<MainCamera>() .enabled = false;

		StartCoroutine("ProcessShake");
	}

	public void StopShake()
	{
		transform.position = OriginalPos;
		transform.rotation = OriginalRot;

		StopCoroutine("ProcessShake");
		isShakeRunning = false;
	}

	IEnumerator ProcessShake()
	{
		if (!isShakeRunning)
		{
			isShakeRunning = true;
			float currentShakeIntensity = shakeIntensity;


			while (currentShakeIntensity > 0)
			{
				if (shakePosition)
				{
					transform.position = OriginalPos + Random.insideUnitSphere * currentShakeIntensity *.02f;

				}
				if (shakeRotation)
				{
					transform.rotation = new Quaternion(OriginalRot.x + Random.Range(-currentShakeIntensity, currentShakeIntensity) * .02f,
						OriginalRot.y + Random.Range(-currentShakeIntensity, currentShakeIntensity) * .02f,
						OriginalRot.z + Random.Range(-currentShakeIntensity, currentShakeIntensity) * .02f,
						OriginalRot.w + Random.Range(-currentShakeIntensity, currentShakeIntensity) * .02f);
				}
				currentShakeIntensity -= shakeDecay;
				yield return null;
			}

			isShakeRunning = false;

            //transform.parent.GetComponent<MainCamera>().enabled = true;
		}
	}
}