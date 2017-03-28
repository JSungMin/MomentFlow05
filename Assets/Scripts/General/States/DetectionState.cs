using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionState : IState {
	public bool isDetection = false;

	public DetectionState(GameObject obj) : base(obj){}

	public override void OnStateEnter(){
		isDetection = true;
		Hold (enemyScript.charAnimName + "_Idle");
	}

	public override void OnStateStay(){
		
	}

	public override void OnStateExit(){

	}
	//현재 State가 Patrol인지를 따진 후, Stay or Enter를 호출한다.
	public override State ChangeState(State nowState){
		if (nowState == State.Detection) {
			OnStateStay ();
		} else {
			if(CheckState())
				OnStateEnter ();
		}
		return State.Detection;	
	}
	public override bool CheckState (){
		return true;
	}
}
