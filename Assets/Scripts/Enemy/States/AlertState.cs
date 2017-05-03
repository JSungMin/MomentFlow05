using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertState : IState {

	public float alertRadius;

	public AlertState (GameObject obj):base(obj){}

	public void InitAlertInfo(float radius){
		alertRadius = radius;
	}
	public void Alert(){
		var colliders = Physics2D.OverlapCircleAll (enemyObj.transform.position, alertRadius, 1 << LayerMask.NameToLayer ("Enemy"));
		for(int i =0;i<colliders.Length;i++){
			var col = colliders [i];
			if(TimeLayer.EqualTimeLayer(col.transform.GetComponentInParent<TimeLayer>(),enemyScript.pTimeLayer)){
				Debug.Log ("Alert Ready");
				var hit = Physics2D.Raycast (enemyObj.transform.position + Vector3.up*0.35f, (col.transform.position - enemyObj.transform.position).normalized,Vector2.Distance(col.transform.position, enemyObj.transform.position),1<<LayerMask.NameToLayer("Collision"));

				if (hit.collider == null) {
					col.GetComponent<EnemyScript> ().GetSpecifiedState<SuspiciousState> (State.Suspicious).InitSuspiciousInfo (enemyScript.GetSpecifiedState<SuspiciousState> (State.Suspicious).targetPos, col.GetComponent<EnemyScript> ().moveSpeed * 0.5f);
					col.GetComponent<EnemyScript> ().SetState (State.Suspicious);
				}
			} 
		}
	}
	public override void OnStateEnter(){
		Debug.Log ("Alert Enter");
		Alert ();
	}

	public override void OnStateStay(){
		
	}

	public override void OnStateExit(){

	}
	//현재 State가 Patrol인지를 따진 후, Stay or Enter를 호출한다.
	public override State ChangeState(State nowState){
		if (nowState == State.Alert) {
			OnStateStay ();
		} else {
			if(CheckState())
				OnStateEnter ();
		}
		return State.Alert;	
	}
	public override bool CheckState (){
		return true;
	}
}
