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

	private const float MAX_GRAB_HEIGHT_RATIO = 1.5f;

	public AnimationState animationState;
    // 플레이어 크기와 비슷한 충돌체
    public BoxCollider2D playerBC;

	bool isAir = false;

	public void ProcessGround(){
		if (controller.collisions.above || controller.collisions.below) {
			velocity.y = 0;

			isJump = false;
			isAir = false;
		} else {
			isAir = true;
		}
	}
	[HideInInspector]
	public Vector2 input;
	float saveDelay = 0;

	float timer;

	void ProcessMove(){
		input = new Vector2 (Input.GetAxis ("Horizontal"), Input.GetAxis ("Vertical"));

		timer = input.x == 0 ? 0 : timer + Time.deltaTime;

		velocity.x += input.x*moveSpeed*moveStep.Evaluate(timer);
		velocity.y += gravity * Time.deltaTime;

		if (input.x != 0) {
			state = State.Walk;
		} else {
			state = State.Idle;
		}

	}

	float jumpSaveDelay=0;
	void ProcessJump(){
		if(Input.GetKey(KeyCode.Space)){
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
			var cols = Physics2D.OverlapBoxAll (colPos, new Vector2 (bc.size.x, bc.size.y), 0,1<<LayerMask.NameToLayer("Collision"));

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

	public float climbDuration;
	float climbDurationTimer = 0;
	bool isClimb = false;

	float tClimbHeight = 0;
	float tClimbSpeed = 0;

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
				anim.SetClimb ();
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
			var cols = Physics2D.OverlapBoxAll (pos, new Vector2 (0.2f, 0.36f), 0);
			for (int i = 0; i < cols.Length; i++) {
				var c = cols [i];
				if (c.gameObject.layer == LayerMask.NameToLayer ("Collision") && 
					c.CompareTag("GrabableObject")||c.CompareTag("Ground")&&
					TimeLayer.EqualTimeLayer (pTimeLayer, c.transform.GetComponentInParent<TimeLayer> ())) {
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

	public float rollingDuration = 0.25f;
	float rollingDurationTimer = 0;

	public float rollingCoolDuration = 1;
	float rollingCollDurationTimer = 0;

	public float rollingSpeed = 2;

	//if isRolling is true then enemy's findOutGauge can't be increased
	bool isRolling = false;

	void Rolling(){
		if (isRolling) {
			if (rollingDurationTimer <= rollingDuration) {
				rollingDurationTimer += Time.deltaTime;
				velocity.x = rollingSpeed * Mathf.Sign(transform.localScale.x);
				state = State.Rolling;
			} else {
				isRolling = false;
				rollingDurationTimer = 0;
			}
		} else {
			rollingCollDurationTimer += Time.deltaTime;
		}
	}

	void ProcessRolling(){
		if (Input.GetKey (KeyCode.LeftShift) && 
			!isRolling && !isAir &&
			rollingCollDurationTimer >= rollingCoolDuration) {

			isRolling = true;
			rollingCollDurationTimer = 0;
		}
		Rolling ();
	}
    
	void Start () {
		pTimeLayer = transform.GetComponentInParent<TimeLayer> ();
		animator = GetComponent<Animator> ();
		controller = GetComponent<Controller2D> ();
		gravity = -9.8f;
		jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
    }

	void Update () {
		if (hp <= 0) {
			Destroyed ();
		}
		ProcessGround();
		ProcessMove();
        ProcessJump();
		ProcessGrabCorner();
		ProcessRolling ();
		ProcessTimeSwitching();
		if(!isGrabing){
			controller.Move ( velocity * Time.deltaTime);
			velocity.x = 0;
		}
	}
}
