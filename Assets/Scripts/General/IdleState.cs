using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class IdleState : IState {

	public IdleState(GameObject obj):base(obj){}
    
	public override void OnStateEnter(GameObject obj){
		Idle ();
	}
	public override void OnStateStay(GameObject obj){
		Idle ();
	}
	public override void OnStateExit(GameObject obj){
		Idle ();
	}
	public override State ChangeState(State nowState){
		if (nowState == State.Patrol) {
			OnStateStay (enemyObj);
		} else {
			if(CheckState(enemyObj))
				OnStateEnter (enemyObj);
		}
		return State.Idle;	
	}
	public override bool CheckState (GameObject obj){
		return true;
	}
}
