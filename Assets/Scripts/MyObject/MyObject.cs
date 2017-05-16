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
		Dizzy
	}

	public State state;

	public List<int> stateQueue;
	public State[] statePriority;

	protected bool isLeft ()
	{
		return Mathf.Sign(transform.localScale.x) == 1.0f ? false : true;
	}

	public int GetStatePriorityLevel(State state)
	{
		for(int i = 0; i < statePriority.Length; i++)
		{
			if(state == statePriority[i])
			{
				return i;
			}
		}
		return -1;
	}

	public void AddToStateQueueWithCheckingOverlap (int stateLevel)
	{
		if (!stateQueue.Contains (stateLevel))
		{
			stateQueue.Add (stateLevel);
		}
	}

	public void DeleteFromStateQueue (int stateLevel)
	{
		if (stateQueue.Contains (stateLevel)) {
			stateQueue.Remove (stateLevel);
		}
	}
}
