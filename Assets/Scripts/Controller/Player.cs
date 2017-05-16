using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public delegate void UndoDelegate();

public class Player : MyObject
{
	private TimeLayer pTimeLayer;

	public float prevZPos;
    public float deltaY;
  
	public TimeLayer ParentTimeLayer
    {
        get
        {
            return pTimeLayer;
        }
        set
        {
            if (value != null)
            {
                pTimeLayer = value;
            }
        }
    }

    public Animator animator;
    public PlayerAnim anim;

    public Transform playerHandTrans;

    private Controller2D controller;

    private BoxCollider pBoxCollider;
	private Camera mainCamera;

    private Vector3 INIT_COLLIDER_OFFSET;
    private Vector3 SIT_COLLIDER_OFFSET;
    private Vector3 INIT_COLLIDER_SIZE = new Vector3(0.2f, 0.36f, 0);
    private Vector3 SIT_COLLIDER_SIZE = new Vector3(0.2f, 0.2f, 0);

    private const float MAX_GRAB_HEIGHT_RATIO = 1.5f;

    public float jumpHeight = 0.1f;
    public float timeToJumpApex = .4f;
    private float accelerationTimeAirborne = .2f;
    private float accelerationTimeGrounded = .1f;
    public float moveSpeed = 6;

    public AnimationCurve moveStep;

    public Vector3 velocity;

    private float velocityXSmoothing;

    private float gravity;

    public bool isJump;
	public bool isAir = false;
    private bool isWalkOnStair = false;

	public float distanceToGround = 0;

    [HideInInspector]
    public Vector2 input;
	float inputXPrevJump;
	float saveDelay = 0;

    float timer;

    public AnimationState animationState;
    private InteractiveManager interacitveManager;

	void InitStatePriority ()
	{
		statePriority = new State[12];
		statePriority [11] = State.Idle;
		statePriority [10] = State.Walk;
		statePriority [9] = State.Run;
		statePriority [8] = State.Jump;
		statePriority [7] = State.Fall;
		statePriority [6] = State.Landing;
		statePriority [5] = State.GrabCorner;
		statePriority [4] = State.ClimbCorner;
		statePriority [3] = State.Attack;
		statePriority [2] = State.Dizzy;
		statePriority [1] = State.Damaged;
		statePriority [0] = State.Dead;
	}

    void Start()
    {
		InitStatePriority ();
        pTimeLayer = transform.GetComponentInParent<TimeLayer>();
        pBoxCollider = GetComponent<BoxCollider>();
        animator = GetComponent<Animator>();
        controller = GetComponent<Controller2D>();
        interacitveManager = GameObject.FindObjectOfType<InteractiveManager>();
        mainCamera = Camera.main;

        INIT_COLLIDER_OFFSET = pBoxCollider.center;
        SIT_COLLIDER_OFFSET = INIT_COLLIDER_OFFSET - Vector3.up * 0.08f;
        gravity = -5;
    }

	public void CheckAndAddIdleState ()
	{
		if (input.x == 0 && !isAir)
		{
			AddToStateQueueWithCheckingOverlap (GetStatePriorityLevel (State.Idle));
		}
	}

	public void CheckAndAddRunState ()
	{
		if (input.x != 0 && !isAir)
		{
			AddToStateQueueWithCheckingOverlap (GetStatePriorityLevel (State.Run));
		}
	}

	public void CheckAndAddJumpState ()
	{
		if (Input.GetKeyDown(KeyCode.Space) && !isAir && !isJump)
		{
			AddToStateQueueWithCheckingOverlap(GetStatePriorityLevel(State.Jump));
		}
	}

	public void CheckAndAddFallState ()
	{
		if (velocity.y < 0f && isAir && distanceToGround >= 0.1f)
		{
			AddToStateQueueWithCheckingOverlap (GetStatePriorityLevel (State.Fall));
		}
	}

	public void CheckAndAddLandingState ()
	{
		if (isAir && distanceToGround <= 0.25f && velocity.y < -1f)
		{
			AddToStateQueueWithCheckingOverlap (GetStatePriorityLevel (State.Landing));
		}
	}

	float landingDelay = 0.4f;
	float landingDelayTimer = 0f;

	public void UpdateAndApplyPlayerState ()
	{
		stateQueue.Sort ();

		if(stateQueue.Count != 0)
			state = statePriority[stateQueue [0]];
		switch (state) 
		{
		case State.Idle:
			DeleteFromStateQueue (GetStatePriorityLevel (State.Idle));
			break;
		case State.Walk:
			velocity.x = input.x * moveSpeed * moveStep.Evaluate (timer);
			DeleteFromStateQueue (GetStatePriorityLevel (State.Walk));
			break;
		case State.Run:
			velocity.x = input.x * moveSpeed * moveStep.Evaluate (timer);
			DeleteFromStateQueue (GetStatePriorityLevel (State.Run));
			break;
		case State.Jump:
			if (!isJump) {
				inputXPrevJump = input.x;
				velocity.y = jumpHeight;
				velocity.x = inputXPrevJump * moveSpeed * 1.2f;
				isJump = true;
			} else {
				velocity.x = inputXPrevJump * moveSpeed * 1.2f;
			}
			break;
		case State.Fall:
			velocity.x = inputXPrevJump * moveSpeed * 1.2f;
			isJump = false;
			DeleteFromStateQueue (GetStatePriorityLevel (State.Jump));
			DeleteFromStateQueue (GetStatePriorityLevel (State.Fall));
			break;
		case State.Landing:
			if (landingDelayTimer <= landingDelay) {
				landingDelayTimer += Time.deltaTime;
				velocity.x = 0;
			} else {
				landingDelayTimer = 0;
				ReleaseLanding ();
			}
			break;
		case State.GrabCorner:
			if (Mathf.Sign (transform.position.x - grappingObj.transform.position.x) == Mathf.Sign(transform.localScale.x))
			{
				isGrabing = false;
				DeleteFromStateQueue (GetStatePriorityLevel (State.GrabCorner));
				break;
			}
			isGrabing = true;
			velocity = Vector3.zero;

			var newPos = transform.position;
			newPos.x = (transform.localScale.x > 0) ? grappingObj.bounds.min.x - pBoxCollider.size.x * 0.5f : grappingObj.bounds.max.x + pBoxCollider.size.x * 0.5f;
			newPos.y = grappingObj.bounds.max.y - pBoxCollider.size.y;
			transform.position = newPos;
			break;
		case State.ClimbCorner:
			ClimbCorner ();
			break;
		case State.Attack:
			if (IsPlayerInPast())
			{
				GameSceneManager.getInstance.AddElasticityGauge(100, UndoAttack);
			}
			else
			{ 
				// 위치 맞춤
				if (anim.isLeft())
					transform.position = new Vector3(enemyScript.transform.position.x + 0.1f, enemyScript.transform.position.y, transform.position.z);
				else
					transform.position = new Vector3(enemyScript.transform.position.x - 0.1f, enemyScript.transform.position.y, transform.position.z);
					enemyScript.enemyState = global::State.Stun;
					velocity = Vector2.zero;
			}
			break;
		case State.Dizzy:

			break;
		case State.Damaged:

			break;
		case State.Dead:

			break;
		}
	}

    // 어느쪽에서 데미지를 받는지 결정함
    public void Damaged (float dAmount, bool isFromLeft)
    {
        if (state == State.Dead)
            return;

        hp -= dAmount;
        if (hp <= 0)
        {
            Dead (isFromLeft);
            return;
        }
    }

    public void Dead (bool isFromLeft)
    {
        state = State.Dead;
        anim.SetDir (isFromLeft);
        anim.SetDie ();
        GameSceneManager.getInstance.ReplayCurrentScene ();
    }

	private void ProcessCustomEventTrigger (Collider col)
	{
		if (IsCutsomEventTrigger (col)) {
			col.GetComponent<CustomEventTrigger> ().StartTriggerEvent ();
		}
	}

    public void ProcessGround ()
    {
        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
		
			isAir = false;
			velocity.x = 0;
        }
        else
        {
            isAir = true;
        }
    }

    private void UndoAttack ()
    {
        if (isLeft ())
            transform.position = new Vector3(transform.position.x + 0.1f, transform.position.y, transform.position.z);
        else
            transform.position = new Vector3(transform.position.x - 0.1f, transform.position.y, transform.position.z);
        state = State.Idle;
        velocity = Vector2.zero;
    }

	EnemyScript enemyScript;

	public void CheckAndAddAttackState ()
	{
		if (!isAir && Input.GetKeyDown (KeyCode.E))
		{
			enemyScript = interacitveManager.FindNearestEnemy();
			if (interacitveManager.FindNearestEnemy () != null) {
				if (enemyScript.enemyState != global::State.Stun &&
				    Mathf.Sign (enemyScript.transform.localScale.x) == Mathf.Sign (transform.localScale.x)) {
					Debug.Log ("Add Attack");
					AddToStateQueueWithCheckingOverlap (GetStatePriorityLevel (State.Attack));
				}
			}
		}
	}

    public void ReleaseAttack()
    {
		DeleteFromStateQueue (GetStatePriorityLevel (State.Attack));
    }


	bool IsPlayerInPast ()
	{
		return pTimeLayer.layerNum == 0;
	}

	void SetGrayScreen ()
	{
		mainCamera.GetComponent<GrayScaleEffect> ().IsActived = true;
	}

	void SetColorScreen ()
	{
		mainCamera.GetComponent<GrayScaleEffect> ().IsActived = false;
	}

    void ProcessTimeSwitching()
    {
        // 다른 레이어의 통과 불가능한 지형들과 플레이어가 충돌한다면
        // 타임 스위칭을 할 수 없다
        if (Input.GetKeyDown(KeyCode.Tab))
        {
			if (IsPlayerInPast ()) {
				mainCamera.GetComponent<FollowUpCamera> ().DoElasticEffect ();
			}
            DoTimeSwitch();
        }

		if (IsPlayerInPast ()) {
			SetGrayScreen ();
		} else {
			SetColorScreen ();
		}
    }

    public void DoTimeSwitch()
    {
        int toLayer = OppositeLayer(pTimeLayer.layerNum);
        if (CanSwitchingTime(toLayer))
        {
            TimeLayerManager.GetInstance.MoveObjectToLayer(gameObject, toLayer);
            pTimeLayer = transform.GetComponentInParent<TimeLayer>();
            controller.pTimeLayer = pTimeLayer;
        }
    }

    public bool CanSwitchingTime(int toLayer)
    {
		if (isWalkOnStair)
			return false;
        bool canSwitching = true;

        var bc = GetComponent<BoxCollider>();
        var colPos = transform.position + new Vector3(bc.center.x, bc.center.y, 0);
        
        var cols = Physics.OverlapBox(colPos, 
            new Vector3(bc.size.x * 0.8f, bc.size.y * 0.8f, bc.size.z), 
            Quaternion.identity, 
            1 << LayerMask.NameToLayer("Collision"));

        for (int i = 0; i < cols.Length; i++)
        {
            var c = cols[i];

            if (c.GetComponentInParent<TimeLayer>().layerNum == toLayer)
            {
                canSwitching = false;
            }
        }

        return canSwitching;
    }

    public int OppositeLayer(int nowLayer)
    {
        return pTimeLayer.layerNum == 0 ? 1 : 0;
    }

    // isGrabing이 true라면 플레이어의 위치를 벽 코너에 고정한다
    public bool isGrabing;

	public LayerMask grabableLayer;
	public void CheckAndAddGrabCornerState ()
	{
		if (isAir && !isClimb)
		{
			var cols = Physics.OverlapBox (transform.position, new Vector3 (0.20f, 0.35f,0.1f), Quaternion.identity, grabableLayer);
			for (int i = 0; i < cols.Length; i++)
			{
				var c = cols [i];


				if (!c.tag.Contains ("Grabable"))
				{
					continue;
				}

				if (TimeLayer.EqualTimeLayer(pTimeLayer, c.transform.GetComponentInParent<TimeLayer>()))
				{
					if (pBoxCollider.bounds.min.y < c.bounds.max.y && 
						pBoxCollider.bounds.max.y >= c.bounds.max.y - 0.1f)
					{
						grappingObj = c;
						AddToStateQueueWithCheckingOverlap (GetStatePriorityLevel (State.GrabCorner));
					}
				}
			}
		}
	}

	public void CheckAndAddClimbCornerState ()
	{
		if (isGrabing && Input.GetKeyDown (KeyCode.Space))
		{
			AddToStateQueueWithCheckingOverlap (GetStatePriorityLevel(State.ClimbCorner));
		}
	}

    public bool isOnLadder = false;

    float lMaxX, lMaxY;
    float lMinX, lMinY;

    private void ProcessClimbLadder()
    {
        if (Input.GetKey(KeyCode.W) && state != State.Attack)
        {
            var cols = Physics.OverlapBox(transform.position,
                new Vector3(pBoxCollider.bounds.extents.x, pBoxCollider.bounds.extents.y, pBoxCollider.bounds.extents.z),
                Quaternion.identity,
                1 << LayerMask.NameToLayer("Trigger"));


            for (int i = 0; i < cols.Length; i++)
            {
                var col = cols[i];
                if (col.CompareTag("Ladder"))
                {
                    lMaxX = col.bounds.max.x;
                    lMaxY = col.bounds.max.y;
                    lMinX = col.bounds.min.x;
                    lMinY = col.bounds.min.y;

                    if (pBoxCollider.bounds.min.y >= lMaxY)
                    {
                        isOnLadder = false;
                        velocity = Vector3.zero;
                        break;
                    }

                    if (!isOnLadder)
                        isOnLadder = true;
                    //state = State.ClimbLadder;
					var newPos = transform.position + Vector3.up * moveSpeed * Time.deltaTime;
					newPos.x = col.bounds.center.x;
					transform.position = newPos;
                }
            }
        }
        else if (Input.GetKey(KeyCode.S) && state != State.Attack)
        {
            var cols = Physics.OverlapBox(transform.position,
                new Vector3(pBoxCollider.bounds.extents.x, pBoxCollider.bounds.extents.y, pBoxCollider.bounds.extents.z),
                Quaternion.identity,
                1 << LayerMask.NameToLayer("Trigger"));

            for (int i = 0; i < cols.Length; i++)
            {
                var col = cols[i];
                if (col.CompareTag("Ladder"))
                {
                    lMaxX = col.bounds.max.x;
                    lMaxY = col.bounds.max.y;
                    lMinX = col.bounds.min.x;
                    lMinY = col.bounds.min.y;

                    if (pBoxCollider.bounds.min.y <= lMinY)
                    {
                        velocity = Vector3.zero;
                        isOnLadder = false;
                        break;
                    }
                    if (!isOnLadder)
                        isOnLadder = true;
//                    state = State.ClimbLadder;
					var newPos = transform.position - Vector3.up * moveSpeed * Time.deltaTime;
					newPos.x = col.bounds.center.x;
					transform.position = newPos;

					transform.position = newPos;
                }
            }
        }
    }
		
    private void ProcessEnterStair(Collider col)
    {
        if (col.CompareTag("Stair"))
        {
            if (Input.GetKeyDown(KeyCode.W) &&
                !isWalkOnStair &&
                TimeLayer.EqualTimeLayer(pTimeLayer, col.GetComponentInParent<TimeLayer>()))
            {
                EnterStair(col);
                stairColliders = col.transform.parent.GetComponentsInChildren<BoxCollider>();
                InitStairZone();
            }
        }
    }

    private void ProcessExitStair(Collider col)
    {
        if (isWalkOnStair)
        {
            var tmpPos = transform.position;
            if (transform.position.y > stairMaxY)
            {
                tmpPos.y = stairMaxY;
                transform.position = tmpPos;
            }
            else if (transform.position.y <= stairMinY)
            {
                tmpPos.y = stairMinY;
                transform.position = tmpPos;
            }

            if (transform.position.x > stairMaxX)
            {
                ExitStair(col);
            }
            else if (transform.position.x <= stairMinX)
            {
                ExitStair(col);
            }
        }
    }

    bool IsVerticalCollision()
    {
        for (int i = 0; i < 3; i++)
        {
            var hits = Physics.RaycastAll(Vector3.up * pBoxCollider.bounds.max.y + Vector3.right * pBoxCollider.bounds.min.x * (i / 2), Vector2.up, (INIT_COLLIDER_SIZE.y - SIT_COLLIDER_SIZE.y), controller.collisionMask);
            for (int j = 0; j < hits.Length; j++)
            {
                var hit = hits[j];
                if (hit.collider != null &&
                    TimeLayer.EqualTimeLayer(hit.collider.GetComponentInParent<TimeLayer>(), pTimeLayer))
				{
                    return true;
                }
            }
        }
        return false;
    }

	public float GetDistanceToGround()
	{
		for (int i = 0; i < 3; i++)
		{
			var origin = Vector3.up * pBoxCollider.bounds.min.y + Vector3.right * pBoxCollider.bounds.min.x + Vector3.right * pBoxCollider.size.x * (i / 2);
			Debug.DrawLine (Vector3.up * pBoxCollider.bounds.min.y + Vector3.right * pBoxCollider.bounds.min.x + Vector3.right * pBoxCollider.size.x * (i / 2),
				Vector3.up * pBoxCollider.bounds.min.y + Vector3.right * pBoxCollider.bounds.min.x + Vector3.right * pBoxCollider.size.x * (i / 2) + Vector3.down * 3,
				Color.red);
			var hits = Physics.RaycastAll(origin, Vector2.down, 3, controller.collisionMask);
			for (int j = 0; j < hits.Length; j++)
			{
				var hit = hits[j];
				if (hit.collider != null &&
					TimeLayer.EqualTimeLayer(hit.collider.GetComponentInParent<TimeLayer>(), pTimeLayer) ||
					hit.collider.tag.Contains("Ground"))
				{
					return hit.distance;
				}
			}
		}
		return -1;
	}

    Collider grappingObj;

    private bool isClimb = false;
    //만족 스러운 Duration 찾은 후 private나 const로 지정
    public float climbDuration;
    private float climbDurationTimer = 0;

    //t는 tmp의 약자로 함수 실행마다 재정의됨
    private float tClimbHeight = 0;
    private float tClimbSpeed = 0;

    //만족 스러운 Duration 찾은 후 private나 const로 지정
    public float climbDelayDuration;

    private void ResetClimbInfo()
    {
        isClimb = false;
        isGrabing = false;

        climbDurationTimer = 0;
        tClimbHeight = 0;
        tClimbSpeed = 0;

		DeleteFromStateQueue (GetStatePriorityLevel (State.GrabCorner));
		DeleteFromStateQueue (GetStatePriorityLevel (State.ClimbCorner));
    }

    private void InitClimbInfo()
    {
        isClimb = true;
        tClimbHeight = pBoxCollider.bounds.max.y - pBoxCollider.bounds.min.y;
        tClimbSpeed = tClimbHeight / climbDuration;
    }

    private BoxCollider[] stairColliders;
    private float stairMaxX, stairMinX;
    private float stairMaxY, stairMinY;

    private void InitStairZone()
    {
        List<BoxCollider> stairs = new List<BoxCollider>();
        for (int i = 0; i < stairColliders.Length; i++)
        {
            if (stairColliders[i].CompareTag("Stair"))
            {
                stairs.Add(stairColliders[i]);
            }
        }

        stairMaxX = stairs[0].transform.position.x + stairs[0].bounds.extents.x;
        stairMaxY = stairs[0].transform.position.y + stairs[0].bounds.extents.y;

        stairMinX = stairs[0].transform.position.x - stairs[0].bounds.extents.x;
        stairMinY = stairs[0].transform.position.y - stairs[0].bounds.extents.y;

        for (int i = 1; i < stairs.Count; i++)
        {
            stairMaxX = Mathf.Max(stairMaxX, stairs[i].transform.position.x + stairs[i].bounds.extents.x);
            stairMaxY = Mathf.Max(stairMaxY, stairs[i].transform.position.y + stairs[i].bounds.extents.y);

            stairMinX = Mathf.Min(stairMinX, stairs[i].transform.position.x - stairs[i].bounds.extents.x);
            stairMinY = Mathf.Min(stairMinY, stairs[i].transform.position.y - stairs[i].bounds.extents.y);
        }
    }

    private void ClimbCorner()
    {
        if (isGrabing && !isClimb)
        {
            InitClimbInfo();
        }
        if (isClimb)
        {
            if (climbDurationTimer <= climbDuration)
            {
                climbDurationTimer += Time.deltaTime;

				transform.position += (Vector3.up * tClimbSpeed) * Time.deltaTime;
            }
            else
            {
                ResetClimbInfo();
                velocity = Vector3.zero;

                var newPos = transform.position;
				newPos.x += (grappingObj.transform.position - transform.position).normalized.x * (pBoxCollider.bounds.extents.x);
                newPos.y = grappingObj.bounds.max.y;
				state = State.Idle;
                transform.position = newPos;
            }
        }
    }

    public void ChangePlayerZLayer(Vector3 newPos)
    {
        prevZPos = transform.position.z;
        transform.position = newPos;
    }

    private void EnterStair(Collider col)
    {
        isWalkOnStair = true;
        var newPos = col.transform.position;
        newPos.x = transform.position.x;
        ChangePlayerZLayer(newPos);
    }

    private void ExitStair(Collider col)
    {
        isWalkOnStair = false;
        var prevPos = transform.position;
        prevPos.z = prevZPos;
        ChangePlayerZLayer(prevPos);
    }

	private bool IsCutsomEventTrigger (Collider col)
	{
		return null != col.GetComponent<CustomEventTrigger> () && TimeLayer.EqualTimeLayer(gameObject, col.gameObject);
	}
    
    private void OnTriggerEnter(Collider col)
    {
		if (enabled) {
			ProcessCustomEventTrigger (col);
		}
    }

    private void OnTriggerStay(Collider col)
    {
        if (enabled)
        {
            ProcessEnterStair(col);
            ProcessExitStair(col);
        }
    }

    private void OnTriggerExit(Collider col)
    {
        ProcessExitStair(col);
    }

	public void ReleaseLanding()
	{
		DeleteFromStateQueue (GetStatePriorityLevel (State.Jump));
		DeleteFromStateQueue (GetStatePriorityLevel (State.Fall));
		DeleteFromStateQueue (GetStatePriorityLevel (State.Landing));
		isJump = false;
	}

    void Update()
    {
		ProcessGround ();
		ProcessTimeSwitching ();
		input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

		velocity.y += gravity * Time.deltaTime;

		distanceToGround = GetDistanceToGround ();

		CheckAndAddIdleState ();
		CheckAndAddRunState ();
		CheckAndAddJumpState ();
		CheckAndAddFallState ();
		CheckAndAddLandingState ();
		CheckAndAddGrabCornerState ();
		CheckAndAddClimbCornerState ();
		CheckAndAddAttackState ();

		UpdateAndApplyPlayerState ();

		if (velocity.x != 0)
			anim.SetDir (Mathf.Sign(velocity.x) < 0);

        controller.Move(velocity * Time.deltaTime);
		velocity.x = 0;
    }
}