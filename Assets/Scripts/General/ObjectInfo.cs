using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInfo : MonoBehaviour {

	public float hp;

	public enum State{
		Idle,
		Sit,
		Walk,
		Run,
		Attack,
		Damaged,
		Dead,
		Fall,
		Jump,
		Dizzy
	}

	public State state;

	public void Destroyed(){
		state = State.Dead;
		Debug.Log ("Destroyed");
	}
}
