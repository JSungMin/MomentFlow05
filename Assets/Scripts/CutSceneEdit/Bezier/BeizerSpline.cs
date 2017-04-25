using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeizerSpline : MonoBehaviour {

	[SerializeField]
	private Vector3[] points;
	private float curveDistance;

	public int ControlPointCount {
		get {
			return points.Length;
		}
	}

	public Vector3 GetControlPoint (int index) {
		return points[index];
	}

	public void SetControlPoint (int index, Vector3 point) {
		points[index] = point;
	}

	public int CurveCount{
		get{
			return (points.Length - 1)/3;
		}
	}

	public void Reset(){
		points = new Vector3[] {
			new Vector3 (0, 0, 0),
			new Vector3 (1, 0, 0),
			new Vector3 (2, 0, 0)
		};
	}

	public void OnDrawGizmos(){
		var unit = GetComponent<HierarchySystem> ().unit;
		var spline = GetComponent<BeizerSpline>();
		if(spline.transform.GetComponentInParent<PositionPool>().positionItemList.Count-1<spline.GetComponent<HierarchySystem> ().index + 1){
			return;
		}
		if (spline.transform.GetComponentInParent<PositionPool>().selected||unit.pinPath) {
			Vector3 p0 = spline.transform.position;
			spline.SetControlPoint (0,p0);
			//Vector3 p1 = (spline.GetControlPoint (1));
			
			Vector3 p2 = spline.transform.GetComponentInParent<PositionPool>().positionItemList [spline.GetComponent<HierarchySystem> ().index + 1].transform.position;
			spline.SetControlPoint (2, p2);

			Vector3 lineStart = spline.GetPoint (0f);
			for (int i = 1; i <= 30; i++) {
				Vector3 lineEnd = spline.GetPoint (i / (float)30);
				var tmpColor = GetComponent<HierarchySystem> ().unit.color;
				tmpColor.a = 255;
				Gizmos.color = tmpColor;
				Gizmos.DrawLine (lineStart, lineEnd);
				lineStart = lineEnd;
			}
		}
	}

	public Vector3 GetPoint (float t) {
		return (Beizer.GetPoint(points[0], points[1],points[2], t));
	}
	public Vector3 GetVelocity (float t) {
		return transform.TransformPoint(Beizer.GetFirstDerivative(points[0], points[1], points[2], t)) -
			transform.position;
	}
	public Vector3 GetDirection(float t){
		return GetVelocity (t).normalized;
	}
		
	public float GetDistance(){
		var dis = 0f;
		for (int i = 1; i <= 30; i++) {
			dis += Vector3.Distance (GetPoint((i/30f)),GetPoint((i-1)/30f));
		}

		return dis;
	}

	public void AddCurve(){
		Vector3 point = points [points.Length - 1];
		Array.Resize (ref points, points.Length + 2);
		point.x += 1;
		points [points.Length - 2] = point;
		point.x += 1;
		points [points.Length - 1] = point;
	}
}