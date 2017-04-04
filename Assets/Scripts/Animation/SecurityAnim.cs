using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityAnim : AnimationBase {
	EnemyScript esrc;
	public MeshRenderer meshRender;
	// Use this for initialization
	void Start () {
		esrc = GetComponent<EnemyScript> ();
	}


	// Update is called once per frame
	void Update () {
		if (!isCutScene) {
			
		}
	}
}
