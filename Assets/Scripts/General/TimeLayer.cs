using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeLayer : MonoBehaviour{
	public int layerNum;

	public static int GetTimeLayer(GameObject obj){
		return obj.GetComponent<TimeLayer> ().layerNum;
	}
	public static bool EqualTimeLayer(GameObject obj1, GameObject obj2){
		return (GetTimeLayer (obj1) == GetTimeLayer (obj2)) ? true : false;
	}
}
