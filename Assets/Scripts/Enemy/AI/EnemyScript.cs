using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyScript : MonoBehaviour {

	public Player playerObject {
		protected set;
		get;
	}

	public enum EnemyState{
		Idle,
		Patrol
	}
	public EnemyState enemyState;

	public float holdDuration;

	public Vector3 velocity;

	public void Walk(){
		transform.Translate (velocity * Time.deltaTime);
	}

	public void Hold(){
		//TODO:Set animation to animName
		Debug.Log("Play : Hold Animation");
	}
}
