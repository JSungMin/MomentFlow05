using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityAnim : AnimationBase
{
    private new void Awake()
    {
        base.Awake();
    }

	public void Idle(){
		setAnimation (1, charAnimName + "Idle", true, 1);
	}

	public void Walk(){
		setAnimation (1, charAnimName + "Walk", true, 1);
	}
}
