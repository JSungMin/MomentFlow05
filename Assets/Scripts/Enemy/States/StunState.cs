using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunState : IState {

	public StunState(GameObject obj) : base(obj){}

	public override void OnStateEnter(){
		//TODO:애니메이션 추가 시 아래 문장 사용
		//enemyScript.anim.setAnimation (0, enemyScript.charAnimName + "_Stun", true, 1);
	}

	public override void OnStateStay(){

	}

	public override void OnStateExit(){

	}
	//현재 State가 Patrol인지를 따진 후, Stay or Enter를 호출한다.
	public override State ChangeState(State nowState){
		if (nowState == State.Stun) {
			OnStateStay ();
		} else {
			if(CheckState())
				OnStateEnter ();
		}
		return State.Stun;	
	}
	public override bool CheckState (){
		return true;
	}
}
