using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour {

	public float playerTime;
	public float environmentTime;

	public float playerDeltaTime;
	public float environmentDeltaTime;

	public float playerTimeScale;
	public float environmentTimeScale;

	private static TimeManager instance;

	public static TimeManager GetInstance{
		get{
			if (instance == null) {
				instance = GameObject.FindObjectOfType<TimeManager> ();
				if (instance == null) {
					var singleton = new GameObject ("TimeManager");
					singleton.AddComponent<TimeManager> ();
					instance = singleton.GetComponent<TimeManager> ();
				}
			}
			return instance;
		}
	}

	// Use this for initialization
	void Start () {
		playerTimeScale = 1;
		environmentTimeScale = 1;
		playerTime = Time.realtimeSinceStartup;
		environmentTime = Time.realtimeSinceStartup;
	}
	
	// Update is called once per frame
	void Update () {
		playerDeltaTime = (Time.realtimeSinceStartup - playerTime)*playerTimeScale;
		environmentDeltaTime = (Time.realtimeSinceStartup - environmentTime)*environmentTimeScale;

		playerTime = Time.realtimeSinceStartup;
		environmentTime = Time.realtimeSinceStartup;
	}
}
