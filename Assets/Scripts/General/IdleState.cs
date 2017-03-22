using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class IdleState : IState {

	public IdleState(GameObject obj):base(obj){}
    
	public override void OnStateEnter(GameObject obj){

	}
	public override void OnStateStay(GameObject obj){

	}
	public override void OnStateExit(GameObject obj){

	}
	public override EnemyState ChangeState(EnemyState nowState){
		if (nowState == EnemyState.Patrol) {
			OnStateStay (enemyObj);
		} else {
			if(CheckState(enemyObj))
				OnStateEnter (enemyObj);
		}
		return EnemyState.Idle;	
	}
	public override bool CheckState (GameObject obj){
		return true;
	}
}
