using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MyObject
{
    public TimeLayer pTimeLayer;
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
    private float jumpVelocity;
    private float velocityXSmoothing;

    private float gravity;

    public bool isJump;
    private bool isAir = false;
    private bool isWalkOnStair = false;

    [HideInInspector]
    public Vector2 input;
    float saveDelay = 0;

    float timer;

    public AnimationState animationState;
    private InteractiveManager interacitveManager;

    void Start()
    {
        pTimeLayer = transform.GetComponentInParent<TimeLayer>();
        pBoxCollider = GetComponent<BoxCollider>();
        animator = GetComponent<Animator>();
        controller = GetComponent<Controller2D>();
        interacitveManager = GameObject.FindObjectOfType<InteractiveManager>();

        INIT_COLLIDER_OFFSET = pBoxCollider.center;
        SIT_COLLIDER_OFFSET = INIT_COLLIDER_OFFSET - Vector3.up * 0.08f;
        gravity = -9.8f;
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
    }

    // 어느쪽에서 데미지를 받는지 결정함
    public void Damaged(float dAmount, bool isFromLeft)
    {
        if (state == State.Dead)
            return;

        hp -= dAmount;
        if (hp <= 0)
        {
            Dead(isFromLeft);
            return;
        }
    }

    public void Dead(bool isFromLeft)
    {
        state = State.Dead;
        anim.SetDir(isFromLeft);
        anim.SetDie();
        GameSceneManager.getInstance.ReplayCurrentScene();
    }

    public void ProcessGround()
    {
        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;

            isJump = false;
            isAir = false;
        }
        else
        {
            isAir = true;
        }
    }

    void ProcessMove()
    {
        if (state == State.Attack)
        {
            return;
        }

        input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        timer = input.x == 0 ? 0 : timer + Time.deltaTime;

        velocity.x += input.x * moveSpeed * moveStep.Evaluate(timer);
        velocity.y += gravity * Time.deltaTime;
        
        if (!isOnLadder)
        {
            if (input.x != 0)
            {
                state = State.Walk;
            }
            else
            {
                if (isSit)
                    state = State.Sit;
                else
                    state = State.Idle;
            }
        }
        else
        {
            if (input.x != 0)
            {
                state = State.Walk;
            }
            else
            {
                if (isSit)
                    state = State.Sit;
                else
                    state = State.Idle;
            }
        }
    }

    void ProcessAttack()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            EnemyScript enemyScript = interacitveManager.FindNearestEnemy();
            if (interacitveManager.FindNearestEnemy() != null)
            {
                if (enemyScript.enemyState != global::State.Stun &&
                    Mathf.Sign(enemyScript.transform.localScale.x) == Mathf.Sign(transform.localScale.x))
                {
                    state = State.Attack;
                    enemyScript.enemyState = global::State.Stun;
                    velocity = Vector2.zero;
                }
            }
        }
    }

    public void ReleaseAttack()
    {
        if (state == State.Attack)
            state = State.Idle;
    }

    float jumpSaveDelay = 0;
    void ProcessJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) &&
            !isWalkOnStair &&
            !isOnLadder &&
            state != State.Attack)
        {
            if (!isJump && velocity.y >= gravity * Time.deltaTime * 7.0f)
            {
                velocity.y = jumpHeight;
                isJump = true;
            }
        }
    }

    void ProcessTimeSwitching()
    {
        // 다른 레이어의 통과 불가능한 지형들과 플레이어가 충돌한다면
        // 타임 스위칭을 할 수 없다
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            var newPos = transform.position;
            newPos.z = 0;
            transform.position = newPos;
            var bc = GetComponent<BoxCollider>();
            var colPos = transform.position + new Vector3(bc.center.x, bc.center.y, 0);
            var cols = Physics.OverlapBox(colPos, new Vector3(bc.size.x * 0.8f, bc.size.y * 0.8f, bc.size.z), Quaternion.identity, 1 << LayerMask.NameToLayer("Collision"));

            bool canSwitching = true;

            int toLayer = 0;
            if (pTimeLayer.layerNum == 0)
            {
                toLayer = 1;
            }

            for (int i = 0; i < cols.Length; i++)
            {
                var c = cols[i];
                Debug.Log("Name : " + c.gameObject.name + " toLayer : " + toLayer + " hit : " + c.GetComponentInParent<TimeLayer>().layerNum);
                if (c.GetComponentInParent<TimeLayer>().layerNum == toLayer)
                {
                    Debug.Log("Overlap");
                    canSwitching = false;
                }
            }

            if (canSwitching)
            {
                TimeLayerManager.GetInstance.MoveObjectToLayer(gameObject, toLayer);
                pTimeLayer = transform.GetComponentInParent<TimeLayer>();
                controller.pTimeLayer = pTimeLayer;
            }
        }
        Camera.main.GetComponent<GrayScaleEffect>().intensity = Mathf.Lerp(Camera.main.GetComponent<GrayScaleEffect>().intensity, 1 - pTimeLayer.layerNum, Time.deltaTime * 2);
    }

    // isGrabing이 true라면 플레이어의 위치를 벽 코너에 고정한다
    public bool isGrabing;
    // 벽의 모서리를 잡는 함수
    void ProcessGrabCorner()
    {
        // if 벽 모서리와 닿아 있다면 isGrabing = true
        if (isAir && !isClimb && state != State.Attack)
        {
            Vector2 pos = new Vector2(transform.position.x, transform.position.y);
            var cols = Physics.OverlapBox(pos, new Vector2(0.20f, 0.35f), Quaternion.identity);
            for (int i = 0; i < cols.Length; i++)
            {
                var c = cols[i];
                if (c.gameObject.layer == LayerMask.NameToLayer("Collision") &&
                    (c.CompareTag("GrabableObject") && TimeLayer.EqualTimeLayer(pTimeLayer, c.transform.GetComponentInParent<TimeLayer>())) ||
                    (c.CompareTag("GrabableGround")))
                {
                    bool isFoward = (Mathf.Sign((c.transform.position - transform.position).x) == Mathf.Sign(transform.localScale.x)) ? true : false;

                    if (isFoward)
                    {
                        if (pBoxCollider.bounds.min.y < c.bounds.max.y)
                        {
                            if (pBoxCollider.bounds.max.y >= c.bounds.max.y - 0.1f)
                            {
                                Debug.Log("Grab Corner : " + pBoxCollider.bounds.min.y + " : " + c.bounds.max.y + " , " + pBoxCollider.bounds.max.y + " : " + (c.bounds.max.y - 0.1f));
                                isGrabing = true;
                                state = State.GrabCorner;
                                grappingObj = c;
                                velocity = Vector3.zero;
                                var newPos = transform.position;
                                newPos.y = c.bounds.max.y - pBoxCollider.size.y;
                                transform.position = newPos;
                            }
                        }
                    }
                    else
                    {
                        isGrabing = false;
                        Debug.Log("Is Not Forward");
                        state = State.Idle;
                        grappingObj = null;
                    }
                }
            }
        }
        ClimbCorner();
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
                    state = State.ClimbLadder;
                    transform.position += Vector3.up * moveSpeed * Time.deltaTime;
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
                    state = State.ClimbLadder;
                    transform.position -= Vector3.up * moveSpeed * Time.deltaTime;
                }
            }
        }
    }

    public bool isSit = false;

    void ProcessSit()
    {
        if (Input.GetKey(KeyCode.S) && !isAir)
        {
            isSit = true;
        }
        else
        {
            if (!IsVerticalCollision())
            {
                isSit = false;
            }
        }
        Sit();
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

    void Sit()
    {
        if (isSit)
        {
            pBoxCollider.center = SIT_COLLIDER_OFFSET;
            pBoxCollider.size = SIT_COLLIDER_SIZE;
        }
        else
        {
            pBoxCollider.center = INIT_COLLIDER_OFFSET;
            pBoxCollider.size = INIT_COLLIDER_SIZE;
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
        if (isGrabing && Input.GetKeyDown(KeyCode.Space) && !isClimb)
        {
            InitClimbInfo();
        }
        if (isClimb)
        {
            if (climbDurationTimer <= climbDuration)
            {
                climbDurationTimer += Time.deltaTime;

                transform.position += Vector3.up * tClimbSpeed * Time.deltaTime;
                state = State.ClimbCorner;
            }
            else
            {
                ResetClimbInfo();
                velocity = Vector3.zero;

                var newPos = transform.position;
                newPos.x += (grappingObj.transform.position - transform.position).normalized.x * (pBoxCollider.bounds.extents.x);
                newPos.y = grappingObj.bounds.max.y;
                Debug.Log(newPos.y);
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
    
    private void OnTriggerEnter(Collider col)
    {
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

    void Update()
    {
        if (state == State.Dead)
            return;

        var tmpY = transform.position.y;
        ProcessGround();
        ProcessJump();
        ProcessGrabCorner();
        ProcessClimbLadder();
        ProcessSit();
        ProcessTimeSwitching();
        ProcessMove();
        ProcessAttack();

        if (!isGrabing && !isOnLadder && state != State.Attack)
        {
            controller.Move(velocity * Time.deltaTime);
            velocity.x = 0;
        }
    }
}