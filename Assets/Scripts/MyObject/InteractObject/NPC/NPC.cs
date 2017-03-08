using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour {
	public bool isInteract = false;

	public delegate void InteractWithNPC();
	public delegate void StopInteractWithNPC ();

	private InteractWithNPC interact;
	private StopInteractWithNPC stopInteract;

	public BubbleDialogue bubble;

	public void TalkStart(){
		if (!isInteract) {
			Debug.Log ("Talk");
			bubble.StartBubble ();
			isInteract = true;
		}
	}
	public void TalkEnd(){
		bubble.EndBubble ();
		isInteract = false;
		GameObject.FindObjectOfType<InteractiveManager> ().nowInteract = false;
	}

	public void Interact(){
		interact();
	}
	public void StopInteract(){
		stopInteract ();
	}

	// Use this for initialization
	void Start () {
		interact += TalkStart;
		stopInteract += TalkEnd;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
