using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeLayerManager : MonoBehaviour {

	public List<Transform> layers = new List<Transform>();
	public static TimeLayerManager instance;

	public void Awake(){
		Application.targetFrameRate = 15;
	}

	public static TimeLayerManager GetInstance{
		get{
			if (instance == null) {
				instance = GameObject.FindObjectOfType<TimeLayerManager> ();
				if (instance == null) {
					var singleton = new GameObject ("TimeLayerManager");
					singleton.AddComponent<TimeLayerManager> ();
					instance = singleton.GetComponent<TimeLayerManager> ();
				}
			}
			return instance;
		}
	}

	public void MoveObjectToLayer(GameObject obj,int toLayer){
		obj.transform.parent = layers[toLayer];
	}
}
