using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPoolScript : MonoBehaviour {

	public void Walk(Vector3 velocity){
		transform.Translate (velocity * Time.deltaTime);
	}

	public void Hold(){
		//TODO:Set animation to animName
		Debug.Log("Play : Hold Animation");
	}
}
