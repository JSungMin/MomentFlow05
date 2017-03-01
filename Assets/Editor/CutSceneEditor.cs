using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;

public class CutSceneEditor : EditorWindow {
	#if UNITY_EDITOR
	public int resolution = 0;
	public int radius = 0;
	public int angle = 0;
	public float duration = 1;

	public int width;
	public int height;
	public float duration02=1;

	public float durationTime;

	public Object source;

	public static Color gridColor = Color.white;
	public static bool isGrid = false;

	AnimBool showArcField;
	AnimBool showSpeedAdjustField;

	void OnEnable(){
		#if UNITY_EDITOR
		EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
		#endif
	}
	bool isF = true;
	bool isC = true;
	bool isD = true;
	bool isL = true;
	void OnHierarchyGUI(int instanceID, Rect selectionRect){
		#if UNITY_EDITOR
		source = UnityEditor.Selection.activeObject;
		if (source == null) {
			return;
		}
		Event e = Event.current;
		switch (e.type) {
		case EventType.keyDown:
			if (Event.current.keyCode == KeyCode.F) {
				if (isF) {
					Debug.Log ("Press F");
					isF = false;
				}
			}else if (Event.current.keyCode == KeyCode.C) {
				if (isC) {
					isC = false;
				}
			}else if (Event.current.keyCode == KeyCode.D) {
				if (isD) {
					isD = false;
				}
			}else if (Event.current.keyCode == KeyCode.L) {
				if (isL) {
					Debug.Log ("Press L");
					isL = false;
				}
			}

			break;
		case EventType.keyUp:
			if (Event.current.keyCode == KeyCode.F) {
				if (!isF) {
					var unit = ((GameObject)source).GetComponent<HierarchySystem> ();
					if (unit != null) {
						if (UnityEditor.Selection.activeTransform == unit.transform) {
							unit.unit.AddFlatPoint (unit.index + 1);
							Debug.Log ("Add Flat Point");
						}
					}
					isF = true;
				}
			}else if (Event.current.keyCode == KeyCode.C) {
				if (!isC) {
					var unit = ((GameObject)source).GetComponent<HierarchySystem> ();
					if (unit != null) {
						if (UnityEditor.Selection.activeTransform == unit.transform) {
							unit.unit.AddCurvePoint (unit.index + 1);
							Debug.Log ("Add Curve Point");
						}
					}
					isC = true;
				}
			}else if (Event.current.keyCode == KeyCode.D) {
				if (!isD) {
					var unit = ((GameObject)source).GetComponent<HierarchySystem> ();
					if (unit != null) {
						if (unit.GetComponent<BeizerSpline> () == null) {
							unit.unit.DeleteFlatPoint (unit.index);
							Debug.Log ("Delete Position");
						}
						else {
							unit.unit.DeleteCurvePoint (unit.index);
							Debug.Log ("Delete Curve Point");
						}
					}
					isD = true;
				}
			}else if (Event.current.keyCode == KeyCode.L) {
				if (!isL) {
					var unit = ((GameObject)source).GetComponent<HierarchySystem> ();
					if (unit != null) {
						unit.unit.LevelizeWay (unit.index);
					}
					isL = true;
				}
			}
	
			break;
		}
		Repaint ();
		#endif
	}
	[DrawGizmo(GizmoType.NotInSelectionHierarchy)]
	static void RenderCustomGizmo(Transform objectTransform, GizmoType gizmoType)
	{
		#if UNITY_EDITOR
		if (isGrid)
		{
			var gridSize = CutSceneManager.GetInstance.gridSize;
			Vector3 minGrid = SceneView.currentDrawingSceneView.camera.ScreenPointToRay(new Vector2(0f, 0f)).origin;
			Vector3 maxGrid = SceneView.currentDrawingSceneView.camera.ScreenPointToRay(new Vector2(SceneView.currentDrawingSceneView.camera.pixelWidth, SceneView.currentDrawingSceneView.camera.pixelHeight)).origin;
			Gizmos.color = new Color (gridColor.r, gridColor.g, gridColor.b, gridColor.a/125);
			for (float i = Mathf.Round (minGrid.x / gridSize.x) * gridSize.x; i < Mathf.Round (maxGrid.x / gridSize.x) * gridSize.x && gridSize.x > 0.05f; i += gridSize.x) {
				Gizmos.DrawLine (new Vector3 (i, minGrid.y, 0.0f), new Vector3 (i, maxGrid.y, 0.0f));
			}
			for (float j = Mathf.Round (minGrid.y / gridSize.y) * gridSize.y; j < Mathf.Round (maxGrid.y / gridSize.y) * gridSize.y && gridSize.y > 0.05f; j += gridSize.y) {
				Gizmos.DrawLine (new Vector3 (minGrid.x, j, 0.0f), new Vector3 (maxGrid.x, j, 0.0f));
			}
				
			SceneView.RepaintAll();
		}
		#endif
	}
	Vector2 scrollPos;
	[MenuItem ("Blacksmith/Advanced CutScene")]
	public static void ShowWindow(){
		#if UNITY_EDITOR
		EditorWindow.GetWindow (typeof(CutSceneEditor));
		#endif
	}

	float verticalScrollValue;


	void OnGUI()
	{
		#if UNITY_EDITOR
		GUILayout.Label ("Select Target", EditorStyles.boldLabel);
		EditorApplication.hierarchyWindowItemOnGUI = CutSceneManager.GetInstance.ResetList;
		source = UnityEditor.Selection.activeObject;

		EditorGUILayout.BeginHorizontal ();
		CutSceneManager.GetInstance.gridSize = EditorGUILayout.Vector2Field ("Grid Size (0.05 minimum)", CutSceneManager.GetInstance.gridSize, GUILayout.Width (236));
		isGrid = EditorGUILayout.Toggle (isGrid, GUILayout.Width (16));

		EditorGUILayout.EndHorizontal ();
		gridColor = EditorGUILayout.ColorField (gridColor, GUILayout.Width (150));
		GUILayout.Space (10);

		CutSceneManager.GetInstance.fontColor = EditorGUILayout.ColorField (CutSceneManager.GetInstance.fontColor, GUILayout.Width (150));

		GUILayout.Space (25);
		GUILayout.BeginHorizontal ();
		GUILayout.Space (50);

		if (GUILayout.Button ("Snap", GUILayout.MaxWidth (200))) {
			var selectObj = UnityEditor.Selection.activeTransform;
			if (selectObj.parent.name.Equals ("PositionPool") && isGrid) {
				selectObj.GetComponent<SnapOnGrid> ().Snap ();
			}
		}

		if (GUILayout.Button ("Snap All", GUILayout.MaxWidth (200))) {
			var selectObjs = UnityEditor.Selection.gameObjects;
			foreach (var selectObj in selectObjs) {
				if (selectObj.transform.parent.name.Equals ("PositionPool") && isGrid) {
					selectObj.GetComponent<SnapOnGrid> ().Snap ();
				}
			}
		}
	
		GUILayout.EndHorizontal ();
		GUILayout.Space (10);
	
		GUILayout.BeginHorizontal ();
		GUILayout.Space (50);

		if (GUILayout.Button ("Levelize", GUILayout.MaxWidth (200))) {
			var selectObj = UnityEditor.Selection.activeTransform;
			if (selectObj.parent.name.Equals ("PositionPool") && isGrid) {
				selectObj.GetComponent<HierarchySystem>().unit.LevelizeWay (selectObj.GetComponent<HierarchySystem>().index);
			}
		}
		if (GUILayout.Button ("Levelize All", GUILayout.MaxWidth (200))) {
			var selectObjs = UnityEditor.Selection.gameObjects;
			foreach (var selectObj in selectObjs) {
				if (selectObj.transform.parent.name.Equals ("PositionPool") && isGrid) {
					selectObj.GetComponent<HierarchySystem>().unit.LevelizeWay (selectObj.GetComponent<HierarchySystem>().index);
				}
			}
		}
		GUILayout.EndHorizontal ();

		GUILayout.Space (20);
		if (((GameObject)source) == null)
			return;
		else if (((GameObject)source).GetComponent<CutSceneUnit> () != null) {
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.ObjectField (source, typeof(CutSceneUnit), true);
			EditorGUILayout.EndHorizontal ();
	
			GUILayout.Space (20);
			{
				GUILayout.BeginHorizontal ();
				{
					GUILayout.BeginVertical ();
					{
						scrollPos = GUILayout.BeginScrollView (scrollPos, false, true, GUILayout.Width (300), GUILayout.Height (200));

						GUILayout.Label ("Make Arc", EditorStyles.boldLabel);
						GUILayout.BeginVertical (EditorStyles.helpBox);
						{
							if (((GameObject)source).GetComponent<CutSceneUnit> () != null) {
								showArcField = new AnimBool (true);
								showArcField.valueChanged.AddListener (Repaint);
								showArcField.target = true;

							} else {
								source = GameObject.FindObjectOfType<CutSceneUnit> ().transform.gameObject;
								showArcField.target = false;
							}
							if (EditorGUILayout.BeginFadeGroup (showArcField.faded)) {
								GUILayout.Space (5);
								resolution = EditorGUILayout.IntField ("Resolution", resolution, GUILayout.MaxWidth (200));
								GUILayout.Space (10);
								radius = EditorGUILayout.IntField ("Radius", radius, GUILayout.MaxWidth (200));
								GUILayout.Space (10);
								angle = EditorGUILayout.IntField ("Angle", angle, GUILayout.MaxWidth (200));
								GUILayout.Space (10);
								duration = EditorGUILayout.FloatField ("Duration", duration, GUILayout.MaxWidth (200));

								GUILayout.BeginHorizontal ();
								GUILayout.FlexibleSpace ();

								if (GUILayout.Button ("Create", GUILayout.MaxWidth (200))) {
									((GameObject)source).GetComponent<CutSceneUnit> ().MakeArc (resolution, radius, angle, duration);		
								}
								GUILayout.EndHorizontal ();	
							}
							EditorGUILayout.EndFadeGroup ();
							GUILayout.EndVertical ();
						}

						GUILayout.Space (10);

						GUILayout.Label ("Make Rectangle", EditorStyles.boldLabel);
						GUILayout.BeginVertical (EditorStyles.helpBox);
						{
							if (((GameObject)source).GetComponent<CutSceneUnit> () != null) {
								showArcField = new AnimBool (true);
								showArcField.valueChanged.AddListener (Repaint);
								showArcField.target = true;

							} else {
								showArcField.target = false;
							}

							if (EditorGUILayout.BeginFadeGroup (showArcField.faded)) {
								GUILayout.Space (5);
								width = EditorGUILayout.IntField ("Width", width, GUILayout.MaxWidth (200));
								GUILayout.Space (10);
								height = EditorGUILayout.IntField ("Height", height, GUILayout.MaxWidth (200));
								GUILayout.Space (10);
								duration02 = EditorGUILayout.FloatField ("Duration", duration02, GUILayout.MaxWidth (200));

								GUILayout.BeginHorizontal ();
								GUILayout.FlexibleSpace ();

								if (GUILayout.Button ("Create", GUILayout.MaxWidth (200))) {
									((GameObject)source).GetComponent<CutSceneUnit> ().MakeRectangle (width, height, duration02);		
								}
								GUILayout.EndHorizontal ();	
							}
							EditorGUILayout.EndFadeGroup ();
							GUILayout.EndVertical ();
						}

						GUILayout.Label ("Adjust MoveSpeed To Value (Only Flat Move)", EditorStyles.boldLabel);
						GUILayout.BeginVertical (EditorStyles.helpBox);
						{
							if (((GameObject)source).GetComponent<CutSceneUnit> ().moveMethod == Struct.MoveMethod.Flat) {
								showSpeedAdjustField = new AnimBool (true);
								showSpeedAdjustField.valueChanged.AddListener (Repaint);
								showSpeedAdjustField.target = true;
							} else {
								showSpeedAdjustField.target = false;
							}

							if (EditorGUILayout.BeginFadeGroup (showSpeedAdjustField.faded)) {
								GUILayout.Space (5);
								durationTime = EditorGUILayout.FloatField ("Speed", durationTime, GUILayout.MaxWidth (200));
								GUILayout.Space (20);
								GUILayout.BeginHorizontal ();
								{
									GUILayout.FlexibleSpace ();
									if (GUILayout.Button ("Adjust It!", GUILayout.MaxWidth (200))) {
										((GameObject)source).GetComponent<CutSceneUnit> ().AdjustSpeed (durationTime);
									}
								}
								GUILayout.EndHorizontal ();
							}
							EditorGUILayout.EndFadeGroup ();
							GUILayout.EndVertical ();
						}

						GUILayout.EndScrollView ();
					}
					GUILayout.EndVertical ();
				}

				GUILayout.EndHorizontal ();
			}


			GUILayout.FlexibleSpace ();
			SceneView.RepaintAll ();
		}//End of Selecting Unit Object Condition
		else if (((GameObject)source).GetComponent<HierarchySystem> () != null) {
			EditorGUILayout.BeginVertical ();
			{

				GUILayout.Space (20);
				EditorGUILayout.BeginHorizontal ();
				{
					if (GUILayout.Button ("Add Flat Point", GUILayout.ExpandWidth (true), GUILayout.MinHeight (40))) {
						Debug.Log ("Add Flat Point");
						((GameObject)source).GetComponent<HierarchySystem> ().unit.AddFlatPoint (((GameObject)source).GetComponent<HierarchySystem> ().index + 1);
					}

					if (GUILayout.Button ("Delete Flat Point", GUILayout.ExpandWidth (true), GUILayout.MinHeight (40))) {
						Debug.Log ("Delete Flat Point");
						((GameObject)source).GetComponent<HierarchySystem> ().unit.DeleteFlatPoint (((GameObject)source).GetComponent<HierarchySystem> ().index);
					}

					EditorGUILayout.EndHorizontal ();
				}

				GUILayout.Space (10);

				EditorGUILayout.BeginHorizontal ();
				{
					if (GUILayout.Button ("Add Curve Point", GUILayout.ExpandWidth (true), GUILayout.MinHeight (40))) {
						Debug.Log ("Add Curve");
						((GameObject)source).GetComponent<HierarchySystem> ().unit.AddCurvePoint (((GameObject)source).GetComponent<HierarchySystem> ().index + 1);
					}

					if (GUILayout.Button ("Delete Curve Point", GUILayout.ExpandWidth (true), GUILayout.MinHeight (40))) {
						((GameObject)source).GetComponent<HierarchySystem> ().unit.DeleteCurvePoint (((GameObject)source).GetComponent<HierarchySystem> ().index);
					}

					EditorGUILayout.EndHorizontal ();
				}
				GUILayout.Space (20);
				EditorGUILayout.EndVertical ();
			}
		} else {
			return;
		}
		#endif
	}
	#endif
}
