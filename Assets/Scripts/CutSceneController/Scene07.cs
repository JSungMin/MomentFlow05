using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene07 : MonoBehaviour {
	public EnemyScript security01;
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
		if(security01.enemyState == State.Stun && offset == 0)
		{
			StartCoroutine(StartCutScene());
		}
	}

	private IEnumerator StartCutScene(){
		offset++;
		yield return new WaitForSeconds (3);
		player.state = MyObject.State.Idle;
		player.enabled = false;
		callingChat.StartChat ();
		yield return WaitForClick ();
		callingChat.NextPage ();
		yield return WaitForClick ();
	}

	private IEnumerator WaitForClick(){
		while(true){
			if(Input.GetMouseButtonDown(0)){
				break;
			}
			yield return null;
		}
	}
}
