using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Security : EnemyScript {
	public float stateChange = 2;
	private float stateChangeTimer = 0;

	public Vector3 patrolDir;
	public float moveSpeed;

	void Awake()
    {
		base.Awake ();
		Debug.Log ("In Child");

		anim = GetComponent<AnimationBase> ();
        // TODO!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        // 부모, 자식의 생성자 호출에 대해서 더 공부해야할것
        // 만약 이게 깔끔하게 된다면 gameobject를 여기서만 보내주고 두고두고 사용할 수 있음
		istate = new IState[3];
		istate [0] = new IdleState (gameObject);
		istate [1] = new PatrolState (gameObject);
		istate [2] = new SuspiciousState (gameObject);
		InitEnemy ();
	}
	float isFindPlayer = -1;
    // Update is called once per frame
    void Update()
    {
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
					GetState (State.Idle).OnStateExit (gameObject);
					enemyState = GetState (State.Patrol).ChangeState (enemyState);
					((PatrolState)GetState (State.Patrol)).InitPatrolInfo (patrolDir, moveSpeed);
					transitionDurationTimer = 0;
				} else {
					transitionDurationTimer += Time.deltaTime;
				}
            }
        	break;
		case State.Patrol:

			if (isFindPlayer != -1) {
				findOutGauge = 50;
				if (transitionDurationTimer >= transitionDuration) {
					enemyState = GetState (State.Suspicious).ChangeState (enemyState);
					((SuspiciousState)GetState (State.Suspicious)).InitSuspiciousInfo (playerObject.transform.position, moveSpeed);
					transitionDurationTimer = 0;
				} else {
					transitionDurationTimer += Time.deltaTime;
					anim.setAnimation (0, "Idle", true, 1);
				}
			} else {
				enemyState = GetState (State.Patrol).ChangeState (enemyState);
				isFindPlayer = Browse ();
				findOutGauge = Mathf.Lerp(findOutGauge,0,Time.deltaTime*findOutGaugeIncrement*0.1f);
			}
            break;
		case State.Suspicious:
			Debug.Log ("In Suspicious");
			enemyState = GetState (State.Suspicious).ChangeState (enemyState);

			isFindPlayer = Browse ();
			if (isFindPlayer != -1) {
				findOutGauge += (findOutGaugeIncrement / isFindPlayer) * Time.deltaTime;
				((SuspiciousState)GetState (State.Suspicious)).InitSuspiciousInfo (playerObject.transform.position, moveSpeed);
				if (findOutGauge >= 100) {
					//enemyState = GetState (State.Detection).ChangeState (enemyState);
				}
			} else {
				if(((SuspiciousState)GetState (State.Suspicious)).CheckArrive()){
					enemyState = GetState (State.Patrol).ChangeState (enemyState);
					((PatrolState)GetState (State.Patrol)).InitPatrolInfo (patrolDir, moveSpeed);
				}
			}

			findOutGauge = Mathf.Clamp (findOutGauge, 0, 100);
			break;
		case State.Detection:

			break;
        }
    }
}
