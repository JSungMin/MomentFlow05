using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityAnim : AnimationBase {
	EnemyScript esrc;

	// Use this for initialization
	void Start () {
		esrc = GetComponent<EnemyScript> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (!isCutScene) {
			switch(esrc.enemyState){
			case State.Idle:
				
				break;
			case State.Patrol:
				
				break;
			case State.Suspicious:
				//setAnimation (0, "Walk", true, 1);
				break;
			case State.Detection:
				//setAnimation (0, "Run", true, 1);
				break;
			case State.Alert:
				//setAnimation (0, "Alert", true, 1);
				break;
			case State.Attack:
				//setAnimation (0,esrc.charAnimName + "_Shoot", true, 1);
				break;
			case State.Escape:
				//setAnimation (0, "Escape", true, 1);
				break;
			case State.Stun:
				//setAnimation (0, "Strun", true, 1);
				break;
			case State.Dead:
				setAnimation (0, "Dead", true, 1);
				break;
			case State.Sit:
				setAnimation (0, "Sit", false, 1);
				break;
			}
		}
	}
}
