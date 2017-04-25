using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoboAnim : AnimationBase {

	private new void Awake()
	{
		base.Awake ();
		NormalFace ();
	}

	public void NormalFace(){
		setAnimation (0, "Lobo_normal", true, 1);
	}

	public void AngryFace(){
		setAnimation (0, "Lobo_Angry", true, 1);
	}

	public void Walk(){
		setAnimation (1, "Lobo_Walk", true, 1);
	}

	public void Run(){
		setAnimation (1, "Walk", true, 1);
	}

	public void Attack(){
		setAnimation (1, charAnimName + "Punch", false, 1);
	}

	public void LoboIdle(){
		setAnimation (1, "Lobo_Idle", true, 1);
	}
}
