using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	private Camera thisCamera;
	private float initSize;

	public LayerMask mask;

	public Vector3 offset;
    public List<GameObject> Player;

    public float rectLeftx, rectLefty;
    public float rectRightx, rectRighty;

    public const float rectLimit = 3.0f;

	public AnimationCurve zoomInCurve;

	public float tape = 0;
	float fixedZoomInTape = 0;
	float shakeTape = 0;

	public bool isZoomIn = false;
	public bool isFixedZoomIn = false;
	public bool isShake = false;

	public Vector3 shakeDirection;
	public AnimationCurve shakeAmount;

	public Vector3 centerPosition;

	public float zoomInSize;

	private float verticalRayLength;
	private float horizontalRayLength;

	private Vector3 vHitPos;
	private Vector3 hHitPos;

	public void StartFixedZoomIn(float size){
		zoomInSize = size;
		isFixedZoomIn = true;
		fixedZoomInTape = 0;
	}

	public void StartInstantZoomIn(){
		isZoomIn = true;
		tape = 0;
	}

	public void StartShake(Vector3 dir){
		isShake = true;
		shakeDirection = dir;
		shakeTape = 0;
	}

	bool isToLeft = false;
	bool isToRight = false;

	bool isToTop = false;
	bool isToBottom = false;

	public void RaycastHorizontal(){
		var max = Player[0].GetComponent<BoxCollider2D> ().bounds.max;
		var min = Player[0].GetComponent<BoxCollider2D> ().bounds.min;

		var len = max.y - min.y;

		for (int i = 0; i < 3; i++) {
			RaycastHit2D[] hits = Physics2D.RaycastAll (new Vector2 (max.x, min.y + len * ((float)i / (float)2)), Vector2.right, horizontalRayLength,mask);
			Debug.DrawLine (new Vector2 (max.x, min.y + len * ((float)i / (float)2)), new Vector2 (max.x, min.y + len * ((float)i / (float)2)) + Vector2.right*horizontalRayLength);
			isToRight = false;
			for(int j = 0; j < hits.Length; j++){
				var hit = hits [j];
				if (hit.collider != null && hit.collider.CompareTag("Bound")) {
					hHitPos = hit.point;
					isToRight = true;
					break;
				}
			}
		}
		for (int i = 0; i < 3; i++) {
			RaycastHit2D[] hits = Physics2D.RaycastAll (new Vector2 (min.x, min.y + len * ((float)i / (float)2)), Vector2.left, horizontalRayLength,mask);
			Debug.DrawLine (new Vector2 (min.x, min.y + len * ((float)i / (float)2)), new Vector2 (max.x, min.y + len * ((float)i / (float)2)) + Vector2.left*horizontalRayLength);
			isToLeft = false;
			for(int j = 0; j < hits.Length; j++){
				var hit = hits [j];
				if (hit.collider != null && hit.collider.CompareTag("Bound")) {
					hHitPos = hit.point;
					isToLeft = true;
					break;
				}
			}
		}
	}

	public void RaycastVertical(){
		var max = Player[0].GetComponent<BoxCollider2D> ().bounds.max;
		var min = Player[0].GetComponent<BoxCollider2D> ().bounds.min;

		var len = max.x - min.x;

		for (int i = 0; i < 3; i++) {
			RaycastHit2D[] hits = Physics2D.RaycastAll (new Vector2 (min.x+ len * ((float)i / (float)2), max.y ), Vector2.up, verticalRayLength,mask);
			Debug.DrawLine (new Vector2 (min.x+ len * ((float)i / (float)2), max.y), new Vector2 (min.x + len * ((float)i / (float)2), max.y) + Vector2.up*verticalRayLength);
			isToTop = false;
			for(int j = 0; j < hits.Length; j++){
				var hit = hits [j];
				if (hit.collider != null && hit.collider.CompareTag("Bound")) {
					isToTop = true;
					vHitPos = hit.point;
					break;
				}
			}
		}
		for (int i = 0; i < 3; i++) {
			RaycastHit2D[] hits = Physics2D.RaycastAll (new Vector2 (min.x + len * ((float)i / (float)2), min.y), Vector2.down, verticalRayLength,mask);
			Debug.DrawLine (new Vector2 (min.x+ len * ((float)i / (float)2), max.y), new Vector2 (min.x + len * ((float)i / (float)2), max.y) + Vector2.down*verticalRayLength);
			isToBottom = false;
			for(int j = 0; j < hits.Length; j++){
				var hit = hits [j];
				if (hit.collider != null && hit.collider.CompareTag("Bound")) {
					isToBottom = true;
					vHitPos = hit.point;
					break;
				}
			}
		}
	}

	private void InitCenterPosition(){
		centerPosition = Vector3.zero;
		for (int i = 0; i < Player.Count; i++) {
			centerPosition += Player [i].transform.position;
		}
		centerPosition /= Player.Count;
		centerPosition.z = transform.position.z;
	}

	private void LockCamera(){
		if ((!isToLeft && !isToRight) &&
			(!isToTop && !isToBottom)) {
			transform.position = Vector3.Lerp (transform.position, centerPosition, Time.deltaTime * 4.0f);
		} else {
			Vector2 dir = Vector2.zero;
			Debug.Log ("L : " + isToLeft + " R : " + isToRight + " T : " + isToTop + " B : " + isToBottom);
			if (isToLeft && isToRight) {
				hHitPos = centerPosition;
			} else {
				if (isToLeft) {
					dir.x += 1;
				}
				else if (isToRight) {
					dir.x += -1;
				}
				else if(!isToLeft && !isToRight){
					hHitPos = centerPosition;
				}
			}

			if (isToTop && isToBottom) {
				vHitPos = centerPosition;
			} else {
				if (isToTop) {
					dir.y += -1;
				}
				else if (isToBottom) {
					dir.y += 1;
				}
				else if(!isToTop && !isToBottom){
					vHitPos = centerPosition;
				}
			}
				
			var tmpOffset = offset;
			tmpOffset.x = hHitPos.x + (horizontalRayLength)* dir.x - dir.x;
			tmpOffset.y = vHitPos.y + (verticalRayLength) * dir.y - dir.y;
			tmpOffset.z = transform.position.z;
			transform.position = Vector3.Lerp (transform.position, tmpOffset, Time.deltaTime * 4.0f);
		}
	}

	void Start()
    {
		thisCamera = GetComponent<Camera> ();
		initSize = thisCamera.orthographicSize;
		horizontalRayLength = Camera.main.ScreenToWorldPoint (new Vector3 (Screen.width, Screen.height, 0)).x;
		verticalRayLength = Camera.main.ScreenToWorldPoint (new Vector3 (Screen.width, Screen.height, 0)).y;
		InitCenterPosition ();
	}

    void Update()
    {
		//if ray's collision object has "Bound" Tag then return isLeft = true bla bla bla..
		RaycastHorizontal ();
		RaycastVertical ();

		{//Must go in order -> 순서를 지켜야만 함
			InitCenterPosition ();
			LockCamera ();
		}

		if (isZoomIn) {
			if (tape <= 1) {
				thisCamera.orthographicSize = (initSize) * (1 + zoomInCurve.Evaluate (tape) * 0.008f);
				tape += Time.deltaTime*2;
			} else {
				tape = 0;
				isZoomIn = false;
			}
		}

		if (isFixedZoomIn) {
			if (fixedZoomInTape <= 0.1f) {
				thisCamera.orthographicSize = Mathf.Lerp (thisCamera.orthographicSize, zoomInSize, Time.deltaTime * 8f);
			} else {
				isFixedZoomIn = false;
				fixedZoomInTape = 0;
			}
		}

		if(isShake){
			if (shakeTape <= 0.25f) {
				thisCamera.transform.position += shakeDirection * shakeAmount.Evaluate (shakeTape / 0.25f)*Time.deltaTime*10;
				shakeTape += Time.deltaTime;
			} else {
				shakeTape = 0;
				isShake = false;
				shakeDirection = Vector3.zero;
			}
		}
    }
}
