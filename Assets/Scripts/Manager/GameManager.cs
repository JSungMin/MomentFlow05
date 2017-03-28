using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	private static GameManager instance;

	public int maxTimeStorage;

	public static GameManager GetInstance{
		get{
			if (instance == null) {
				instance = GameObject.FindObjectOfType<GameManager> ();
				if (instance == null) {
					var singleton = new GameObject ("GamaManager");
					singleton.AddComponent<GameManager> ();
					instance = singleton.GetComponent<GameManager> ();
				}
			}
			return instance;
		}
	}
}
