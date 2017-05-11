using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;


public class Lever : InteractInterface
{
    private ThrowableObjectScript bigBox;

    private new void Awake()
    {
        base.Awake();
        bigBox = GetComponentInChildren<ThrowableObjectScript>();

        interact += Activate;
    }
	private IEnumerator DelayApplyGravity()
	{
		Camera.main.GetComponent<FollowUpCamera> ().SetFollowTarget (bigBox.transform);
		Camera.main.GetComponent<CameraMotionBlur> ().enabled = true;
		yield return new WaitForSeconds (1.5f);
		bigBox.applyGravity = true;
		yield return null;
	}
	private IEnumerator SetCameraTargetToPlayer()
	{
		yield return new WaitForSeconds (6.5f);
		Camera.main.GetComponent<FollowUpCamera> ().SetFollowTarget (GameObject.FindObjectOfType<Player> ().transform);
		Camera.main.GetComponent<CameraMotionBlur> ().enabled = false;
		yield return null;
	}

    private void Activate()
    {
        if(!isInteracted)
        {
            isInteracted = true;
			StartCoroutine("DelayApplyGravity");
			StartCoroutine ("SetCameraTargetToPlayer");
        }
    }

    private void InActivate()
    {

    }
}
