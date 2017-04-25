using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FadeView : BaseUnit {

	public float startViewSize=1;
	public float endViewSize=4;

	private float gapViewSize;

	public AnimationCurve curve;

	public Camera dynamicCamera;

	bool canUse = false;
	bool isOrthographicSize = false;
	void OnEnable(){
		InitUnit ();

		if(GetComponent<Camera>()==null){
			dynamicCamera = hi.unit.GetComponent<Camera> ();
			if (GetComponent<Camera>() == null) {
				Debug.LogWarning ("No Camera Component useable");
				return;
			}
		}
		canUse = true;
		isOrthographicSize = GetComponent<Camera>().orthographic;
	}
	
	// Update is called once per frame
	void Update () {
		if(canUse&&transform.GetComponentInParent<PositionPool>().offset == hi.index){
			if (hi.unit.timer >= startTime) {
				gapViewSize = endViewSize - startViewSize;
				if(isOrthographicSize)
					GetComponent<Camera>().orthographicSize = startViewSize + curve.Evaluate ((hi.unit.timer-startTime)/endTime)*gapViewSize;
				else
					GetComponent<Camera>().fieldOfView = startViewSize + curve.Evaluate ((hi.unit.timer-startTime)/endTime)*gapViewSize;
			}
		}
	}
}
