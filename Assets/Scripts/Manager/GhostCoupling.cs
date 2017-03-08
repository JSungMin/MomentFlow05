using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostCoupling : MonoBehaviour {

	public enum CoupleState
	{
		Solo,
		Duo
	}

	public bool isPresentObject;

	public CoupleState coupleState;

	public GameObject pastObject;
	public GameObject presentObject;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(coupleState == CoupleState.Duo){
			if (pastObject == null) {
				DestroyObject (presentObject);
			}
			if(presentObject == null){
				DestroyObject (pastObject);
			}
		}
	}
}
