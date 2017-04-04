using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeState : IState {
	public EscapeState (GameObject obj) : base (obj){}
	public override void OnStateEnter(){
		enemyScript.InitToTransition ();
		enemyScript.GetSpecifiedState<AlertState> (State.Alert).Alert ();
	}

	public override void OnStateStay(){
		var bInfo = enemyScript.Browse (enemyScript.findOutSight);
		if (bInfo.layer == LayerMask.NameToLayer ("Player")) {
			var ePos = new Vector2 (enemyObj.transform.position.x, enemyObj.transform.position.y);
			var d = (bInfo.point - ePos).normalized;
			d.y = 0;
			d.x = Mathf.Sign (d.x);
			if (bInfo.layer != LayerMask.NameToLayer ("Collision")) {
				Run (enemyObj.transform, d*enemyScript.moveSpeed * 1.2f);
			}
		} else {
			var d = enemyObj.transform.localScale;
			d.y = 0;
			d.z = 0;
			if (bInfo.layer != LayerMask.NameToLayer ("Collision")) {
				Run (enemyObj.transform, d * enemyScript.moveSpeed * 1.2f);
			}
		}
	}

	public override void OnStateExit(){
		enemyScript.SetDefaultState ();
	}
	public override State ChangeState(State nowState){
		if (nowState == State.Escape) {
			OnStateStay ();
		} else {
			if(CheckState())
				OnStateEnter ();
		}
		return State.Escape;	
	}
	public override bool CheckState (){
		return true;
	}
}
