using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Security : EnemyScript {
	public float stateChange = 2;
	private float stateChangeTimer = 0;

	void Awake()
    {
		base.Awake ();
		Debug.Log ("In Child");

		anim = GetComponent<AnimationBase> ();
        // TODO!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        // 부모, 자식의 생성자 호출에 대해서 더 공부해야할것
        // 만약 이게 깔끔하게 된다면 gameobject를 여기서만 보내주고 두고두고 사용할 수 있음
		istate = new IState[6];
		istate [0] = new IdleState (gameObject);
		istate [1] = new PatrolState (gameObject);
		istate [2] = new SuspiciousState (gameObject);
		istate [3] = new DetectionState (gameObject);
		istate [4] = new AlertState (gameObject);
		istate [5] = new AttackState (gameObject);

		InitEnemy ();
	}
	BrowseInfo isFindPlayer;
    // Update is called once per frame
    void Update()
    {
		if (GetSpecifiedState<DetectionState> (State.Detection).isDetection) {
			if (isFindPlayer.layer != LayerMask.NameToLayer ("Player")) {
				detectionDurationTimer += Time.deltaTime;
				transform.localScale = new Vector3 (Mathf.Sign ((playerObject.transform.position.x - transform.position.x)) * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
				if(detectionDurationTimer>=detectionDuration){
					if(isAttack){
						GetSpecifiedState<AttackState> (State.Attack).OnStateExit ();
					}
					GetSpecifiedState<DetectionState> (State.Detection).isDetection = false;
					GetSpecifiedState<PatrolState> (State.Patrol).InitPatrolInfo (patrolDir, moveSpeed);
					SetState (State.Patrol);
					detectionDurationTimer = 0;
				}
			}
		}

        switch (enemyState)
        {
           case State.Idle:
            if (stateChangeTimer <= stateChange)
            {
                    // 함수 내부에서 형변환을 해주어서 조금 더 깔끔하게 짜기 위해 함수를 구현함
                    // 단 성능이 더 중요하다면 함수를 호출하는 overload를 줄이기 위해 함수를 사용하지 않아도 됨
				enemyState =  GetState (State.Idle).ChangeState (enemyState);
				stateChangeTimer += Time.deltaTime;    
			}
            else
            {
				if (transitionDurationTimer >= transitionDuration) {
					//Patrol State로 enemyState를 바꾸는 구문이다.
					GetState (State.Idle).OnStateExit ();
					SetState (State.Patrol);
					transitionDurationTimer=0;
					GetSpecifiedState<PatrolState>(State.Patrol).InitPatrolInfo (patrolDir, moveSpeed);
				} else {
					transitionDurationTimer += Time.deltaTime;
				}
            }
        	break;
		case State.Patrol:
			if (isFindPlayer.hittedObj != null) {
				if (isFindPlayer.layer == LayerMask.NameToLayer ("Player")) {
					if (transitionDurationTimer >= transitionDuration) {
						SetState (State.Suspicious);
						transitionDurationTimer = 0;
						GetSpecifiedState<SuspiciousState> (State.Suspicious).InitSuspiciousInfo (playerObject.transform.position, moveSpeed);
					} else {
						transitionDurationTimer += Time.deltaTime;
						anim.setAnimation (0, "Rifle_Suspicious_Idle", true, 1);
					}
				} else {
					SetState (State.Patrol);
					isFindPlayer = Browse (findOutSight);
					findOutGauge = Mathf.Lerp (findOutGauge, 0, Time.deltaTime * findOutGaugeIncrement * 0.1f);
				}
			} else {
				isFindPlayer = Browse (findOutSight);
				SetState (State.Patrol);
			}
            break;
		case State.Suspicious:
			//When findOuGauge larger then 100 => Change state to Detection
			if (findOutGauge >= 100) {
				if (transitionDurationTimer >= transitionDuration) {
					SetState (State.Detection);  //then isDetection in IState[3] will be true 
					transitionDurationTimer = 0;
				} else {
					transitionDurationTimer += Time.deltaTime;
					anim.setAnimation (0, "Rifle_Idle", true, 1);
				}
				findOutGauge = 100;
				return;
			}
			//When findOutGauge smaller then 100 then => Play Suspicious with playing OnStateStay
			SetState (State.Suspicious);

			//TODO : Make animation to 살금살금 걷기 but now 'Walk' <= Cleard
			anim.setAnimation (0, "Rifle_Suspicious_Walk", true, 1);

			//Detect Object and Player Browse() return is distance between player and this enemy
			//if can't find player then return -1
			isFindPlayer = Browse (1);

			//if find out player then play below statement
			if(isFindPlayer.layer == LayerMask.NameToLayer("Player")){
				findOutGauge += (findOutGaugeIncrement / isFindPlayer.distance) * Time.deltaTime*4;
				GetSpecifiedState<SuspiciousState>(State.Suspicious).InitSuspiciousInfo (playerObject.transform.position, moveSpeed);
			} 
			else {
				if(GetSpecifiedState<SuspiciousState>(State.Suspicious).CheckArrive()){
					Debug.Log ("Can't Find Player");
					if (transitionDurationTimer >= transitionDuration) {
						SetState(State.Patrol);  //then isDetection in IState[3] will be true 
						GetSpecifiedState<PatrolState>(State.Patrol).InitPatrolInfo (patrolDir, moveSpeed);
						transitionDurationTimer = 0;
					} else {
						transitionDurationTimer += Time.deltaTime*0.25f;
						anim.setAnimation (0, "Rifle_Idle", true, 1);
					}
				}
			}
			break;
		case State.Detection:
			//below statement will make this enemy state to Alert or Attack
			if (!GetSpecifiedState<DetectionState> (State.Detection).isDetection) {
				SetState (State.Detection);
				transitionDurationTimer = 0;
			}
			else {
				if (canAlert) {
					GetSpecifiedState<AlertState> (State.Alert).InitAlertInfo (1);
					SetState (State.Alert);
				}
			}
			break;
		case State.Alert:
			SetState (State.Alert);
			if (transitionDurationTimer >= transitionDuration) {
				GetSpecifiedState<AttackState> (State.Attack).InitAttackInfo (EnemyAttackType.Gun,bullet,3);
				SetState(State.Attack);  //then isDetection in IState[3] will be true 
				transitionDurationTimer = 0;
			} else {
				detectionDurationTimer = 0;
				transitionDurationTimer += Time.deltaTime;
				anim.setAnimation (0, "Rifle_Attach", true, 0.5f);
			}
			break;
		case State.Attack:
			isFindPlayer = Browse (findOutSight);
			SetState (State.Attack);
			break;
		case State.Escape:

			break;
		case State.Sturn:

			break;
		case State.Dead:

			break;
		case State.Sit:

			break;
        }
    }
}
