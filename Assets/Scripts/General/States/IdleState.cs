using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class IdleState : IState {

	public IdleState(GameObject obj):base(obj){}
    
	public override void OnStateEnter(){
		Idle (enemyScript.charAnimName + "_Idle");
	}
	public override void OnStateStay(){
		Idle (enemyScript.charAnimName + "_Idle");
	}
	public override void OnStateExit(){
		Idle (enemyScript.charAnimName + "_Idle");
	}
	public override State ChangeState(State nowState){
		if (nowState == State.Patrol) {
			OnStateStay ();
		} else {
			if(CheckState())
				OnStateEnter ();
		}
		return State.Idle;	
	}
	public override bool CheckState (){
		return true;
	}
}
