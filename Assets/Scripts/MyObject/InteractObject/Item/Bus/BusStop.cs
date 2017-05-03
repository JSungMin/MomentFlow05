using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BusStop : InteractInterface {
	public CutSceneUnit bus;
	public BusController busController;

	public TweenAlpha fadeOutCam;

	public void CallBus(){
		if (!isInteracted) {
			fadeOutCam.PlayForward ();
			busController.state = BusController.BusState.Move;
            bus.StartAction();
            isInteracted = true;
		}
	}

	public void DropBus(){
	}

	public void NextScene(){
		SceneManager.LoadScene (1);	
	}
    
	void Start () {
		interact += CallBus;
		stopInteract += DropBus;
	}
}
