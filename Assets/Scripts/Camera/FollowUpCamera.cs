using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowUpCamera : MonoBehaviour {

	public Transform followTarget;

	private Camera mainCamera;

	public float followSpeed = 2;
	//if -1 => don't limit distance
	public float maxDis = -1;

	public Vector3 paddingVector;

	public bool isActive = true;

	// Use this for initialization
	void Start () {
		mainCamera = GetComponent<Camera> ();	
	}
	
	// Update is called once per frame
	void Update () {
		if (isActive) {
			var targetPos = followTarget.position;
			targetPos.z = 0;
			var nowPos = mainCamera.transform.position;
			nowPos.z = 0;
			var tmpVector = paddingVector;
			tmpVector.x *= Input.GetAxis ("Horizontal");
			var pos = Vector3.Lerp (nowPos, targetPos + tmpVector, followSpeed * Time.deltaTime);
			pos.z = -30;
			mainCamera.transform.position = pos;
		}
	}
}
