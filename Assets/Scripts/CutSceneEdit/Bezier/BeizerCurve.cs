using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Beizer{
	public static Vector3 GetPoint (Vector3 p0, Vector3 p1, Vector3 p2, float t) {
		t = Mathf.Clamp01(t);
		float oneMinusT = 1f - t;
		return
			oneMinusT * oneMinusT * p0 +
			2f * oneMinusT * t * p1 +
			t * t * p2;
	}

	public static Vector3 GetFirstDerivative (Vector3 p0, Vector3 p1, Vector3 p2, float t) {
		return
			2f * (1f - t) * (p1 - p0) +
			2f * t * (p2 - p1);
	}

	public static float GetDistance(Vector3 p0,Vector3 p1,Vector3 p2,int steps){
		var dis = 0f;
		for (int i = 1; i <= steps; i++) {
			Debug.Log ("0 : " + GetPoint(p0,p1,p2,i/steps) + "   1 : " + GetPoint(p0,p1,p2,(i-1)/steps));
			dis += Vector3.Distance (GetPoint(p0,p1,p2,i/steps),GetPoint(p0,p1,p2,(i-1)/steps));
		}

		return dis;
	}
}

public class BeizerCurve : MonoBehaviour {

	public Vector3[] points;

	public void Reset(){
		points = new Vector3[] {
			new Vector3 (1, 0, 0),
			new Vector3 (2, 0, 0),
			new Vector3 (3, 0, 0),
			new Vector3 (4, 0, 0)
		};
	}

	public Vector3 GetPoint(float t){
		return transform.TransformPoint(Beizer.GetPoint(points[0],points[1],points[2],t));
	}
	public Vector3 GetVelocity(float t){
		return transform.TransformPoint (Beizer.GetFirstDerivative (points [0], points [1], points [2], t)) - transform.position;
	}
	public Vector3 GetDirection(float t){
		return GetVelocity (t).normalized;
	}
}
