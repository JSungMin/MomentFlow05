using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusStop : InteractObject {
	public CutSceneUnit bus;

	public void CallBus(){
		if (!isInteract) {
			Debug.Log ("Talk");
			bus.isAction = true;
			isInteract = true;
		}
	}
	public void DropBus(){
		isInteract = false;
		GameObject.FindObjectOfType<InteractiveManager> ().nowInteract = false;
	}

	// Use this for initialization
	void Start () {
		interact += CallBus;
		stopInteract += DropBus;
	}
}
