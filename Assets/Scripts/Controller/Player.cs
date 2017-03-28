using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MyObject
{
	public Animator animator;
	public PlayerAnim anim;

	public Transform playerHandTrans;

	private Controller2D controller;

	public float jumpHeight = 10;
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

	public AnimationState animationState;


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
		if (input.x != 0) {
			state = State.Walk;
		} else {
			state = State.Idle;
		}
		velocity.x += input.x*moveSpeed*moveStep.Evaluate(timer);
		velocity.y += gravity * Time.deltaTime;
	}

	float jumpSaveDelay=0;
	void ProcessJump(){
		if(Input.GetKeyDown(KeyCode.Space)){
			if (!isJump) {
				velocity.y += jumpHeight;
				isJump = true;
			}
		}
	}

	void ProcessTimeSwitching(){
		if(Input.GetKeyDown(KeyCode.Tab)){
			int toLayer = 0;
			if (TimeLayer.GetTimeLayer (gameObject) == 0) {
				toLayer = 1;
			} else {
				toLayer = 0;
			}
			GetComponent<TimeLayer> ().layerNum = toLayer;
			TimeLayerManager.GetInstance.MoveObjectToLayer (gameObject, toLayer);
			var equipItems = playerHandTrans.GetComponentsInChildren<TimeLayer> ();
			for(int i =0;i<equipItems.Length;i++){
				equipItems [i].layerNum = toLayer;
			}	
		}
	}

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator> ();
		controller = GetComponent<Controller2D> ();
		gravity = -9.8f;
		//gravity = -(2*jumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
	}

	// Update is called once per frame
	void Update () {
		if (hp <= 0) {
			Destroyed ();
		}
		ProcessTimeSwitching ();
		ProcessGround ();
		ProcessMove ();

		controller.Move ( velocity * Time.deltaTime);
		velocity.x = 0;
	}
}
