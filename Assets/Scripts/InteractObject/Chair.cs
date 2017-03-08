using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chair : InteractObject {

	public Player player;

	public Vector3 direction;

	public void SitDown(){
		if (!isInteract&&player.state != ObjectInfo.State.Sit) {
			player.state = ObjectInfo.State.Sit;
			isInteract = true;
		}
	}
	public void SitUp(){
		isInteract = false;
		GameObject.FindObjectOfType<InteractiveManager> ().nowInteract = false;
	}

	void Start () {
		interact += SitDown;
		stopInteract += SitUp;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
