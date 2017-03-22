using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : IState
{
	public float holdDuration;
	private float holdDurationTimer=0;

	public float walkDuration;
	private float walkDurationTimer = 0;

	private Vector3 dir;
	private float speed;

	public PatrolState (GameObject obj) : base (obj){}

	public void InitPatrolInfo(Vector3 d, float s){
		dir = d;
		speed = s;
	}

    public override void OnStateEnter(GameObject obj){
		holdDuration = obj.GetComponent<EnemyScript> ().holdDuration;
		walkDuration = obj.GetComponent<EnemyScript> ().walkDuration;
	}

	public override void OnStateStay(GameObject obj){
		if (walkDurationTimer >= walkDuration) {
			holdDurationTimer += Time.deltaTime;
			Hold ();

			if (holdDurationTimer >= holdDuration) {
				dir.x = -dir.x;
				holdDurationTimer = 0;
				walkDurationTimer = 0;
			}
		} else {
			walkDurationTimer += Time.deltaTime;
			Walk (obj.transform,dir*speed);
		}
	}

	public override void OnStateExit(GameObject obj){
		
	}
	//현재 State가 Patrol인지를 따진 후, Stay or Enter를 호출한다.
	public override EnemyState ChangeState(EnemyState nowState){
		if (nowState == EnemyState.Patrol) {
			OnStateStay (enemyObj);
		} else {
			if(CheckState(enemyObj))
				OnStateEnter (enemyObj);
		}
		return EnemyState.Patrol;	
	}
	public override bool CheckState (GameObject obj){
		return true;
	}
}
