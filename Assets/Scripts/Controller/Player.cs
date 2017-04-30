using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MyObject
{
	public TimeLayer pTimeLayer;

	public TimeLayer ParentTimeLayer{
		get{
			return pTimeLayer;
		}
		set{
			if(value != null){
				pTimeLayer = value;
			}
		}
	}

	public Animator animator;
	public PlayerAnim anim;

	public Transform playerHandTrans;

	private Controller2D controller;

	private BoxCollider2D pBoxCollider;

	private Vector2 INIT_COLLIDER_OFFSET;
	private Vector2 SIT_COLLIDER_OFFSET;
	private Vector2 INIT_COLLIDER_SIZE = new Vector2(0.2f, 0.36f);
	private Vector2 SIT_COLLIDER_SIZE = new Vector2 (0.2f,0.2f);

	private const float MAX_GRAB_HEIGHT_RATIO = 1.5f;

	public float jumpHeight = 0.1f;
	public float timeToJumpApex = .4f;
	private float accelerationTimeAirborne = .2f;
	private float accelerationTimeGrounded = .1f;
	public float moveSpeed = 6;

	public AnimationCurve moveStep;

	public Vector3 velocity;
	private float jumpVelocity;
	private float velocityXSmoothing;

	private float gravity;

	public bool isJump;
	bool isAir = false;

	[HideInInspector]
	public Vector2 input;
	float saveDelay = 0;

	float timer;

	public AnimationState animationState;

	void Start () {
		pTimeLayer = transform.GetComponentInParent<TimeLayer> ();
		animator = GetComponent<Animator> ();
		controller = GetComponent<Controller2D> ();
		pBoxCollider = GetComponent<BoxCollider2D> ();
		INIT_COLLIDER_OFFSET = pBoxCollider.offset;
		SIT_COLLIDER_OFFSET = INIT_COLLIDER_OFFSET - Vector2.up*0.08f;
		gravity = -9.8f;
		jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
	}

	public void Damaged(float dAmount){
		if (hp - dAmount <= 0){
			Dead ();
			return;
		}
		hp -= dAmount;
	}

	public void Dead(){
		Debug.Log ("Player Dead");
	}

	public void ProcessGround(){
		if (controller.collisions.above || controller.collisions.below) {
			velocity.y = 0;

			isJump = false;
			isAir = false;
		} else {
			isAir = true;
		}
	}

	void ProcessMove(){
		input = new Vector2 (Input.GetAxis ("Horizontal"), Input.GetAxis ("Vertical"));

		timer = input.x == 0 ? 0 : timer + Time.deltaTime;

		velocity.x += input.x*moveSpeed*moveStep.Evaluate(timer);
		velocity.y += gravity * Time.deltaTime;

		if (input.x != 0) {
			state = State.Walk;
		} else {
			if (isSit)
				state = State.Sit;
			else
				state = State.Idle;
		}
	}

	float jumpSaveDelay=0;
	void ProcessJump(){
		if(Input.GetKeyDown(KeyCode.Space)){
			if (!isJump && velocity.y>= gravity * Time.deltaTime*7.0f) {
				velocity.y = jumpHeight;
				isJump = true;
			}
		}
	}

	void ProcessTimeSwitching(){
        // 다른 레이어의 통과 불가능한 지형들과 플레이어가 충돌한다면
        // 타임 스위칭을 할 수 없다
		if(Input.GetKeyDown(KeyCode.Tab)){
			var bc = GetComponent<BoxCollider2D> ();
			var colPos = transform.position + new Vector3(bc.offset.x,bc.offset.y,0);
			var cols = Physics2D.OverlapBoxAll (colPos, new Vector2 (bc.size.x*0.8f, bc.size.y*0.8f), 0,1<<LayerMask.NameToLayer("Collision"));

			bool canSwitching = true;

			int toLayer = 0;
			if (pTimeLayer.layerNum == 0) {
				toLayer = 1;
			}

			for(int i = 0;i<cols.Length;i++){
				var c = cols [i];
				Debug.Log ("Name : " + c.gameObject.name + " toLayer : " + toLayer + " hit : " + c.GetComponentInParent<TimeLayer>().layerNum);
				if(c.GetComponentInParent<TimeLayer>().layerNum == toLayer){
					Debug.Log ("Overlap");
					canSwitching = false;
				}
			}

			if (canSwitching) {
				TimeLayerManager.GetInstance.MoveObjectToLayer (gameObject, toLayer);
				pTimeLayer = transform.GetComponentInParent<TimeLayer> ();
				controller.pTimeLayer = pTimeLayer;
			}
		}
		Camera.main.GetComponent<GrayScaleEffect> ().intensity = Mathf.Lerp (Camera.main.GetComponent<GrayScaleEffect> ().intensity, 1 - pTimeLayer.layerNum, Time.deltaTime*2);
	}
		
	Collider2D grappingObj;

	private bool isClimb = false;
	//만족 스러운 Duration 찾은 후 private나 const로 지정
	public float climbDuration;
	private float climbDurationTimer = 0;

	//t는 tmp의 약자로 함수 실행마다 재정의됨
	private float tClimbHeight = 0;
	private float tClimbSpeed = 0;

	//만족 스러운 Duration 찾은 후 private나 const로 지정
	public float climbDelayDuration;

	void ClimbCorner(){
		if(isGrabing && Input.GetKeyDown(KeyCode.Space) && !isClimb){
			isClimb = true;
			tClimbHeight = GetComponent<BoxCollider2D> ().bounds.max.y - GetComponent<BoxCollider2D>().bounds.min.y;
			tClimbSpeed = tClimbHeight / climbDuration;
		}
		if(isClimb){
			if (climbDurationTimer <= climbDuration) {
				climbDurationTimer += Time.deltaTime;

				transform.position += Vector3.up * tClimbSpeed * Time.deltaTime;
				state = State.ClimbCorner;
			} else {
				isClimb = false;
				isGrabing = false;

				state = State.Idle;

				climbDurationTimer = 0;
				tClimbHeight = 0;
				tClimbSpeed = 0;

				velocity = Vector3.zero;

				var newPos = transform.position;
				newPos.x += (grappingObj.transform.position - transform.position).normalized.x * (GetComponent<BoxCollider2D> ().bounds.extents.x);
				newPos.y = grappingObj.bounds.max.y;
				transform.position = newPos;
			}
		}
	}

    // isGrabing이 true라면 플레이어의 위치를 벽 코너에 고정한다
    private bool isGrabing;
    // 벽의 모서리를 잡는 함수
    void ProcessGrabCorner()
    {
		// if 벽 모서리와 닿아 있다면 isGrabing = true
		if (isAir && !isClimb) {
			Vector2 pos = new Vector2 (transform.position.x, transform.position.y);
			var cols = Physics2D.OverlapBoxAll (pos, new Vector2 (0.20f, 0.35f), 0);
			for (int i = 0; i < cols.Length; i++) {
				var c = cols [i];
				if (c.gameObject.layer == LayerMask.NameToLayer ("Collision") && 
					(c.CompareTag("GrabableObject") && TimeLayer.EqualTimeLayer (pTimeLayer, c.transform.GetComponentInParent<TimeLayer> ()))||
					(c.CompareTag("GrabableGround"))) {
					bool isFoward = (Mathf.Sign ((c.transform.position - transform.position).x) == Mathf.Sign (transform.localScale.x)) ? true : false;
					if (isFoward) {
						if (GetComponent<BoxCollider2D> ().bounds.min.y < c.bounds.max.y) {
							if (GetComponent<BoxCollider2D> ().bounds.max.y >= c.bounds.max.y - 0.1f) {
								isGrabing = true;
								state = State.GrabCorner;
								grappingObj = c;
								velocity = Vector3.zero;
								var newPos = transform.position;
								newPos.y = c.bounds.max.y - GetComponent<BoxCollider2D> ().size.y;
								transform.position = newPos;
							}
						}
					} else {
						isGrabing = false;
						state = State.Idle;
						grappingObj = null;
					}
				}
			}
		}
		ClimbCorner ();
    }
		
	public bool isSit = false;

	void Sit(){
		if (isSit) {
			pBoxCollider.offset = SIT_COLLIDER_OFFSET;
			pBoxCollider.size = SIT_COLLIDER_SIZE;
		} else {
			pBoxCollider.offset = INIT_COLLIDER_OFFSET;
			pBoxCollider.size = INIT_COLLIDER_SIZE;
		}
	}
    
	bool IsVerticalCollision(){
		for(int i = 0; i < 3; i++){
			var hits = Physics2D.RaycastAll (Vector3.up*pBoxCollider.bounds.max.y + Vector3.right*pBoxCollider.bounds.min.x * (i / 2), Vector2.up,(INIT_COLLIDER_SIZE.y - SIT_COLLIDER_SIZE.y),controller.collisionMask);
			for(int j = 0; j < hits.Length; j++){
				var hit = hits [j];
				if(hit.collider != null && TimeLayer.EqualTimeLayer(hit.collider.GetComponentInParent<TimeLayer>(),pTimeLayer)){
					return true;
				}
			}
		}
		return false;
	}

	void ProcessSit(){
		if (Input.GetKey (KeyCode.S) && !isAir) {
			isSit = true;
		} 
		else {
			if (!IsVerticalCollision()) {
				isSit = false;
			}
		}
		Sit ();
	}

	void Update () {
		if (hp <= 0) {
			Destroyed ();
		}
		ProcessGround();
		ProcessMove();
        ProcessJump();
		ProcessGrabCorner();
		ProcessSit ();
		ProcessTimeSwitching();
		if(!isGrabing){
			controller.Move ( velocity * Time.deltaTime);
			velocity.x = 0;
		}
	}
}
