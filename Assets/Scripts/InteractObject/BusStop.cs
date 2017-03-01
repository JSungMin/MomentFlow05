using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BusStop : InteractObject {
	public CutSceneUnit bus;

	public TweenAlpha fadeOutCam;

	public void CallBus(){
		if (!isInteract) {
			Debug.Log ("Talk");
			fadeOutCam.PlayForward ();
			isInteract = true;
		}
	}
	public void DropBus(){
		isInteract = false;
		GameObject.FindObjectOfType<InteractiveManager> ().nowInteract = false;
	}

	public void NextScene(){
		SceneManager.LoadScene (1);	
	}

	// Use this for initialization
	void Start () {
		interact += CallBus;
		stopInteract += DropBus;
	}
}
