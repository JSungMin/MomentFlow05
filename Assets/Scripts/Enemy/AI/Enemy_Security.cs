using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Security : EnemyScript {

	public IState[] istate;

	void Awake(){

		playerObject = GameObject.FindObjectOfType<Player> ();

		istate = new IState[2];
		istate [0] = new IdleState ();
		istate [1] = new PatrolState ();
	}

	public float stateChange = 2;
	private float stateChangeTimer = 0;

	// Update is called once per frame
	void Update () {
		switch (enemyState) {
		case EnemyState.Idle:
			if (stateChangeTimer <= stateChange) {
				istate [0].OnStateStay (gameObject);
				stateChangeTimer += Time.deltaTime;
			} else {
				
			}
			break;
		case EnemyState.Patrol:
			istate [1].OnStateStay (gameObject);
			break;
		}
	}
}
