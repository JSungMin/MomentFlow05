using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CustomEditor(typeof(HierarchySystem))]
public class ShortCutKey : Editor
{
	#if UNITY_EDITOR
	bool isF = true;
	bool isC = true;
	bool isD = true;
	bool isL = true;

	private PositionPool posPool;

	Mesh CreateMesh(float width, float height, float start){
		Mesh m = new Mesh();
		m.name = "ScriptedMesh";
		m.vertices = new Vector3[] {
			new Vector3(start, 0, 0),
			new Vector3(start + width,0, 0f),
			new Vector3(start + width, height, 0f),
			new Vector3(start, height, 0f)
		};
		m.uv = new Vector2[] {
			new Vector2 (0, 0),
			new Vector2 (0, 1),
			new Vector2(1, 1),
			new Vector2 (1, 0)
		};
		m.triangles = new int[] { 0, 1, 2, 0, 2, 3};
		m.RecalculateNormals();

		return m;
	}

	void ApplyShortCut(HierarchySystem unit){
		Event e = Event.current;

		switch (e.type)
		{
		case EventType.keyDown:
			if (Event.current.keyCode == KeyCode.F)
			{
				if (isF)
					isF = false;
			}
			else if (Event.current.keyCode == KeyCode.C)
			{
				if (isC)
					isC = false;
			}
			else if (Event.current.keyCode == KeyCode.D)
			{

				if (isD)
					isD = false;
			}
			else if (Event.current.keyCode == KeyCode.L) {
				if (isL) {
					isL = false;
				}
			}
			break;
		case EventType.keyUp:
			if (Event.current.keyCode == KeyCode.F)
			{
				if (!isF)
				{
					if (unit != null)
					{
						if (UnityEditor.Selection.activeTransform == unit.transform)
						{
							unit.unit.AddFlatPoint(unit.index + 1);
						}
					}
					isF = true;
				}
			}
			else if (Event.current.keyCode == KeyCode.C)
			{
				if (!isC)
				{
					if (unit != null)
					{
						if (UnityEditor.Selection.activeTransform == unit.transform)
						{
							unit.unit.AddCurvePoint(unit.index + 1);
						}
					}
					isC = true;
				}
			}
			else if (Event.current.keyCode == KeyCode.D)
			{
				if (!isD)
				{
					if (unit != null)
					{
						if (unit.GetComponent<BeizerSpline>() == null)
						{
							unit.unit.DeleteFlatPoint(unit.index);
							Debug.Log("DeleteFlat");
						}
						else
						{
							Debug.Log("DeleteCurve");
							unit.unit.DeleteCurvePoint(unit.index);
						}
					}
					isD = true;
				}
			}
			else if (Event.current.keyCode == KeyCode.L) {
				if (!isL) {
					if (unit != null) {
						unit.unit.LevelizeWay(unit.index);
					}
					isL = true;
				}
			}
			break;
		}
	}
	void ShowFadeColor(HierarchySystem hi){
		var nowPos = posPool.positionItemList [hi.index].transform.position;
		var nextPos = posPool.positionItemList [hi.index + 1].transform.position;

		float size = HandleUtility.GetHandleSize (hi.transform.position);

		if (Mathf.Abs (nowPos.x- nextPos.x) >= Mathf.Abs (nowPos.y- nextPos.y)) {
			if (nowPos.y <= nextPos.y) {
				nowPos.y -= 0.5f * size;
				nextPos.y = nowPos.y;
			} else {
				nextPos.y -= 0.5f * size;
				nowPos.y = nextPos.y;
			}
			Handles.DrawDottedLine (nowPos, nextPos, 5 / size);
			int timeOffset = (int)(posPool.durationItemList [hi.index].duration / 0.1f) + 1;
			float normalSize = (nextPos.x - nowPos.x) / timeOffset;

			GUIStyle newStyle = new GUIStyle ();
			newStyle.normal.textColor = CutSceneManager.GetInstance.fontColor;
			newStyle.fontSize = (int)(20 / size)*(int)Mathf.Abs(normalSize);
			newStyle.alignment = TextAnchor.MiddleCenter;

			for (int i = 0; i < timeOffset; i++) {
				var p = nowPos + Vector3.right * i * normalSize + Vector3.up * 0.35f;
				Handles.Label (p, (0.1f * i).ToString (), newStyle);
			}


		} 
		else {
			if (nowPos.x <= nextPos.x) {
				nowPos.x -= 0.5f * size;
				nextPos.x = nowPos.x;
			} else {
				nextPos.x -= 0.5f * size;
				nowPos.x = nextPos.x;
			}
			Handles.DrawDottedLine (nowPos, nextPos, 5 / size);
			int timeOffset = (int)(posPool.durationItemList [hi.index].duration / 0.1f) + 1;
			float normalSize = (nextPos.y- nowPos.y) / timeOffset;

			GUIStyle newStyle = new GUIStyle ();
			newStyle.normal.textColor = CutSceneManager.GetInstance.fontColor;
			newStyle.fontSize = (int)(20 / size)*(int)Mathf.Abs(normalSize);
			newStyle.alignment = TextAnchor.MiddleCenter;

			for (int i = 0; i <= timeOffset; i++) {
				var p = nowPos + Vector3.up * i * normalSize +Vector3.left * 0.35f;
				Handles.Label (p, (0.1f * i).ToString (), newStyle);
			}
		}
		var fc = hi.GetComponent<FadeColor> ();
		if (fc != null) {
			hi.transform.position = Handles.DoPositionHandle (hi.transform.position, Quaternion.identity);

			var pNowPos = (Mathf.Abs (nextPos.x - nowPos.x) >= Mathf.Abs (nextPos.y - nowPos.y)) ? HandleUtility.WorldToGUIPoint (nowPos + Vector3.right * ((nextPos - nowPos).magnitude) * ((fc.startTime) / (posPool.durationItemList [hi.index].duration))) : HandleUtility.WorldToGUIPoint (nowPos + Vector3.up * ((nextPos - nowPos).magnitude) * ((fc.startTime) / (posPool.durationItemList [hi.index].duration)));
			var pNextPos = (Mathf.Abs (nextPos.x - nowPos.x) >= Mathf.Abs (nextPos.y - nowPos.y)) ? HandleUtility.WorldToGUIPoint (nowPos + Vector3.right * ((nextPos - nowPos).magnitude) * ((fc.endTime) / (posPool.durationItemList [hi.index].duration))) : HandleUtility.WorldToGUIPoint (nowPos + Vector3.up * ((nextPos - nowPos).magnitude) * ((fc.endTime) / (posPool.durationItemList [hi.index].duration)));

			var dis = Vector3.Distance (pNowPos, pNextPos);
			var dir = (nextPos - nowPos);

			var sign = (Mathf.Abs (nextPos.x - nowPos.x) >= Mathf.Abs (nextPos.y - nowPos.y)) ? Mathf.Sign (dir.x):Mathf.Sign(dir.y);
		
			var width = (Mathf.Abs (nextPos.x - nowPos.x) >= Mathf.Abs (nextPos.y - nowPos.y)) ? dis : 10;
			var height = (Mathf.Abs (nextPos.x - nowPos.x) >= Mathf.Abs (nextPos.y - nowPos.y)) ? 10 : dis;
			if (sign >= 0) {
				Handles.BeginGUI ();
				GUIStyle newStyle = new GUIStyle ();
				newStyle.overflow = new RectOffset ((int)width, (int)width, (int)height, (int)height);

				if (Mathf.Abs (nextPos.x - nowPos.x) >= Mathf.Abs (nextPos.y - nowPos.y)) {
					fc.tex = new Texture2D ((int)width, (int)height, TextureFormat.ARGB32, false);
					for (int i = 0; i < (int)width; i++) {
						for (int j = 0; j < (int)height; j++) {
							Color tmpColor = fc.color.Evaluate (((float)i / width));
							tmpColor.a *= 0.8f;
							fc.tex.SetPixel (i, j, tmpColor);
						}
					}
					fc.tex.Apply ();

					GUI.Box (new Rect (pNowPos.x, pNowPos.y, width, height), fc.tex, newStyle);
				} else {
					fc.tex = new Texture2D ((int)width, (int)height, TextureFormat.ARGB32, false);
					for (int i = 0; i < (int)height; i++) {
						for (int j = 0; j < (int)width; j++) {
							Color tmpColor = fc.color.Evaluate (((float)i / height));
							tmpColor.a *= 0.8f;
							fc.tex.SetPixel (j, i, tmpColor);
						}
					}
					fc.tex.Apply ();

					GUI.Box (new Rect (pNowPos.x - 15/size, pNextPos.y, width, height), fc.tex, newStyle);
				}
				Handles.EndGUI ();
			} else {
				pNowPos = (Mathf.Abs (nextPos.x - nowPos.x) >= Mathf.Abs (nextPos.y - nowPos.y)) ? HandleUtility.WorldToGUIPoint (nowPos - Vector3.right * ((nextPos - nowPos).magnitude) * ((fc.startTime) / (posPool.durationItemList [hi.index].duration))) : HandleUtility.WorldToGUIPoint (nowPos - Vector3.up * ((nextPos - nowPos).magnitude) * ((fc.startTime) / (posPool.durationItemList [hi.index].duration)));
				pNextPos = (Mathf.Abs (nextPos.x - nowPos.x) >= Mathf.Abs (nextPos.y - nowPos.y)) ? HandleUtility.WorldToGUIPoint (nowPos - Vector3.right * ((nextPos - nowPos).magnitude) * ((fc.endTime) / (posPool.durationItemList [hi.index].duration))) : HandleUtility.WorldToGUIPoint (nowPos - Vector3.up * ((nextPos - nowPos).magnitude) * ((fc.endTime) / (posPool.durationItemList [hi.index].duration)));


				Handles.BeginGUI ();
				GUIStyle newStyle = new GUIStyle ();
				newStyle.overflow = new RectOffset ((int)width, (int)width, (int)height, (int)height);

				if (Mathf.Abs (nextPos.x - nowPos.x) >= Mathf.Abs (nextPos.y - nowPos.y)) {
					fc.tex = new Texture2D ((int)width, (int)height, TextureFormat.ARGB32, false);
					for (int i = 0; i < (int)width; i++) {
						for (int j = 0; j < (int)height; j++) {
							Color tmpColor = fc.color.Evaluate (((float)i / width));
							tmpColor.a *= 0.8f;
							fc.tex.SetPixel ((int)(width-1) - i, j, tmpColor);
						}
					}
					fc.tex.Apply ();

					GUI.Box (new Rect (pNextPos.x, pNextPos.y, width, height), fc.tex, newStyle);
				} else {
					fc.tex = new Texture2D ((int)width, (int)height, TextureFormat.ARGB32, false);
					for (int i = 0; i < (int)height; i++) {
						for (int j = 0; j < (int)width; j++) {
							Color tmpColor = fc.color.Evaluate (((float)i / height));
							tmpColor.a *= 0.8f;
							fc.tex.SetPixel (j, (int)(height-1) - i, tmpColor);
						}
					}
					fc.tex.Apply ();

					GUI.Box (new Rect (pNextPos.x- 15/size, pNowPos.y, width, height), fc.tex, newStyle);
				}
				Handles.EndGUI ();
			}
		}
	}

	public void ShowDelayTimer(HierarchySystem hi){

		var dt = hi.GetComponent<DelayTimer> ();
		if (dt == null) {
			return;
		}

		var nowPos = hi.transform.position;
		var nextPos = posPool.positionItemList [hi.index + 1].transform.position;
		var dis = Vector3.Distance (nowPos,nextPos);
		var pos = nowPos+(nextPos - nowPos).normalized * (dt.delayStartTime / posPool.durationItemList[hi.index].duration)*dis;

		float size = HandleUtility.GetHandleSize (pos);

		var tmpColor = CutSceneManager.GetInstance.fontColor;

		tmpColor = new Color (tmpColor.g, tmpColor.b, tmpColor.r);

		Handles.color = tmpColor;
		Handles.DrawDottedLine (pos, pos + Vector3.right*(dt.delayStartTime)*size, 3f);

		GUIStyle newStyle = new GUIStyle ();
		newStyle.normal.textColor = tmpColor;
		newStyle.fontSize = (int)(10/size);
		Handles.Label(pos + Vector3.right*(dt.delayStartTime + 0.2f)*size,"Delay "+dt.delayTime.ToString(),newStyle);

	}

	void ApplyVisualization(HierarchySystem hi){
		if (posPool.selected) {
			if (hi.index < posPool.positionItemList.Count-1) {
				ShowFadeColor (hi);
				ShowDelayTimer (hi);
			}
		} 
	}

	void OnSceneGUI()
	{
		#if UNITY_EDITOR
		var unit = target as HierarchySystem;
		posPool = unit.GetComponentInParent<PositionPool>();
		ApplyShortCut (unit);
		ApplyVisualization (unit);
		#endif
	}
	#endif
}