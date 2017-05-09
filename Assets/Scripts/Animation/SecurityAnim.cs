using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityAnim : AnimationBase
{
    private new void Awake()
    {
        base.Awake();
    }

	public void SetStrangle(){
		setAnimation (0, "Strangle", false, 1);
	}
}
