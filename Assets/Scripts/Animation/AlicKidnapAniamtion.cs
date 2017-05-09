using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlicKidnapAniamtion : AnimationBase {

	private new void Awake()
	{
		base.Awake();
	}

	public void Dragged()
	{
		setAnimation (0, "Run",true, 1.0f);
		//setAnimation (0, "Dragged",true,1.0f);
	}

	public void FallDown()
	{

	}
}
