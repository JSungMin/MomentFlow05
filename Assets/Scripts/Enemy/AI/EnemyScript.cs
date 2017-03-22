using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyScript : MonoBehaviour {

	public Player playerObject { protected set; get; }
    
	public EnemyState enemyState;
    public IState[] istate;

    public float holdDuration;
	public float walkDuration;

	public Vector3 velocity;

	public void Awake(){
		Debug.Log ("In Parent");
		playerObject = GameObject.FindObjectOfType<Player> ();
	}

	public void Walk(){
		transform.Translate (velocity * Time.deltaTime);
	}

	public void Hold(){
		//TODO:Set animation to animName
		Debug.Log("Play : Hold Animation");
	}

	public IState GetState(EnemyState enemyState)
    {
        return istate[(int)enemyState];
    }
}
