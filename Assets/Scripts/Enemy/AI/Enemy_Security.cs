using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Spine;
using Spine.Unity;
using Spine.Unity.Modules;

public class Enemy_Security : EnemyScript
{
    public float stateChange = 2;
    private float stateChangeTimer = 0;

    //public Dictionary<int, State> levelValue = new Dictionary<int, State>();
    public State[] levelValue = new State[8];

    private void InitStates()
    {
        istate = new IState[8];
        istate[0] = new IdleState(gameObject);
        istate[1] = new PatrolState(gameObject);
        istate[2] = new SuspiciousState(gameObject);
        istate[3] = new DetectionState(gameObject);
        istate[4] = new AlertState(gameObject);
        istate[5] = new AttackState(gameObject);
        istate[6] = new EscapeState(gameObject);
        istate[7] = new StunState(gameObject);
    }
    private void SetStatesLevel()
    {
        levelValue[0] = State.Stun;
        levelValue[1] = State.Escape;
        levelValue[2] = State.Attack;
        levelValue[3] = State.Alert;
        levelValue[4] = State.Detection;
        levelValue[5] = State.Suspicious;
        levelValue[6] = State.Patrol;
        levelValue[7] = State.Idle;
    }

    public int GetStateLayerKey(State s)
    {
        for (int i = 0; i < levelValue.Length; i++)
        {
            if (levelValue[i] == s)
            {
                return i;
            }
        }
        return -1;
    }

    private new void Awake()
    {
        base.Awake();
        pTimeLayer = transform.GetComponentInParent<TimeLayer>();
        Debug.Log("Enemy Name : " + gameObject.name + "  Layer : " + pTimeLayer.transform.name);
        anim = GetComponent<AnimationBase>();

        InitStates();
        SetStatesLevel();
        AddStateToListWithCheckingOverlap(GetStateLayerKey(defaultState));
        InitEnemy();
    }

    private void ProcessTimeEffect()
    {
        if (TimeLayer.EqualTimeLayer(playerObject.ParentTimeLayer, pTimeLayer))
        {
            ((SecurityAnim)anim).PresentColor();
            GetComponentInChildren<SkeletonGhost>().ghostingEnabled = false;
        }
        else
        {
            ((SecurityAnim)anim).PastColor();
            GetComponentInChildren<SkeletonGhost>().ghostingEnabled = true;
        }
    }

    private void AutoReleaseDetectionState()
    {
        if (GetSpecifiedState<DetectionState>(State.Detection).isDetection)
        {

            if (!IsFindPlayer(findOutSight))
            {
                detectionDurationTimer += Time.deltaTime;
                transform.localScale = new Vector3(Mathf.Sign((playerObject.transform.position.x - transform.position.x)) * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                if (detectionDurationTimer >= detectionDuration)
                {
                    GetSpecifiedState<DetectionState>(State.Detection).isDetection = false;
                    DeleteStateToList(GetStateLayerKey(State.Detection));
                    detectionDurationTimer = 0;
                }
            }
        }
    }

    private void AutoReleasePatrolState()
    {
        if (patrolDurationTimer >= patrolDuration)
        {
            patrolDurationTimer = 0;
            DeleteStateToList(GetStateLayerKey(State.Patrol));
            SetDefaultState();
            InitToTransition();
        }
    }

    private void EnemyLostPlayerByDifferentTimeLayer()
    {
        if (!TimeLayer.EqualTimeLayer(playerObject.ParentTimeLayer.gameObject, pTimeLayer.gameObject))
        {
            if (!isAttack)
            {
                DeleteStateToList(GetStateLayerKey(State.Attack));
                DeleteStateToList(GetStateLayerKey(State.Detection));
                GetSpecifiedState<DetectionState>(State.Detection).isDetection = false;

                detectionDurationTimer = 0;
            }
            //findOutGauge = 99;
        }
    }

    private void IncreaseFindOutGauge()
    {
        if (IsFindPlayer(findOutSight))
        {
            var pPos = playerObject.transform.position;
            pPos.z = 0;
            var ePos = transform.position;
            ePos.z = 0;
            var dist = Vector3.Distance(pPos, ePos);

            switch (enemyState)
            {
                case State.Suspicious:
                    findOutGauge = Mathf.Clamp(findOutGauge + Time.deltaTime * findOutGaugeIncrement * (findOutSight / dist) * 5, 0, 100);
                    break;
            }
        }
    }

    private void DecreaseFindOutGauge()
    {
        if (!IsFindPlayer(findOutSight) && !GetSpecifiedState<DetectionState>(State.Detection).isDetection)
        {
            var pPos = playerObject.transform.position;
            pPos.z = 0;
            var ePos = transform.position;
            ePos.z = 0;
            var dist = Vector3.Distance(pPos, ePos);

            findOutGauge = Mathf.Clamp(findOutGauge - Time.deltaTime * findOutGaugeIncrement * (findOutSight / dist), 0, 100);
        }
    }

    private void CheckIdleState()
    {
        if (enemyState == State.Idle)
        {
            if (!IsFindPlayer(findOutSight))
            {
                AddStateToListWithCheckingOverlap(GetStateLayerKey(State.Idle));
            }
            else if (IsFindPlayer(findOutSight))
            {
                AddStateToListWithCheckingOverlap(GetStateLayerKey(State.Suspicious));
                GetSpecifiedState<SuspiciousState>(State.Suspicious).InitSuspiciousInfo(playerObject.transform.position, moveSpeed * 0.5f);
            }
        }
    }

    private void CheckPatrolState()
    {
        if (enemyState == State.Patrol)
        {
            if (IsFindPlayer(findOutSight))
            {
                AddStateToListWithCheckingOverlap(GetStateLayerKey(State.Suspicious));
                GetSpecifiedState<SuspiciousState>(State.Suspicious).InitSuspiciousInfo(playerObject.transform.position, moveSpeed * 0.5f);
            }
            else
            {
                AddStateToListWithCheckingOverlap(GetStateLayerKey(State.Patrol));
            }
        }
    }

    private void CheckSuspiciousState()
    {
        if (enemyState == State.Suspicious)
        {
            if (findOutGauge >= 100)
            {
                AddStateToListWithCheckingOverlap(GetStateLayerKey(State.Detection));
            }
            else if (findOutGauge <= 0)
            {
                AddStateToListWithCheckingOverlap(GetStateLayerKey(State.Patrol));
                DeleteStateToList(GetStateLayerKey(State.Suspicious));
            }
        }
    }

    private void CheckDetectionState()
    {
        if (enemyState == State.Detection)
        {
            AddStateToListWithCheckingOverlap(GetStateLayerKey(State.Attack));
            GetSpecifiedState<AttackState>(State.Attack).InitAttackInfo(EnemyAttackType.Gun, bullet, 3);
        }
    }

    private void CheckAttackState()
    {
        if (enemyState == State.Attack)
        {
            if (!GetSpecifiedState<DetectionState>(State.Detection).isDetection)
                DeleteStateToList(GetStateLayerKey(State.Attack));
        }
    }

    // player가 state를 stun으로 바꿔줘야 함
    private void CheckStunState()
    {
        if(enemyState == State.Stun)
        {
            AddStateToListWithCheckingOverlap(GetStateLayerKey(State.Stun));
        }
    }
    
    private IEnumerator ProcessStateList()
    {
        yield return new WaitForEndOfFrame();
        canUseStateList.Sort();
        SetState(levelValue[canUseStateList[0]]);
        yield return null;
    }

    bool isFindPlayer;
    // Update is called once per frame
    void Update()
    {
        //이 부분 이후에 리펙토링 필요 => Clear
        ProcessTimeEffect();
        //ProcessDead ();
        //Detection 이후 일정 시간이 지나면 isDetection = false And Patrol 상태로 만든다.
        AutoReleaseDetectionState();
        //Patrol 이후 일정 시간이 지나면 State Set to DefaultState
        AutoReleasePatrolState();
        
        if (CheckEscapeConditioin())
        {
            SetState(State.Escape);
        }

        IncreaseFindOutGauge();
        DecreaseFindOutGauge();

        CheckIdleState();
        CheckPatrolState();
        CheckSuspiciousState();
        CheckDetectionState();
        CheckAttackState();
        CheckStunState();

        EnemyLostPlayerByDifferentTimeLayer();

        StartCoroutine(ProcessStateList());

        velocity.y -= 2.8f * Time.deltaTime;
        VerticalCollisions();
        transform.Translate(velocity * Time.deltaTime);
    }
}