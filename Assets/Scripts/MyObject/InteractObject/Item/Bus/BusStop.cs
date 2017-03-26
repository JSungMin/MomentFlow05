using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BusStop : InteractInterface {
	public CutSceneUnit bus;
	public BusController busController;

	public TweenAlpha fadeOutCam;

	public void CallBus(){
		if (!isInteract) {
			fadeOutCam.PlayForward ();
			busController.state = BusController.BusState.Move;
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
