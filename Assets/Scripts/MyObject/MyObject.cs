using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyObject : MonoBehaviour {

	public float hp;

    // resource manger pattern
    // factory pattern, single pattern
    public enum Character
    {
        Researcher,
        Overseer
    }

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
