using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

using Struct;

public class PositionPool : MonoBehaviour {
	public CutSceneUnit unit;

	public bool selected {
		private set;
		get;
	}
	public int trackIndex;
	public int size;
	public int offset = 0;

	//Independece with size
	public List<PositionItem> positionItemList = new List<PositionItem>();
	public List<DurationItem> durationItemList = new List<DurationItem>();
	public List<CurveItem> curveItemList = new List<CurveItem>(); 

	//not dependence with size
	public List<EventItem> eventItemList = new List<EventItem>();
	// Use this for initialization
	void OnEnable () {
		if(unit == null)
			unit = transform.parent.GetComponentInChildren<CutSceneUnit> ();
	}

	void OnValidate(){
		#if UNITY_EDITOR
		//OccurEvent의 크기와 Event Item List의 크기를 같게 조정함
		for (int i = 0; i < eventItemList.Count; i++) {				
			if (eventItemList [i].eventTimeList.Count > eventItemList [i].OccurEvent.GetPersistentEventCount ()) {
				//Delete
				var num = eventItemList [i].eventTimeList.Count - eventItemList [i].OccurEvent.GetPersistentEventCount ();
				eventItemList [i].eventTimeList.RemoveRange (eventItemList [i].eventTimeList.Count - 1 - num, num);
			} 
			else if(eventItemList [i].eventTimeList.Count < eventItemList [i].OccurEvent.GetPersistentEventCount ()){
				//Add
				var num = -eventItemList [i].eventTimeList.Count + eventItemList [i].OccurEvent.GetPersistentEventCount ();
				for (int j = 0; j < num; j++) {
					eventItemList[i].eventTimeList.Add(0);
				}
			}
			if (eventItemList [i].isUsed.Count > eventItemList [i].OccurEvent.GetPersistentEventCount ()) {
				//Delete
				var num = eventItemList [i].isUsed.Count - eventItemList [i].OccurEvent.GetPersistentEventCount ();
				eventItemList [i].isUsed.RemoveRange (eventItemList [i].isUsed.Count - 1 - num, num);
			}
			else if(eventItemList [i].isUsed.Count < eventItemList [i].OccurEvent.GetPersistentEventCount ()){
				//Add
				var num = -eventItemList [i].isUsed.Count + eventItemList [i].OccurEvent.GetPersistentEventCount ();
				for(int j =0;j<num;j++){
					eventItemList[i].isUsed.Add(false);
				}
			}
		}
		#endif
	}
	void OnDrawGizmos(){
		#if UNITY_EDITOR
		if (size < 2)
			size = 2;

		//size에 맞게 PositionItemList를 조정한다.
		{
			while (size > positionItemList.Count) {
				GameObject newPosition = new GameObject ((positionItemList.Count + 1).ToString ());
				newPosition.transform.parent = transform;
				if (positionItemList.Count != 0)
					newPosition.transform.position = positionItemList [positionItemList.Count - 1].transform.position;
				else
					newPosition.transform.position = Vector3.zero;
				newPosition.AddComponent<SnapOnGrid> ();
				newPosition.AddComponent<HierarchySystem> ();
				newPosition.GetComponent<HierarchySystem> ().SetParent (transform,unit);
				newPosition.GetComponent<HierarchySystem> ().index = positionItemList.Count;

				var newItem = new PositionItem (positionItemList.Count, newPosition.transform);
				positionItemList.Add (newItem);
			}

			while (size < positionItemList.Count) {
				DestroyImmediate (transform.GetChild(positionItemList.Count-1).gameObject);
				positionItemList.RemoveAt (positionItemList.Count - 1);
			}
		}

		//size-1에 맞게 durationItemList를 조정한다.
		{
			while (size - 1 > durationItemList.Count) {
				var newItem = new DurationItem (durationItemList.Count, 1);
				durationItemList.Add (newItem);
			}
			while(size-1<durationItemList.Count){
				durationItemList.RemoveAt (durationItemList.Count-1);
			}
		}
		//size-1에 맞게 curveItemList를 조정한다.
		{
			while (size - 1 > curveItemList.Count) {
				AnimationCurve newCurve = new AnimationCurve ();
				var newKey01 = new Keyframe (0, 0,0,0);
				var newKey02 = new Keyframe (1, 1,0,0);
				newCurve.AddKey (newKey01);
				newCurve.AddKey (newKey02);

				var newItem = new CurveItem (curveItemList.Count, newCurve);
				curveItemList.Add (newItem);
			}
			while(size-1<curveItemList.Count){
				curveItemList.RemoveAt (curveItemList.Count-1);
			}
		}

		//Set Select State
		if (UnityEditor.Selection.activeGameObject != null) {
			if (UnityEditor.Selection.activeGameObject == gameObject) {
				selected = true;
			} else if (UnityEditor.Selection.activeTransform.parent != null && UnityEditor.Selection.activeTransform.parent.GetComponent<PositionPool>() != null ||
				UnityEditor.Selection.activeTransform.GetComponent<PositionPool>() != null ||
				UnityEditor.Selection.activeTransform.parent == this.transform) {
				selected = true;
			} else {
				selected = false;
			}
		}

		//Show Position and Event Icon
		if(selected||unit.pinPath){
			for(int i =0;i<size;i++){
				float tmpSize = 1f;
				tmpSize = HandleUtility.GetHandleSize(positionItemList[i].transform.position);
				GUIStyle newStyle = new GUIStyle ();
				newStyle.normal.textColor = CutSceneManager.GetInstance.fontColor;
				newStyle.fontSize = 20;
				newStyle.alignment = TextAnchor.MiddleCenter;
				Handles.Label (positionItemList[i].transform.position + Vector3.up*0.1f + Vector3.up*tmpSize*0.3f,(i + 1).ToString(),newStyle);
				for(int j =i;j<size;j++){
					if(positionItemList[i].index==positionItemList[j].index-1){
						Gizmos.DrawIcon (positionItemList [i].transform.position + Vector3.up*0.05f, "Position.png",true);
						Gizmos.DrawIcon (positionItemList [j].transform.position + Vector3.up*0.05f, "Position.png",true);
						for(int k =0;k<eventItemList.Count;k++){
							foreach(EventItem t in eventItemList){
								if(t.targetIndex==i){
									foreach(float et in t.eventTimeList){
										var pos = Vector3.Lerp (positionItemList[i].transform.position,positionItemList[j].transform.position,et/durationItemList[i].duration);
										Gizmos.DrawIcon (pos,"Event.png",true);
									}
								}
							}
						}

						var tmpColor = new Color (unit.color.r - ((unit.color.r-0.3f)/(float)size)*positionItemList[i].index,unit.color.g - ((unit.color.g-0.3f)/(float)size)*positionItemList[i].index,unit.color.b - ((unit.color.b-0.3f)/(float)size)*positionItemList[i].index);

						Gizmos.color = tmpColor;
						if(positionItemList[i].transform.GetComponent<BeizerSpline>()==null)
							Gizmos.DrawLine (positionItemList[i].transform.position,positionItemList[j].transform.position);
					}
				}
			}
		}
		#endif
	}
	// Update is called once per frame
	void Update () {
		
	}
}
