using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeLayer : MonoBehaviour{
	public int layerNum;

	//You have to box your parent which have TimeLayer Component to parameter
	public static int GetTimeLayer(GameObject obj){
		return obj.GetComponent<TimeLayer> ().layerNum;
	}
	//You have to box your parent which have TimeLayer Component to parameter
	public static bool EqualTimeLayer(GameObject obj1, GameObject obj2){
		return (GetTimeLayer (obj1) == GetTimeLayer (obj2)) ? true : false;
	}
	public static bool EqualTimeLayer(TimeLayer obj1, TimeLayer obj2){
		return (obj1.layerNum == obj2.layerNum) ? true : false;
	}
}
