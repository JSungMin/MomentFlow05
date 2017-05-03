using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerAnim : AnimationBase {

	private new void Awake()
	{
		base.Awake ();
	}

	public void Walk(){
		setAnimation (0, charAnimName + "Walk", true, 1);
	}

	public void FallDown(){
		//TODO : Idle motion to FallDown motion
		setAnimation (0,  charAnimName + "Idle", true, 1);
		Debug.Log ("FallDown");
	}

	//손님이 손잡이를 잡고 있는 모션
	public void GrabHandle(){
		//setAnimation (0, "GrabHandle", false, 1);
		setAnimation (0,  charAnimName + "Idle", true, 1);
	}
}
