﻿using System.Collections;
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
		Walk,
		Sit,
		Run,
		Attack,
		Damaged,
		Dead,
		Fall,
		Jump,
		GrabCorner,
		ClimbCorner,
		Rolling,
		Dizzy
	}

	public State state;

	public void Destroyed(){
		state = State.Dead;
	}
}
