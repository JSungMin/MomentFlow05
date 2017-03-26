using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : IState
{
	public float holdDuration;
	private float holdDurationTimer=0;

	public float walkDuration;
	private float walkDurationTimer = 0;

	public PatrolState (GameObject obj) : base (obj){}

	public void InitPatrolInfo(Vector3 d, float s){
		dir = d;
		speed = s;
	}

    public override void OnStateEnter(){
		holdDuration = enemyObj.GetComponent<EnemyScript> ().holdDuration;
		walkDuration = enemyObj.GetComponent<EnemyScript> ().walkDuration;
	}

	public override void OnStateStay(){
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
			Walk (enemyObj.transform,dir*speed);
		}
	}

	public override void OnStateExit(){
		
	}
	//현재 State가 Patrol인지를 따진 후, Stay or Enter를 호출한다.
	public override State ChangeState(State nowState){
		if (nowState == State.Patrol) {
			OnStateStay ();
		} else {
			if(CheckState())
				OnStateEnter ();
		}
		return State.Patrol;	
	}
	public override bool CheckState (){
		return true;
	}
}
