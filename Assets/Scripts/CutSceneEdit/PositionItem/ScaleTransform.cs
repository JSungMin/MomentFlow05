using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleTrnasform : BaseUnit {

	bool canUse = false;

	public Vector3 targetScale;
	public AnimationCurve scaleSpeedCurve;

	void OnEnable(){
		InitUnit ();
	}

	// Update is called once per frame
	void Update () {
		if(transform.GetComponentInParent<PositionPool>().offset == hi.index){
			if (hi.unit.timer >= startTime) {
				transform.localScale = Vector3.Lerp(transform.rotation.eulerAngles, targetScale, scaleSpeedCurve.Evaluate ((hi.unit.timer-startTime)/endTime));
			}
		}
	}
}
