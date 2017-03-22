using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : IState
{
	public float holdDuration;
	private float holdDurationTimer=0;

    public PatrolState(GameObject obj)
    {
        enemyObj = obj;
    }

    public override void OnStateEnter(GameObject obj){
		holdDuration = obj.GetComponent<EnemyScript> ().holdDuration;
	}

	public override void OnStateStay(GameObject obj){
		holdDurationTimer += Time.deltaTime;

		Hold ();

		if(holdDurationTimer>=holdDuration){
			holdDurationTimer = 0;
			Walk (obj.transform);
		}
	}

	public override void OnStateExit(GameObject obj){
		
	}

	public override EnemyState ChangeState(GameObject obj){
		return EnemyState.Patrol;	
	}
}
