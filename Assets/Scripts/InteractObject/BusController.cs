using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusController : MonoBehaviour {

	public SpriteRenderer tire01;
	public SpriteRenderer tire02;

	public float maxSpeed;
	public float accelSpeed;
	public float breakSpeed;
	public float speed;

	public enum BusState{
		Stop,
		Move
	}

	public BusState state;

	public void SetBusState(string s){
		if (s == "Move") {
			state = BusState.Move;
			Debug.Log ("MOve");
		} else if (s == "Stop") {
			Debug.Log ("Stop");
			state = BusState.Stop;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(state == BusState.Move){
			if (speed <= maxSpeed) {
				speed += accelSpeed * Time.deltaTime;
			}
		}else if(state == BusState.Stop){
			if (speed > 0) {
				speed -= breakSpeed * Time.deltaTime;
			} else {
				speed = 0;
			}
		}
		tire01.transform.Rotate (Vector3.forward * speed * Time.deltaTime);
		tire02.transform.Rotate (Vector3.forward * speed * Time.deltaTime);
	}
}
