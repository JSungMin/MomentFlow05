using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerAnim : AnimationBase {

	private new void Awake()
	{
		base.Awake ();
	}

	public void Walk(){
		setAnimation (0, "Walk", true, 1);
	}
	//손님이 손잡이를 잡고 있는 모션
	public void GrabHandle(){
		//setAnimation (0, "GrabHandle", false, 1);
		setAnimation (0, "Idle", true, 1);
	}
}
