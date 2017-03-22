using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class IdleState : IState {

    public IdleState(GameObject obj)
    {
        enemyObj = obj;
    }

    void Awake()
    {

    }
    
	public override void OnStateEnter(GameObject obj){

	}
	public override void OnStateStay(GameObject obj){

	}
	public override void OnStateExit(GameObject obj){

	}
	public override EnemyState ChangeState(GameObject obj){
		if (obj.transform.position.x >= obj.GetComponent<EnemyScript> ().playerObject.transform.position.x)
			return EnemyState.Patrol;
		return EnemyState.Idle;
	}
}
