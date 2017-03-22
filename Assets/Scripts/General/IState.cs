using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IState {
	public void Hold(){

	}

	public void Walk(Transform t){
		t.Translate (Vector3.right * 0.5f * Time.deltaTime);
	}

	public abstract void OnStateEnter (GameObject obj);
	public abstract void OnStateStay (GameObject obj);
	public abstract void OnStateExit (GameObject obj);
	public abstract EnemyScript.EnemyState ChangeState (GameObject obj);
}
