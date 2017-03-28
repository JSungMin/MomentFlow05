using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowUpCamera : MonoBehaviour {

	public Transform followTarget;
	public LayerMask mask;
	private Camera mainCamera;

	public float followSpeed = 2;
	//if -1 => don't limit distance
	public float maxDis = -1;

	public Vector3 paddingVector;

	public bool isActive = true;

	Vector2 input;

	// Use this for initialization
	void Start () {
		mainCamera = GetComponent<Camera> ();	
	}

	bool isToLeft = false;
	bool isToRight = false;

	public void RaycastHorizontal(){
		var max = followTarget.GetComponent<BoxCollider2D> ().bounds.max;
		var min = followTarget.GetComponent<BoxCollider2D> ().bounds.min;

		var len = max.y - min.y;

			for (int i = 0; i < 3; i++) {
				RaycastHit2D hit = Physics2D.Raycast (new Vector2 (max.x, min.y + len * ((float)i / (float)2)), Vector2.right, 1.6f,mask);
				Debug.DrawLine (new Vector2 (max.x, min.y + len * ((float)i / (float)2)), new Vector2 (max.x, min.y + len * ((float)i / (float)2)) + Vector2.right*2);
				if (hit.collider != null) {
					
				isToRight = true;
				} else {
				isToRight = false;
				}
			}
			for (int i = 0; i < 3; i++) {
				RaycastHit2D hit = Physics2D.Raycast (new Vector2 (min.x, min.y + len * ((float)i / (float)2)), Vector2.left, 1.6f,mask);
				Debug.DrawLine (new Vector2 (min.x, min.y + len * ((float)i / (float)2)), new Vector2 (max.x, min.y + len * ((float)i / (float)2)) + Vector2.left*2);
				if (hit.collider != null) {
				isToLeft = true;
				} else {
				isToLeft = false;
				}
			}
	}

	// Update is called once per frame
	void Update () {
		RaycastHorizontal ();
		//if (!isToLeft&&!isToRight) {
			var targetPos = followTarget.position;
			targetPos.z = 0;
			var nowPos = mainCamera.transform.position;
			nowPos.z = 0;
			var tmpVector = paddingVector;
			tmpVector.x *= Input.GetAxis ("Horizontal");
			var pos = Vector3.Lerp (nowPos, targetPos + tmpVector, followSpeed * Time.deltaTime);
			pos.z = -30;
			mainCamera.transform.position = pos;
		//} else {
		//
		//}
	}
}
