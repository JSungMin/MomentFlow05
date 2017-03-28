﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuspiciousState : IState {

	public Vector3 targetPos;

	public SuspiciousState (GameObject obj):base(obj){}

	public void InitSuspiciousInfo(Vector3 t,float s){
		targetPos = t;
		speed = s;
		CalculateDirection ();
	}
	public void CalculateDirection(){
		dir = (targetPos - enemyObj.transform.position).normalized;
		dir.y = 0;
		dir.x = Mathf.Sign (dir.x) * 1;
	}

	public bool CheckArrive(){
		var ePos = enemyObj.transform.position;
		var tPos = targetPos;
		ePos.y = 0;
		tPos.y = 0;
		if (Vector3.Distance (ePos, tPos)>=0.3f) {
			CalculateDirection ();
			return false;
		}
		return true;
	}

	public override void OnStateEnter(){
		CalculateDirection ();
		enemyScript.findOutGauge = 50;
		enemyObj.transform.localScale = new Vector3 (Mathf.Sign (dir.x) * Mathf.Abs(enemyObj.transform.localScale.x), enemyObj.transform.localScale.y,enemyObj.transform.localScale.z);
	}

	public override void OnStateStay(){
		if(!CheckArrive()){
			var bInfo = enemyScript.Browse (speed*0.5f);
			if (bInfo.layer != LayerMask.NameToLayer ("Collision")) {
				Walk (enemyScript.charAnimName + "_Suspicious_Walk",enemyObj.transform, dir * speed*0.5f);
			}
			enemyScript.AimToObject (targetPos);
		}
	}

	public override void OnStateExit(){

	}
	//현재 State가 Patrol인지를 따진 후, Stay or Enter를 호출한다.
	public override State ChangeState(State nowState){
		if (nowState == State.Suspicious) {
			OnStateStay ();
		} else {
			if(CheckState())
				OnStateEnter ();
		}
		return State.Suspicious;	
	}
	public override bool CheckState (){
		return true;
	}
}
