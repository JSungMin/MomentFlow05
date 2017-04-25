using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUnit : MonoBehaviour {

	protected HierarchySystem hi;

	protected float lineDuration;

	public float startTime;
	public float endTime=1;

	protected bool isInit = false;

	public void InitUnit(){
		if (!isInit) {
			hi = GetComponent<HierarchySystem> ();
			if (hi.index >= transform.GetComponentInParent<PositionPool>().durationItemList.Count)
				hi.unit.AddFlatPoint (hi.index + 1);
			lineDuration =transform.GetComponentInParent<PositionPool>().durationItemList [hi.index].duration;
			isInit = true;
		}
	}
}
