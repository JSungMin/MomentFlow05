using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
[CustomEditor(typeof(BeizerSpline))]
public class BeizerSplineInspector : Editor {
	#if UNITY_EDITOR
	private BeizerSpline spline;
	private Transform handleTransform;
	private Quaternion handleRotation;

	public const int lineSteps = 30;

	private void OnSceneGUI(){
		#if UNITY_EDITOR
		spline = target as BeizerSpline;
		if (spline != null)
		{
			if (spline.GetComponentInParent<PositionPool>().positionItemList.Count - 1 < spline.GetComponent<HierarchySystem>().index + 1)
			{
				return;
			}
		}
		else
			return;

		handleTransform = spline.transform;
		handleRotation = Tools.pivotRotation == PivotRotation.Local ?
			handleTransform.rotation : Quaternion.identity;

		Vector3 p0 = spline.transform.position;
		spline.SetControlPoint (0,p0);

		Vector3 p1 = ShowPoint(1);

		Vector3 p2 = spline.GetComponentInParent<PositionPool>().positionItemList [spline.GetComponent<HierarchySystem> ().index + 1].transform.position;
		spline.SetControlPoint (2, p2);

		Handles.color = Color.gray;
		Handles.DrawLine(p0, p1);
		Handles.DrawLine(p1, p2);

		Handles.color = Color.white;
		Vector3 lineStart = spline.GetPoint(0f);
		for (int i = 1; i <= lineSteps; i++) {
			Vector3 lineEnd = spline.GetPoint(i / (float)lineSteps);
			Handles.DrawLine(lineStart, lineEnd);
			lineStart = lineEnd;
		}
		#endif
	}

	private const float handleSize = 0.04f;
	private const float pickSize = 0.06f;

	private int selectedIndex = -1;

	private Vector3 ShowPoint (int index) {
		Vector3 point = (spline.GetControlPoint(index));
		float size = HandleUtility.GetHandleSize(point);
		if (Handles.Button(point, handleRotation, size*handleSize, pickSize, Handles.DotCap)) {
			selectedIndex = index;
			Repaint ();
		}
		if (selectedIndex == index&&index!=0&&index!=2) {
			EditorGUI.BeginChangeCheck();
			point = Handles.DoPositionHandle(point, handleRotation);
			if (EditorGUI.EndChangeCheck()) {
				Undo.RecordObject(spline, "Move Point");
				EditorUtility.SetDirty(spline);
				spline.SetControlPoint (index, (point));
			}
		}
		return point;
	}
	#endif
}