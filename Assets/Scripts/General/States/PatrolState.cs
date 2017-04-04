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

		OnStateEnter ();
	}

    public override void OnStateEnter(){
		holdDuration = enemyObj.GetComponent<EnemyScript> ().holdDuration;
		walkDuration = enemyObj.GetComponent<EnemyScript> ().walkDuration;
		enemyScript.AimToForward ();
	}

	public override void OnStateStay(){
		if (walkDurationTimer >= walkDuration) {
			holdDurationTimer += Time.deltaTime;
			Hold ();
			if (holdDurationTimer >= holdDuration) {
				dir.x = -dir.x;
				enemyObj.transform.localScale = new Vector3 (Mathf.Sign (dir.x) * Mathf.Abs(enemyObj.transform.localScale.x), enemyObj.transform.localScale.y,enemyObj.transform.localScale.z);
				holdDurationTimer = 0;
				walkDurationTimer = 0;
			}
		} else {
			walkDurationTimer += Time.deltaTime;
	
			var bInfo = enemyScript.Browse (speed*Time.deltaTime);
			if (bInfo.layer!=LayerMask.NameToLayer("Collision")) {
				Walk (enemyObj.transform, dir * speed);
			} else {
				Hold ();
			}
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
