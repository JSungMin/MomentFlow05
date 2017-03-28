using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableObjectScript : InteractInterface {
	private Transform initRootTrans;
	private Transform playerHands;

	private bool isGrabbed;
	private RaycastHit2D[] hitsX;
	private RaycastHit2D[] hitsY;

	public LayerMask mask;
	public LayerMask alertMask;
	public bool applyGravity = true;
	public float flingPower;
	public int rayDensity;
	public Vector2 velocity;

	public float alertRadius;

	public void CalculateThrowVelocity(){
		Vector3 mp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector3 dir = (mp - transform.position).normalized;
		velocity.x = dir.x * flingPower * 2;
		velocity.y = dir.y * flingPower;
	}

	//if this object collides with collision layer object 
	//below function will be used.
	public void AlertToNearEnemy(){
		var colliders = Physics2D.OverlapCircleAll (transform.position, alertRadius, alertMask);
	
		for(int i = 0;i<colliders.Length;i++){
			var escr = colliders [i].GetComponent<EnemyScript> ();
			RaycastHit2D hit = Physics2D.Raycast (transform.position, (escr.transform.position - transform.position).normalized, alertRadius,mask);
			Debug.DrawLine (transform.position, transform.position + (escr.transform.position - transform.position).normalized * alertRadius,Color.red);
			if(null!=escr){
				if (hit.collider.gameObject.layer != LayerMask.NameToLayer("Collision")) {
					if (!escr.GetSpecifiedState<DetectionState> (State.Detection).isDetection) {
						escr.GetSpecifiedState<SuspiciousState> (State.Suspicious).InitSuspiciousInfo (transform.position, escr.moveSpeed * 0.5f);
						escr.SetState (State.Suspicious);
						escr.InitToTransition ();
						Debug.Log (escr.name);
					}
				}
			}
		}
	}

	//Interact Function
	public void GrabObject(){
		if (!isInteract) {
			Debug.Log ("Grab");
			
			transform.parent = playerHands.transform;
			transform.localPosition = Vector3.zero;
			
			velocity = Vector3.zero;
			applyGravity = false;
			isGrabbed = true;
		}
	}
	//Stop Interact Function
	public void ReleaseObject(){
		isInteract = false;
		GameObject.FindObjectOfType<InteractiveManager> ().nowInteract = false;
		transform.parent = initRootTrans;

		applyGravity = true;
		isGrabbed = false;
	}
	//triggered when is grabbed and mouse left button click
	public void ThrowObject(){
		Debug.Log ("Throw");
		ReleaseObject ();
		CalculateThrowVelocity ();
	}
	float maxX,maxY;
	float minX,minY;
	float colXLen;
	float colYLen;
	public void RayCastByVelocity(){
		hitsX = new RaycastHit2D[rayDensity];
		hitsY = new RaycastHit2D[rayDensity];
		maxX = GetComponent<Collider2D> ().bounds.max.x;
		maxY = GetComponent<Collider2D> ().bounds.max.y;
		minX = GetComponent<Collider2D> ().bounds.min.x;
		minY = GetComponent<Collider2D> ().bounds.min.y;

		Vector2 xDir = Vector2.zero;
		Vector2 yDir = Vector2.zero;

		for(int i =0;i<rayDensity;i++){
			if (velocity.x > 0) {
				xDir = Vector2.right;
				hitsX [i] = Physics2D.Raycast (
					new Vector2 (maxX, minY + colYLen * (i / (rayDensity - 1))),
					xDir,Mathf.Abs(velocity.x)*Time.deltaTime,mask);
			} else if(velocity.x<0){
				xDir = Vector2.left;
				hitsX [i] = Physics2D.Raycast (
					new Vector2 (minX, minY + colYLen * (i / (rayDensity - 1))),
					xDir,Mathf.Abs(velocity.x)*Time.deltaTime,mask);
			}

			if (velocity.y > 0) {
				yDir = Vector2.up;
				hitsY [i] = Physics2D.Raycast (
					new Vector2 (minX + colXLen * (i / (rayDensity - 1)), maxY),
					yDir,Mathf.Abs(velocity.y)*Time.deltaTime,mask);
			} else if (velocity.y < 0) {
				yDir = Vector2.down;
				hitsY [i] = Physics2D.Raycast (
					new Vector2 (minX + colXLen * (i / (rayDensity - 1)), minY),
					yDir,Mathf.Abs(velocity.y)*Time.deltaTime,mask);
			}
			Vector2 reflectVec = Vector2.zero;
			if(hitsX[i].collider!=null){
				if(Vector3.Magnitude(velocity)>=2)
					AlertToNearEnemy ();
				velocity += Vector2.Reflect (xDir*Mathf.Abs(velocity.x), hitsX [i].normal);
			}
			if(hitsY[i].collider!=null){
				if(Vector3.Magnitude(velocity)>=2)
					AlertToNearEnemy ();
				velocity += Vector2.Reflect (yDir*Mathf.Abs(velocity.y), hitsY [i].normal);
			}
		}
	}

	// Use this for initialization
	void Start () {
		initRootTrans = transform.parent;
		playerHands = GameObject.FindObjectOfType<Player> ().transform.GetChild (2);
		interact += GrabObject;
		stopInteract += ReleaseObject;
	}

	void Update(){
		if(applyGravity){
			velocity += Vector2.down * 9.8f * Time.deltaTime;
		}
		RayCastByVelocity ();
		velocity.x = Mathf.Lerp (velocity.x, 0, Time.deltaTime * 3f);

		if(isGrabbed){
			if (Input.GetMouseButtonDown (0)) {
				ThrowObject ();
			}
		}

		transform.Translate (velocity * Time.deltaTime);

	}
}
