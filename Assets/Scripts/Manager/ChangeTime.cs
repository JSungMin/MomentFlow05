using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeTime : MonoBehaviour {

	public GameObject pastTime;
	public GameObject presentTime;

	public enum TimeState{
		Past,
		Present
	}

	public TimeState timeState;

	// Use this for initialization
	void Start () {
		timeState = TimeState.Present;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Tab)){
			if (timeState == TimeState.Past) {
				presentTime.SetActive (true);
				pastTime.SetActive (false);
				timeState = TimeState.Present;
			} else if(timeState == TimeState.Present){
				presentTime.SetActive (false);
				pastTime.SetActive (true);
				timeState = TimeState.Past;
			}
		}
	}
}
