using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Struct;

namespace Struct{
	public enum CutSceneWrapMode{
		Default,
		Loop,
		PingPong
	}
	public enum MoveMethod{
		Flat,
		Lelp,
		Slerp
	}
	[System.Serializable]
	public struct PositionItem{
		public int index;
		public Transform transform;

		public PositionItem (int i, Transform t){
			index = i;
			transform = t;
		}
	}
	[System.Serializable]
	public struct DurationItem{
		public int index;
		public float duration;

		public DurationItem(int i, float d){
			index = i;
			duration = d;
		}
		public void ChangeDuration(float d){
			duration = d;
		}
	}
	[System.Serializable]
	public struct CurveItem{
		public int index;
		public AnimationCurve curve;

		public CurveItem(int i, AnimationCurve c){
			index = i;
			curve = c;
		}
	}
	[System.Serializable]
	public struct EventItem{
		public string eventName;
		public int targetIndex;//어떤 오프셋에서 해당 이벤트가 발생 할 지
		public List<float> eventTimeList;//이벤트가 발생할 시간
		public List<bool> isUsed;//이벤트를 사용했는지에 대한 여부
		[HideInInspector]
		public List<bool> storePrevUsed;
		public UnityEvent OccurEvent;//발생 할 이벤트

		public EventItem(string en,int t,float et,UnityEvent ue){
			eventName = en;
			targetIndex = t;
			eventTimeList = new List<float> ();
			eventTimeList.Add(et);
			isUsed = new List<bool> ();
			storePrevUsed = new List<bool> ();
			OccurEvent = ue;
		}
		public void StoreUsedInfo(){
			if(storePrevUsed.Count!=isUsed.Count){
				while(storePrevUsed.Count<isUsed.Count){
					storePrevUsed.Add (false);
				}
				while (storePrevUsed.Count > isUsed.Count) {
					if (storePrevUsed.Count - 1 >= 0)
						storePrevUsed.RemoveAt (storePrevUsed.Count - 1);
					else
						break;
				}
			}

			for(int i =0;i<isUsed.Count;i++){
				storePrevUsed [i] = isUsed [i];
			}
		}
		public void UndoUsedInfo(){
			for(int i =0;i<isUsed.Count;i++){
				isUsed [i] = storePrevUsed [i];
			}
		}
	}
}
public class CutSceneUnit : MonoBehaviour {
	public bool pinPath;

	public List<PositionPool> tracks = new List<PositionPool>();
	public int nowTrackIndex = 0;

	public bool isAction;
	public CutSceneWrapMode wrapMode;
	public MoveMethod moveMethod;
	public Color color;

	private void MakePositionPool(){
		prevTracksCount = tracks.Count;
		GameObject newPositionPool = new GameObject ("PositionPool");
		newPositionPool.transform.position = transform.position;
		newPositionPool.transform.parent = transform.parent;

		var newScr = newPositionPool.AddComponent<PositionPool>();
		newScr.unit = this;
		newScr.trackIndex = tracks.Count;
		tracks.Add(newScr);
	}

	int prevTracksCount;
	void OnValidate(){
		#if UNITY_EDITOR

		#endif
	}
		
	void OnDrawGizmos(){
		#if UNITY_EDITOR
		if (!CutSceneManager.GetInstance.sceneUnitsList.Contains (this))
			CutSceneManager.GetInstance.sceneUnitsList.Add (this);
		if(tracks.Count == 0){
			GameObject newPositionPool = new GameObject ("PositionPool");
			newPositionPool.transform.position = transform.position;
			var newScr = newPositionPool.AddComponent<PositionPool>();
			newScr.trackIndex = 0;
			newScr.unit = this;

			GameObject newParent = new GameObject ("DynamicObject");
			newParent.transform.localPosition = Vector3.zero;
			newParent.transform.parent = transform.parent;
			transform.parent = newParent.transform;
			newPositionPool.transform.parent = newParent.transform;

			tracks.Add(newScr);
		}
		#endif
	}

	int MaxAliquot(int nInput){
		if (nInput == 0 || nInput == 1)
			return 1;
		List<int> numArray = new List<int>();
		numArray.Add (1);
		var max = 1;
		for( int i=1 ; i<=nInput/2 ; ++i )
		{
			if(nInput%i == 0)
			{
				if (max <= i)
					max = i;
				if(!numArray.Contains(i))
					numArray.Add( i );
			}
		}
		if (max == 1)
			return MaxAliquot (nInput - 1);
		return numArray [numArray.Count-1];
	}

	public void LevelizeWay(int index){
		if (index + 1 < tracks[nowTrackIndex].positionItemList.Count) {
			var hi = tracks[nowTrackIndex].positionItemList[index].transform.GetComponent<HierarchySystem> ();

			var nowPos = hi.transform.position;
			var nextPos = tracks[nowTrackIndex].positionItemList [hi.index + 1].transform.position;

			var offsetX = (int)(nextPos.x - nowPos.x)/CutSceneManager.GetInstance.gridSize.x;
			var offsetY = (int)(nextPos.y - nowPos.y)/CutSceneManager.GetInstance.gridSize.y;
			var signX = Mathf.Sign (offsetX);
			var signY = Mathf.Sign (offsetY);

			var numX = MaxAliquot ((int)(offsetX*signX));
			var numY = MaxAliquot ((int)(offsetY*signY));

			offsetX += signX * 1;
			offsetY += signY * 1;

			var tmpX = 1;
			var tmpY = 1;


			var nowIndex = hi.index + 1;
			var newPos = hi.transform.position;
			while(tmpX<offsetX*signX&&tmpY<offsetY*signY){

				if (tmpX <= offsetX*signX) {
					hi.unit.AddFlatPoint (nowIndex, newPos+ numX*signX* Vector3.right * CutSceneManager.GetInstance.gridSize.x);
					tmpX+= numX;
					nowIndex++;
					newPos = newPos + numX*signX * Vector3.right * CutSceneManager.GetInstance.gridSize.x;
				}
				if (tmpY <= offsetY*signY) {
					hi.unit.AddFlatPoint (nowIndex, newPos + numY*signY*Vector3.up*CutSceneManager.GetInstance.gridSize.y);
					tmpY+=numY;
					nowIndex++;
					newPos = newPos + numY*signY * Vector3.up * CutSceneManager.GetInstance.gridSize.y;
				}
			}
		}
	}

	public void AddFlatPoint(){
		tracks[nowTrackIndex].size+=1;
	}
	public void AddFlatPoint(int index, Vector3 pointPos){
		var newPoint = AddFlatPoint (index);
		newPoint.transform.position = pointPos;
	}
	//index is index + 1
	public Transform AddFlatPoint(int index){
		tracks[nowTrackIndex].size+=1;
		GameObject newPosition = new GameObject ((index + 1).ToString ());
		newPosition.transform.parent = tracks[nowTrackIndex].transform;
		newPosition.transform.SetSiblingIndex (index);
		var array = new GameObject[1];
		array [0] = newPosition;

		if (index < tracks[nowTrackIndex].positionItemList.Count)
			newPosition.transform.position = (tracks[nowTrackIndex].positionItemList [Mathf.Max (index - 1, 0)].transform.position + tracks[nowTrackIndex].positionItemList [Mathf.Max (index, 0)].transform.position) * 0.5f;
		else {
			newPosition.transform.position = tracks[nowTrackIndex].positionItemList [tracks[nowTrackIndex].positionItemList.Count - 1].transform.position;
		}
		newPosition.AddComponent<SnapOnGrid> ();

		var hierarchy = newPosition.AddComponent<HierarchySystem> ();
		hierarchy.SetParent (tracks[nowTrackIndex].transform,this);
		hierarchy.index = index;

		for(int i=index;i<tracks[nowTrackIndex].positionItemList.Count;i++){
			tracks[nowTrackIndex].positionItemList [i] = new PositionItem(tracks[nowTrackIndex].positionItemList[i].index + 1,tracks[nowTrackIndex].positionItemList[i].transform);
			tracks[nowTrackIndex].positionItemList [i].transform.name = ((tracks[nowTrackIndex].positionItemList [i].index) + 1).ToString ();
			tracks[nowTrackIndex].positionItemList [i].transform.GetComponent<HierarchySystem> ().index = tracks[nowTrackIndex].positionItemList [i].index;
		}

		var newItem = new PositionItem (index, newPosition.transform);
		tracks[nowTrackIndex].positionItemList.Insert (index, newItem);

		if (index < tracks[nowTrackIndex].durationItemList.Count) {
			var durationItem = new DurationItem (index, 1);
			tracks[nowTrackIndex].durationItemList.Insert (index, durationItem);
		} else {
			var durationItem = new DurationItem (tracks[nowTrackIndex].durationItemList.Count, 1);
			tracks[nowTrackIndex].durationItemList.Add (durationItem);
		}
		if (index < tracks[nowTrackIndex].curveItemList.Count) {
			var newCurve = new AnimationCurve();
			var newKey01 = new Keyframe (0, 0,0,0);
			var newKey02 = new Keyframe (1, 1,0,0);
			newCurve.AddKey (newKey01);
			newCurve.AddKey (newKey02);

			var curveItem = new CurveItem (index,newCurve);
			tracks[nowTrackIndex].curveItemList.Insert (index, curveItem);
		} else {
			var newCurve = new AnimationCurve();
			var newKey01 = new Keyframe (0, 0,0,0);
			var newKey02 = new Keyframe (1, 1,0,0);
			newCurve.AddKey (newKey01);
			newCurve.AddKey (newKey02);

			var curveItem = new CurveItem (index,newCurve);
			tracks[nowTrackIndex].curveItemList.Add (curveItem);
		}
		return newPosition.transform;
	}

	public void DeleteFlatPoint(){
		DestroyImmediate(tracks[nowTrackIndex].transform.GetChild(tracks[nowTrackIndex].transform.childCount-1).gameObject);
		CutSceneManager.GetInstance.ReSortPosition ();
	}
	public void DeleteFlatPoint(int index){
		if (index < tracks[nowTrackIndex].transform.childCount)
			DestroyImmediate (tracks[nowTrackIndex].transform.GetChild (index).gameObject);
		else
			DeleteFlatPoint ();
		CutSceneManager.GetInstance.ReSortPosition ();
	}


	public void AddCurvePoint(){
		tracks[nowTrackIndex].size+=1;

		GameObject newPosition = new GameObject ((tracks[nowTrackIndex].positionItemList.Count + 1).ToString ());
		newPosition.transform.parent = tracks[nowTrackIndex].transform;
		newPosition.transform.position = tracks[nowTrackIndex].positionItemList[tracks[nowTrackIndex].positionItemList.Count-1].transform.position + Vector3.right*2;
		newPosition.AddComponent<SnapOnGrid> ();

		var hierarchy = newPosition.AddComponent<HierarchySystem> ();
		hierarchy.SetParent (tracks[nowTrackIndex].transform,this);
		hierarchy.index = tracks[nowTrackIndex].positionItemList.Count;

		var newItem = new PositionItem (tracks[nowTrackIndex].positionItemList.Count, newPosition.transform);
		tracks[nowTrackIndex].positionItemList.Add (newItem);
		var spline = new BeizerSpline ();

		if (tracks[nowTrackIndex].positionItemList [tracks[nowTrackIndex].positionItemList.Count - 2].transform.GetComponent<BeizerSpline> () == null) {
			spline = tracks[nowTrackIndex].positionItemList [tracks[nowTrackIndex].positionItemList.Count - 2].transform.gameObject.AddComponent<BeizerSpline> ();
			spline.SetControlPoint (1, (tracks[nowTrackIndex].positionItemList [tracks[nowTrackIndex].positionItemList.Count - 1].transform.position + newPosition.transform.position)*0.5f);
		}
		else
			Debug.LogError("Already have Curve");
	}
	//index is index + 1
	public void AddCurvePoint(int index){
		if (tracks[nowTrackIndex].positionItemList.Count < index + 1) {
			AddCurvePoint ();
		} else {
			if (tracks[nowTrackIndex].positionItemList [index - 1].transform.GetComponent<BeizerSpline> () == null) {
				var spline = tracks[nowTrackIndex].positionItemList [index - 1].transform.gameObject.AddComponent<BeizerSpline> ();
				spline.SetControlPoint (1, (tracks[nowTrackIndex].positionItemList [index].transform.position + tracks[nowTrackIndex].positionItemList [index-1].transform.position)*0.5f);
			}
			else
				Debug.LogError ("Already have Curve");
		}
	}
	public void DeleteCurvePoint(int index){
		if (tracks[nowTrackIndex].positionItemList [index].transform.GetComponent<BeizerSpline> () != null)
			DestroyImmediate (tracks[nowTrackIndex].positionItemList [index].transform.GetComponent<BeizerSpline> ());
		else
			Debug.LogError ("Index : " + index + "  Object haven't Curve Component");
	}

	public void StartAction(){
		isAction = true;
	}
	public void PasueAction(){
		isAction = false;
	}
	public void StopAction(){
		isAction = false;
		tracks[nowTrackIndex].offset = 0;
		timer = 0;
		transform.position = tracks[nowTrackIndex].positionItemList [tracks[nowTrackIndex].positionItemList.Count - 1].transform.position;
		isPong = false;
	}

	public void MakeArc(int resolution,int radius,int angle,float duration){
		var dir = (tracks[nowTrackIndex].positionItemList [tracks[nowTrackIndex].positionItemList.Count - 2].transform.position - tracks[nowTrackIndex].positionItemList [tracks[nowTrackIndex].positionItemList.Count - 1].transform.position).normalized;
		var center = tracks[nowTrackIndex].positionItemList [tracks[nowTrackIndex].positionItemList.Count - 1].transform.position + dir* radius;

		float tmpAngle = 0;

		for (int i = 0; i < resolution; i++) {
			GameObject newPosition = new GameObject ((tracks[nowTrackIndex].positionItemList.Count + 1).ToString ()+ " Circle");
			newPosition.transform.parent = tracks[nowTrackIndex].transform;
			newPosition.transform.position = center + radius*(new Vector3 (Mathf.Sin(tmpAngle*Mathf.Deg2Rad),Mathf.Cos(tmpAngle*Mathf.Deg2Rad),0));
			newPosition.AddComponent<SnapOnGrid> ();

			var newItem = new PositionItem (tracks[nowTrackIndex].positionItemList.Count, newPosition.transform);
			tracks[nowTrackIndex].positionItemList.Add (newItem);
			tmpAngle += (float)((float)angle / (float)resolution);
		}

		tracks[nowTrackIndex].size += resolution;
		while (tracks[nowTrackIndex].size - 1 > tracks[nowTrackIndex].durationItemList.Count) {
			var newItem = new DurationItem (tracks[nowTrackIndex].durationItemList.Count, duration/resolution);
			tracks[nowTrackIndex].durationItemList.Add (newItem);
		}
	}

	public void MakeRectangle(int width, int height, float duration){
		var center = tracks[nowTrackIndex].positionItemList [tracks[nowTrackIndex].positionItemList.Count-1].transform.position;

		GameObject newPosition = new GameObject ((tracks[nowTrackIndex].positionItemList.Count + 1).ToString ()+ " Rectangle");
		newPosition.transform.parent = tracks[nowTrackIndex].transform;
		newPosition.transform.position = center + Vector3.right * width * 0.5f + Vector3.up*height*0.5f;
		newPosition.AddComponent<SnapOnGrid> ();

		var newItem = new PositionItem (tracks[nowTrackIndex].positionItemList.Count, newPosition.transform);
		tracks[nowTrackIndex].positionItemList.Add (newItem);

		newPosition = new GameObject ((tracks[nowTrackIndex].positionItemList.Count + 1).ToString ()+ " Rectangle");
		newPosition.transform.parent = tracks[nowTrackIndex].transform;
		newPosition.transform.position = center -Vector3.right * width * 0.5f + Vector3.up*height*0.5f;
		newPosition.AddComponent<SnapOnGrid> ();

		newItem = new PositionItem (tracks[nowTrackIndex].positionItemList.Count, newPosition.transform);
		tracks[nowTrackIndex].positionItemList.Add (newItem);

		newPosition = new GameObject ((tracks[nowTrackIndex].positionItemList.Count + 1).ToString ()+ " Rectangle");
		newPosition.transform.parent = tracks[nowTrackIndex].transform;
		newPosition.transform.position = center - Vector3.up * height * 0.5f - Vector3.right*width*0.5f;
		newPosition.AddComponent<SnapOnGrid> ();

		newItem = new PositionItem (tracks[nowTrackIndex].positionItemList.Count, newPosition.transform);
		tracks[nowTrackIndex].positionItemList.Add (newItem);

		newPosition = new GameObject ((tracks[nowTrackIndex].positionItemList.Count + 1).ToString ()+ " Rectangle");
		newPosition.transform.parent = tracks[nowTrackIndex].transform;
		newPosition.transform.position = center -Vector3.up * height * 0.5f + Vector3.right*width*0.5f;
		newPosition.AddComponent<SnapOnGrid> ();

		newItem = new PositionItem (tracks[nowTrackIndex].positionItemList.Count, newPosition.transform);
		tracks[nowTrackIndex].positionItemList.Add (newItem);

		newPosition = new GameObject ((tracks[nowTrackIndex].positionItemList.Count + 1).ToString ()+ " Rectangle");
		newPosition.transform.parent = tracks[nowTrackIndex].transform;
		newPosition.transform.position = center + Vector3.right * width * 0.5f + Vector3.up*height*0.5f;
		newPosition.AddComponent<SnapOnGrid> ();

		newItem = new PositionItem (tracks[nowTrackIndex].positionItemList.Count, newPosition.transform);
		tracks[nowTrackIndex].positionItemList.Add (newItem);

		tracks[nowTrackIndex].size += 5;
		while (tracks[nowTrackIndex].size - 1 > tracks[nowTrackIndex].durationItemList.Count) {
			var Item = new DurationItem (tracks[nowTrackIndex].durationItemList.Count, duration/4);
			tracks[nowTrackIndex].durationItemList.Add (Item);
		}

	}

	public void AdjustSpeed(float speed){
		if (tracks[nowTrackIndex].durationItemList.Count == tracks[nowTrackIndex].positionItemList.Count - 1) {
			List<DurationItem> tmp = new List<DurationItem>();
			for (int i = 0; i < tracks[nowTrackIndex].durationItemList.Count; i++) {
				if (tracks[nowTrackIndex].positionItemList [i].transform.GetComponent<BeizerSpline> () == null) {
					var target01 = tracks[nowTrackIndex].positionItemList [i].transform.position;
					var target02 = tracks[nowTrackIndex].positionItemList [i + 1].transform.position;

					DurationItem tmpItem = new DurationItem (i, Vector3.Distance (target01, target02) / speed);
					tmp.Add (tmpItem);
				} else {
//					Debug.Log ("i : " + i + "  Dis : " + positionItemList[i].transform.GetComponent<BeizerSpline>().GetDistance());
					DurationItem tmpItem = new DurationItem (i, tracks[nowTrackIndex].positionItemList[i].transform.GetComponent<BeizerSpline>().GetDistance()/ speed);
					tmp.Add (tmpItem);
				}
			}
			tracks[nowTrackIndex].durationItemList.Clear ();
			for(int i =0;i<tmp.Count;i++){
				tracks[nowTrackIndex].durationItemList.Add (tmp [i]);
			}
		} else {
			Debug.LogWarning ("Wait for second to adjust Items");
		}
	}

	Vector3 ClassificateSpeedMethod(){
		Vector3 tmp = Vector3.zero;
		return tmp;
	}

	Vector3 pp = Vector3.zero;

	void ProcessDefaultWrapMode(){
		if (tracks[nowTrackIndex].offset < tracks[nowTrackIndex].size-1) {
			if (timer <= tracks[nowTrackIndex].durationItemList [tracks[nowTrackIndex].offset].duration) {

				//Animate Transform
				switch (moveMethod) {
				case MoveMethod.Flat:
					if (tracks[nowTrackIndex].positionItemList [tracks[nowTrackIndex].offset].transform.GetComponent<BeizerSpline> () == null)
						pp += -Time.deltaTime * (tracks[nowTrackIndex].positionItemList [tracks[nowTrackIndex].offset].transform.position - tracks[nowTrackIndex].positionItemList [tracks[nowTrackIndex].offset + 1].transform.position) / tracks[nowTrackIndex].durationItemList [tracks[nowTrackIndex].offset].duration;
					else {
						var scale = (timer / tracks[nowTrackIndex].durationItemList [tracks[nowTrackIndex].offset].duration);
						pp = tracks[nowTrackIndex].positionItemList [tracks[nowTrackIndex].offset].transform.GetComponent<BeizerSpline> ().GetPoint (scale);
					}
					break;
				case MoveMethod.Lelp:
					pp = Vector3.Lerp (tracks[nowTrackIndex].positionItemList [tracks[nowTrackIndex].offset].transform.position, tracks[nowTrackIndex].positionItemList [tracks[nowTrackIndex].offset + 1].transform.position,tracks[nowTrackIndex].curveItemList [tracks[nowTrackIndex].offset].curve.Evaluate (timer));
					break;
				case MoveMethod.Slerp:
					pp = Vector3.Slerp (tracks[nowTrackIndex].positionItemList [tracks[nowTrackIndex].offset].transform.position, tracks[nowTrackIndex].positionItemList [tracks[nowTrackIndex].offset + 1].transform.position,tracks[nowTrackIndex].curveItemList [tracks[nowTrackIndex].offset].curve.Evaluate (timer));
					break;
				}
				transform.position =  pp;
			
				//Finish Transform Scope
				for(int i =0;i<tracks[nowTrackIndex].eventItemList.Count;i++){
					if(tracks[nowTrackIndex].eventItemList[i].targetIndex == tracks[nowTrackIndex].offset){
						for(int j=0;j<tracks[nowTrackIndex].eventItemList[i].eventTimeList.Count;j++){
							if(tracks[nowTrackIndex].eventItemList[i].eventTimeList[j]<=timer){
								if (null != tracks[nowTrackIndex].eventItemList [i].OccurEvent&&!tracks[nowTrackIndex].eventItemList[i].isUsed[j]) {
									tracks[nowTrackIndex].eventItemList [i].OccurEvent.Invoke ();
									tracks[nowTrackIndex].eventItemList [i].isUsed [j] = true;
								}
							}
						}
					}
				}

				timer += Time.deltaTime;
			} else {
				timer = 0;
				tracks[nowTrackIndex].offset++;
				pp = tracks[nowTrackIndex].positionItemList [tracks[nowTrackIndex].offset].transform.position;
			}
		}
	}

	void ProcessLoopWrapMode(){
		ProcessDefaultWrapMode ();

		if (tracks[nowTrackIndex].offset >= tracks[nowTrackIndex].size - 1) {
			tracks[nowTrackIndex].offset = 0;
			pp = tracks[nowTrackIndex].positionItemList [tracks[nowTrackIndex].offset].transform.position;
			for(int i =0;i<tracks[nowTrackIndex].eventItemList.Count;i++){
				tracks[nowTrackIndex].eventItemList [i].UndoUsedInfo ();
			}
		}
	}

	void ProcessPingPongWrapMode(){
		if (tracks[nowTrackIndex].offset < tracks[nowTrackIndex].size - 1 && !isPong) {
			if (timer <= tracks[nowTrackIndex].durationItemList [tracks[nowTrackIndex].offset].duration) {
				//Animate Transform
				switch (moveMethod) {
				case MoveMethod.Flat:
					if (tracks[nowTrackIndex].positionItemList [tracks[nowTrackIndex].offset].transform.GetComponent<BeizerSpline> () == null)
						pp += -Time.deltaTime * (tracks[nowTrackIndex].positionItemList [tracks[nowTrackIndex].offset].transform.position - tracks[nowTrackIndex].positionItemList [tracks[nowTrackIndex].offset + 1].transform.position) / tracks[nowTrackIndex].durationItemList [tracks[nowTrackIndex].offset].duration;
					else {
						var scale = (timer / tracks[nowTrackIndex].durationItemList [tracks[nowTrackIndex].offset].duration);
						pp = tracks[nowTrackIndex].positionItemList [tracks[nowTrackIndex].offset].transform.GetComponent<BeizerSpline> ().GetPoint (scale);
					}
					break;
				case MoveMethod.Lelp:
					pp = Vector3.Lerp (tracks[nowTrackIndex].positionItemList [tracks[nowTrackIndex].offset].transform.position, tracks[nowTrackIndex].positionItemList [tracks[nowTrackIndex].offset + 1].transform.position,tracks[nowTrackIndex].curveItemList [tracks[nowTrackIndex].offset].curve.Evaluate (timer));
					break;
				case MoveMethod.Slerp:
					pp = Vector3.Slerp (tracks[nowTrackIndex].positionItemList [tracks[nowTrackIndex].offset].transform.position, tracks[nowTrackIndex].positionItemList [tracks[nowTrackIndex].offset + 1].transform.position,tracks[nowTrackIndex].curveItemList [tracks[nowTrackIndex].offset].curve.Evaluate (timer));
					break;
				}

				transform.position =  pp;
				//transform.rotation = Quaternion.Lerp (positionItemList [offset].transform.rotation, positionItemList [offset + 1].transform.rotation, curveItemList [offset].curve.Evaluate (timer));
				//transform.localScale = Vector3.Lerp (positionItemList [offset].transform.localScale, positionItemList [offset + 1].transform.localScale, curveItemList [offset].curve.Evaluate (timer));
				//
				for (int i = 0; i < tracks[nowTrackIndex].eventItemList.Count; i++) {
					if (tracks[nowTrackIndex].eventItemList [i].targetIndex == tracks[nowTrackIndex].offset) {
						for (int j = 0; j < tracks[nowTrackIndex].eventItemList [i].eventTimeList.Count; j++) {
							if (tracks[nowTrackIndex].eventItemList [i].eventTimeList [j] <= timer) {
								if (tracks[nowTrackIndex].eventItemList [i].OccurEvent != null && !tracks[nowTrackIndex].eventItemList [i].isUsed [j]) {
									tracks[nowTrackIndex].eventItemList [i].OccurEvent.Invoke ();
									tracks[nowTrackIndex].eventItemList [i].isUsed [j] = true;
								}
							}
						}
					}
				}

				timer += Time.deltaTime;
			} else {
				timer = 0;
				tracks[nowTrackIndex].offset++;
				pp = tracks[nowTrackIndex].positionItemList [tracks[nowTrackIndex].offset].transform.position;
			}
		} else {
			if (!isPong) {
				isPong = true;
				timer = tracks[nowTrackIndex].durationItemList [tracks[nowTrackIndex].offset-1].duration;
				tracks[nowTrackIndex].offset--;
			}
			if (tracks[nowTrackIndex].offset >= 0) {
				if (timer >= 0) {
					//Animate Transform
					switch (moveMethod) {
					case MoveMethod.Flat:
						if (tracks[nowTrackIndex].positionItemList [tracks[nowTrackIndex].offset].transform.GetComponent<BeizerSpline> () == null)
							pp += Time.deltaTime * (tracks[nowTrackIndex].positionItemList [tracks[nowTrackIndex].offset].transform.position - tracks[nowTrackIndex].positionItemList [tracks[nowTrackIndex].offset + 1].transform.position) / tracks[nowTrackIndex].durationItemList [tracks[nowTrackIndex].offset].duration;
						else {
							var scale = (timer / tracks[nowTrackIndex].durationItemList [tracks[nowTrackIndex].offset].duration);
							pp = tracks[nowTrackIndex].positionItemList [tracks[nowTrackIndex].offset].transform.GetComponent<BeizerSpline> ().GetPoint (scale);
						}
						break;
					case MoveMethod.Lelp:
						pp = Vector3.Lerp (tracks[nowTrackIndex].positionItemList [tracks[nowTrackIndex].offset].transform.position, tracks[nowTrackIndex].positionItemList [tracks[nowTrackIndex].offset + 1].transform.position,tracks[nowTrackIndex].curveItemList [tracks[nowTrackIndex].offset].curve.Evaluate (timer));
						break;
					case MoveMethod.Slerp:
						pp = Vector3.Slerp (tracks[nowTrackIndex].positionItemList [tracks[nowTrackIndex].offset].transform.position, tracks[nowTrackIndex].positionItemList [tracks[nowTrackIndex].offset + 1].transform.position,tracks[nowTrackIndex].curveItemList [tracks[nowTrackIndex].offset].curve.Evaluate (timer));
						break;
					}

					transform.position =  pp;
					//transform.rotation = Quaternion.Lerp (positionItemList [offset].transform.rotation, positionItemList [offset + 1].transform.rotation, curveItemList [offset].curve.Evaluate (timer));
					//transform.localScale = Vector3.Lerp (positionItemList [offset].transform.localScale, positionItemList [offset + 1].transform.localScale, curveItemList [offset].curve.Evaluate (timer));
					//
					for (int i = 0; i < tracks[nowTrackIndex].eventItemList.Count; i++) {
						if (tracks[nowTrackIndex].eventItemList [i].targetIndex == tracks[nowTrackIndex].offset) {
							for (int j = 0; j < tracks[nowTrackIndex].eventItemList [i].eventTimeList.Count; j++) {
								if (tracks[nowTrackIndex].eventItemList [i].eventTimeList [j] >= timer) {
									if (tracks[nowTrackIndex].eventItemList [i].OccurEvent != null && tracks[nowTrackIndex].eventItemList [i].isUsed [j]) {
										tracks[nowTrackIndex].eventItemList [i].OccurEvent.Invoke ();
										tracks[nowTrackIndex].eventItemList [i].isUsed [j] = false;
									}
								}
							}
						}
					}
					timer -= Time.deltaTime;
				} else {
					if (tracks[nowTrackIndex].offset-1 >= 0) {
						timer =tracks[nowTrackIndex].durationItemList [tracks[nowTrackIndex].offset-1].duration;
						pp = tracks[nowTrackIndex].positionItemList [tracks[nowTrackIndex].offset].transform.position;
					}
					tracks[nowTrackIndex].offset--;
				}
			} else {
				tracks[nowTrackIndex].offset = 0;
				pp = tracks[nowTrackIndex].positionItemList [0].transform.position;
				for(int i =0;i<tracks[nowTrackIndex].eventItemList.Count;i++){
					tracks[nowTrackIndex].eventItemList [i].UndoUsedInfo ();
				}
				isPong = false;
			}
		}	
	}
	[HideInInspector]
	public float timer = 0;
	[HideInInspector]
	public bool isPong;

	void Start(){
		pp = tracks[nowTrackIndex].positionItemList [0].transform.position;
		for(int i=0;i<tracks[nowTrackIndex].eventItemList.Count;i++){
			tracks[nowTrackIndex].eventItemList [i].StoreUsedInfo ();
		}
	}
	private DelayTimer haveDelay;
	// Update is called once per frame
	void Update () {
		if (isAction) {
			switch (wrapMode) {
			case CutSceneWrapMode.Default:
				ProcessDefaultWrapMode ();
				break;
			case CutSceneWrapMode.Loop:
				ProcessLoopWrapMode ();
				break;
			case CutSceneWrapMode.PingPong:
				ProcessPingPongWrapMode ();
				break;
			}

		}
	}
}