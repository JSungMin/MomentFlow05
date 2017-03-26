using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuspiciousState : IState {

	public Vector3 targetPos;

	public SuspiciousState (GameObject obj):base(obj){}

	public void InitSuspiciousInfo(Vector3 t,float s){
		targetPos = t;
		speed = s;
		CalculateDirection ();
	}
	public void CalculateDirection(){
		dir = (targetPos - enemyObj.transform.position).normalized;
		dir.y = 0;
		dir.x = Mathf.Sign (dir.x) * 1;
	}

	public bool CheckArrive(){
		if (Vector3.Distance (enemyObj.transform.position, targetPos) >= 0.025f) {
			return false;
		}
		return true;
	}

	public override void OnStateEnter(){
	}

	public override void OnStateStay(){
		if(!CheckArrive()){
			Walk (enemyObj.transform, dir * speed);
		}
	}

	public override void OnStateExit(){

	}
	//현재 State가 Patrol인지를 따진 후, Stay or Enter를 호출한다.
	public override State ChangeState(State nowState){
		if (nowState == State.Suspicious) {
			OnStateStay ();
		} else {
			if(CheckState())
				OnStateEnter ();
		}
		return State.Suspicious;	
	}
	public override bool CheckState (){
		return true;
	}
}
