using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class IdleState : IState {
	public override void OnStateEnter(GameObject obj){

	}
	public override void OnStateStay(GameObject obj){

	}
	public override void OnStateExit(GameObject obj){

	}
	public override EnemyScript.EnemyState ChangeState(GameObject obj){
		if (obj.transform.position.x >= obj.GetComponent<EnemyScript> ().playerObject.transform.position.x)
			return EnemyScript.EnemyState.Patrol;
		return EnemyScript.EnemyState.Idle;
	}
}
