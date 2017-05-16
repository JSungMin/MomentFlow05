using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyObject : MonoBehaviour {

	public float hp = 100;

    // resource manger pattern
    // factory pattern, single pattern
    public enum Character
    {
        Researcher,
        Overseer
    }

	public enum State{
		Idle,
		Walk,
		Sit,
		Run,
		Attack,
		Damaged,
		Dead,
		Fall,
		Jump,
		Landing,
		GrabCorner,
		ClimbCorner,
		ClimbLadder,
		Rolling,
		Dizzy
	}

	public State state;
}
