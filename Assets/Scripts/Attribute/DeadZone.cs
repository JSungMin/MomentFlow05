using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadZone : MonoBehaviour {

	public LayerMask destroyLayer;

	public void OnTriggerEnter2D(Collider2D col){
		Debug.Log ("In Trigger");
		Debug.Log ("COl : " + col.gameObject.layer + "  :  " + destroyLayer);
		if(col.gameObject.layer == LayerMask.NameToLayer("Player")){
			Debug.Log ("In Trigger and layer");
			col.GetComponent<MyObject> ().Destroyed ();
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
