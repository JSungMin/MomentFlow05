﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene07 : MonoBehaviour {
	public Player player;
	private PlayerAnim playerAnim;
	public ChatDialogue callingChat;

	int offset = 0;

	// Use this for initialization
	void Start () {
		playerAnim = player.GetComponent<PlayerAnim> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void StartCutScene(){
		Debug.Log ("StartCutScene");
		StartCoroutine(DialogCut());
	}

	private IEnumerator DialogCut(){
		callingChat.StartChat ();
		yield return WaitForClick ();
		callingChat.NextPage ();
		yield return WaitForClick ();
		player.enabled = true;
		callingChat.EndChat ();
		yield return null;
	}

	private IEnumerator WaitForClick(){
		while(true){
			if(Input.GetMouseButtonDown(0)){
				yield return null;
				break;
			}
			yield return null;
		}
	}
}
