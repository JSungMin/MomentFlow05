using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairZone : MonoBehaviour {

	private Collider[] stairTriggers;

	// Use this for initialization
	void Start () {
		stairTriggers = transform.GetComponentsInChildren<BoxCollider> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
