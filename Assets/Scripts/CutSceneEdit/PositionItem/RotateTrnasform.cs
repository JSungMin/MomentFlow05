using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTrnasform : BaseUnit {

	bool canUse = false;

	public Vector3 targetRotation;
	public AnimationCurve rotateSpeedCurve;

	void OnEnable(){
		InitUnit ();
	}

	// Update is called once per frame
	void Update () {
		if(transform.GetComponentInParent<PositionPool>().offset == hi.index){
			if (hi.unit.timer >= startTime) {
				transform.rotation = Quaternion.Euler(Vector3.Lerp(transform.rotation.eulerAngles, targetRotation, rotateSpeedCurve.Evaluate ((hi.unit.timer-startTime)/endTime)));
			}
		}
	}
}
