using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DelayTimer : MonoBehaviour {
	public float delayStartTime = 0;
	public float delayTime=0;

	private HierarchySystem hi;

	public bool isUsed = false;

	void OnEnable(){
		hi = GetComponent<HierarchySystem> ();
	}

	void Pasuse(){
		hi.unit.PasueAction ();
		Invoke ("ReStart", delayTime);
	}

	void ReStart(){
		hi.unit.StartAction ();
		isUsed = true;
	}

	void Update(){
		if (transform.GetComponentInParent<PositionPool>().offset == hi.index) {
			if (hi.unit.isAction) {
				if (delayStartTime <= hi.unit.timer && !isUsed) {
					Pasuse ();
				}
			} 
		}
		else {
			isUsed = false;
		}

	}
}
