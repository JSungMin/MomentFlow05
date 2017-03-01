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
		public int targetIndex;//어떤 오프셋에서 해당 이벤트가 발생 할 지
		public List<float> eventTimeList;//이벤트가 발생할 시간
		public List<bool> isUsed;//이벤트를 사용했는지에 대한 여부
		[HideInInspector]
		public List<bool> storePrevUsed;
		public UnityEvent OccurEvent;//발생 할 이벤트

		public EventItem(int t,float et,UnityEvent ue){
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

	private bool selected;
	public bool Selected{
		set{
			selected = value;
		}
		get{
			return selected;
		}
	}
	public bool isAction;
	public CutSceneWrapMode wrapMode;
	public MoveMethod moveMethod;
	public Color color;

	public int size;
	public int offset = 0;

	public Transform positionItemPool;
	//Independece with size
	public List<PositionItem> positionItemList = new List<PositionItem>();
	public List<DurationItem> durationItemList = new List<DurationItem>();
	public List<CurveItem> curveItemList = new List<CurveItem>(); 

	//not dependence with size
	public List<EventItem> eventItemList = new List<EventItem>();

	void OnValidate(){
		#if UNITY_EDITOR
		for (int i = 0; i < curveItemList.Count; i++) {
			var tmpLastIndex = curveItemList [i].curve.length - 1;
			var tmpLastValue = curveItemList [i].curve.keys [tmpLastIndex].value;
			var tmpLastTime = curveItemList [i].curve.keys [tmpLastIndex].time;

			for (int j = 1; j < curveItemList [i].curve.length; j++) {
				var tmpValue = curveItemList [i].curve.keys [j].value;
				var tmpTime = curveItemList [i].curve.keys [j].time;
				curveItemList [i].curve.MoveKey (j, new Keyframe (Mathf.Clamp (tmpTime + (durationItemList [i].duration - tmpLastTime), 0, durationItemList [i].duration), tmpValue));
			}
		}

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
		if (!CutSceneManager.GetInstance.sceneUnitsList.Contains (this))
			CutSceneManager.GetInstance.sceneUnitsList.Add (this);
		if(positionItemPool==null){
			GameObject newPositionPool = new GameObject ("PositionPool");
			newPositionPool.transform.position = transform.position;
			positionItemPool = newPositionPool.transform;

			GameObject newParent = new GameObject ("DynamicObject");
			newParent.transform.localPosition = Vector3.zero;
			transform.parent = newParent.transform;
			positionItemPool.parent = newParent.transform;
		}

		if (size < 2)
			size = 2;
		//size에 맞게 PositionItemList를 조정한다.
		{
			while (size > positionItemList.Count) {
				GameObject newPosition = new GameObject ((positionItemList.Count + 1).ToString ());
				newPosition.transform.parent = positionItemPool;
				if (positionItemList.Count != 0)
					newPosition.transform.position = positionItemList [positionItemList.Count - 1].transform.position;
				else
					newPosition.transform.position = Vector3.zero;
				newPosition.AddComponent<SnapOnGrid> ();
				newPosition.AddComponent<HierarchySystem> ();
				newPosition.GetComponent<HierarchySystem> ().SetParent (positionItemPool,this);
				newPosition.GetComponent<HierarchySystem> ().index = positionItemList.Count;

				var newItem = new PositionItem (positionItemList.Count, newPosition.transform);
				positionItemList.Add (newItem);
			}

			while (size < positionItemList.Count) {
				DestroyImmediate (positionItemPool.GetChild(positionItemList.Count-1).gameObject);
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
			} else if (UnityEditor.Selection.activeTransform.parent == positionItemPool ||
			          UnityEditor.Selection.activeTransform == positionItemPool ||
			          UnityEditor.Selection.activeTransform.parent == this.transform) {
				selected = true;
			} else {
				selected = false;
			}
		}

		//Show Position and Event Icon
		if(selected||pinPath){
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

						var tmpColor = new Color (color.r - ((color.r-0.3f)/(float)size)*positionItemList[i].index,color.g - ((color.g-0.3f)/(float)size)*positionItemList[i].index,color.b - ((color.b-0.3f)/(float)size)*positionItemList[i].index);

						Gizmos.color = tmpColor;
						if(positionItemList[i].transform.GetComponent<BeizerSpline>()==null)
							Gizmos.DrawLine (positionItemList[i].transform.position,positionItemList[j].transform.position);
					}
				}
			}
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
		if (index + 1 < positionItemList.Count) {
			var hi = positionItemList[index].transform.GetComponent<HierarchySystem> ();

			var nowPos = hi.transform.position;
			var nextPos = hi.unit.positionItemList [hi.index + 1].transform.position;

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
		size++;
	}
	public void AddFlatPoint(int index, Vector3 pointPos){
		var newPoint = AddFlatPoint (index);
		newPoint.transform.position = pointPos;
	}
	//index is index + 1
	public Transform AddFlatPoint(int index){
		size++;
		GameObject newPosition = new GameObject ((index + 1).ToString ());
		newPosition.transform.parent = positionItemPool;
		newPosition.transform.SetSiblingIndex (index);
		var array = new GameObject[1];
		array [0] = newPosition;

		if (index < positionItemList.Count)
			newPosition.transform.position = (positionItemList [Mathf.Max (index - 1, 0)].transform.position + positionItemList [Mathf.Max (index, 0)].transform.position) * 0.5f;
		else {
			newPosition.transform.position = positionItemList [positionItemList.Count - 1].transform.position;
		}
		newPosition.AddComponent<SnapOnGrid> ();

		var hierarchy = newPosition.AddComponent<HierarchySystem> ();
		hierarchy.SetParent (positionItemPool,this);
		hierarchy.index = index;

		for(int i=index;i<positionItemList.Count;i++){
			positionItemList [i] = new PositionItem(positionItemList[i].index + 1,positionItemList[i].transform);
			positionItemList [i].transform.name = ((positionItemList [i].index) + 1).ToString ();
			positionItemList [i].transform.GetComponent<HierarchySystem> ().index = positionItemList [i].index;
		}

		var newItem = new PositionItem (index, newPosition.transform);
		positionItemList.Insert (index, newItem);

		if (index < durationItemList.Count) {
			var durationItem = new DurationItem (index, 1);
			durationItemList.Insert (index, durationItem);
		} else {
			var durationItem = new DurationItem (durationItemList.Count, 1);
			durationItemList.Add (durationItem);
		}
		if (index < curveItemList.Count) {
			var newCurve = new AnimationCurve();
			var newKey01 = new Keyframe (0, 0,0,0);
			var newKey02 = new Keyframe (1, 1,0,0);
			newCurve.AddKey (newKey01);
			newCurve.AddKey (newKey02);

			var curveItem = new CurveItem (index,newCurve);
			curveItemList.Insert (index, curveItem);
		} else {
			var newCurve = new AnimationCurve();
			var newKey01 = new Keyframe (0, 0,0,0);
			var newKey02 = new Keyframe (1, 1,0,0);
			newCurve.AddKey (newKey01);
			newCurve.AddKey (newKey02);

			var curveItem = new CurveItem (index,newCurve);
			curveItemList.Add (curveItem);
		}
		return newPosition.transform;
	}

	public void DeleteFlatPoint(){
		DestroyImmediate(positionItemPool.GetChild(positionItemPool.childCount-1).gameObject);
		CutSceneManager.GetInstance.ReSortPosition ();
	}
	public void DeleteFlatPoint(int index){
		if (index < positionItemPool.childCount)
			DestroyImmediate (positionItemPool.GetChild (index).gameObject);
		else
			DeleteFlatPoint ();
		CutSceneManager.GetInstance.ReSortPosition ();
	}


	public void AddCurvePoint(){
		size+=1;

		GameObject newPosition = new GameObject ((positionItemList.Count + 1).ToString ());
		newPosition.transform.parent = positionItemPool;
		newPosition.transform.position = positionItemList[positionItemList.Count-1].transform.position + Vector3.right*2;
		newPosition.AddComponent<SnapOnGrid> ();

		var hierarchy = newPosition.AddComponent<HierarchySystem> ();
		hierarchy.SetParent (positionItemPool,this);
		hierarchy.index = positionItemList.Count;

		var newItem = new PositionItem (positionItemList.Count, newPosition.transform);
		positionItemList.Add (newItem);
		var spline = new BeizerSpline ();

		if (positionItemList [positionItemList.Count - 2].transform.GetComponent<BeizerSpline> () == null) {
			spline = positionItemList [positionItemList.Count - 2].transform.gameObject.AddComponent<BeizerSpline> ();
			spline.SetControlPoint (1, (positionItemList [positionItemList.Count - 1].transform.position + newPosition.transform.position)*0.5f);
		}
		else
			Debug.LogError("Already have Curve");
	}
	//index is index + 1
	public void AddCurvePoint(int index){
		if (positionItemList.Count < index + 1) {
			AddCurvePoint ();
		} else {
			if (positionItemList [index - 1].transform.GetComponent<BeizerSpline> () == null) {
				var spline = positionItemList [index - 1].transform.gameObject.AddComponent<BeizerSpline> ();
				spline.SetControlPoint (1, (positionItemList [index].transform.position + positionItemList [index-1].transform.position)*0.5f);
			}
			else
				Debug.LogError ("Already have Curve");
		}
	}
	public void DeleteCurvePoint(int index){
		if (positionItemList [index].transform.GetComponent<BeizerSpline> () != null)
			DestroyImmediate (positionItemList [index].transform.GetComponent<BeizerSpline> ());
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
		offset = 0;
		timer = 0;
		transform.position = positionItemList [positionItemList.Count - 1].transform.position;
		isPong = false;
	}

	public void MakeArc(int resolution,int radius,int angle,float duration){
		var dir = (positionItemList [positionItemList.Count - 2].transform.position - positionItemList [positionItemList.Count - 1].transform.position).normalized;
		var center = positionItemList [positionItemList.Count - 1].transform.position + dir* radius;

		float tmpAngle = 0;

		for (int i = 0; i < resolution; i++) {
			GameObject newPosition = new GameObject ((positionItemList.Count + 1).ToString ()+ " Circle");
			newPosition.transform.parent = positionItemPool;
			newPosition.transform.position = center + radius*(new Vector3 (Mathf.Sin(tmpAngle*Mathf.Deg2Rad),Mathf.Cos(tmpAngle*Mathf.Deg2Rad),0));
			newPosition.AddComponent<SnapOnGrid> ();

			var newItem = new PositionItem (positionItemList.Count, newPosition.transform);
			positionItemList.Add (newItem);
			tmpAngle += (float)((float)angle / (float)resolution);
		}

		size += resolution;
		while (size - 1 > durationItemList.Count) {
			var newItem = new DurationItem (durationItemList.Count, duration/resolution);
			durationItemList.Add (newItem);
		}
	}

	public void MakeRectangle(int width, int height, float duration){
		var center = positionItemList [positionItemList.Count-1].transform.position;

		GameObject newPosition = new GameObject ((positionItemList.Count + 1).ToString ()+ " Rectangle");
		newPosition.transform.parent = positionItemPool;
		newPosition.transform.position = center + Vector3.right * width * 0.5f + Vector3.up*height*0.5f;
		newPosition.AddComponent<SnapOnGrid> ();

		var newItem = new PositionItem (positionItemList.Count, newPosition.transform);
		positionItemList.Add (newItem);

		newPosition = new GameObject ((positionItemList.Count + 1).ToString ()+ " Rectangle");
		newPosition.transform.parent = positionItemPool;
		newPosition.transform.position = center -Vector3.right * width * 0.5f + Vector3.up*height*0.5f;
		newPosition.AddComponent<SnapOnGrid> ();

		newItem = new PositionItem (positionItemList.Count, newPosition.transform);
		positionItemList.Add (newItem);

		newPosition = new GameObject ((positionItemList.Count + 1).ToString ()+ " Rectangle");
		newPosition.transform.parent = positionItemPool;
		newPosition.transform.position = center - Vector3.up * height * 0.5f - Vector3.right*width*0.5f;
		newPosition.AddComponent<SnapOnGrid> ();

		newItem = new PositionItem (positionItemList.Count, newPosition.transform);
		positionItemList.Add (newItem);

		newPosition = new GameObject ((positionItemList.Count + 1).ToString ()+ " Rectangle");
		newPosition.transform.parent = positionItemPool;
		newPosition.transform.position = center -Vector3.up * height * 0.5f + Vector3.right*width*0.5f;
		newPosition.AddComponent<SnapOnGrid> ();

		newItem = new PositionItem (positionItemList.Count, newPosition.transform);
		positionItemList.Add (newItem);

		newPosition = new GameObject ((positionItemList.Count + 1).ToString ()+ " Rectangle");
		newPosition.transform.parent = positionItemPool;
		newPosition.transform.position = center + Vector3.right * width * 0.5f + Vector3.up*height*0.5f;
		newPosition.AddComponent<SnapOnGrid> ();

		newItem = new PositionItem (positionItemList.Count, newPosition.transform);
		positionItemList.Add (newItem);

		size += 5;
		while (size - 1 > durationItemList.Count) {
			var Item = new DurationItem (durationItemList.Count, duration/4);
			durationItemList.Add (Item);
		}

	}

	public void AdjustSpeed(float speed){
		if (durationItemList.Count == positionItemList.Count - 1) {
			List<DurationItem> tmp = new List<DurationItem>();
			for (int i = 0; i < durationItemList.Count; i++) {
				if (positionItemList [i].transform.GetComponent<BeizerSpline> () == null) {
					var target01 = positionItemList [i].transform.position;
					var target02 = positionItemList [i + 1].transform.position;

					DurationItem tmpItem = new DurationItem (i, Vector3.Distance (target01, target02) / speed);
					tmp.Add (tmpItem);
				} else {
//					Debug.Log ("i : " + i + "  Dis : " + positionItemList[i].transform.GetComponent<BeizerSpline>().GetDistance());
					DurationItem tmpItem = new DurationItem (i, positionItemList[i].transform.GetComponent<BeizerSpline>().GetDistance()/ speed);
					tmp.Add (tmpItem);
				}
			}
			durationItemList.Clear ();
			for(int i =0;i<tmp.Count;i++){
				durationItemList.Add (tmp [i]);
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
		if (offset < size-1) {
			if (timer <= durationItemList [offset].duration) {

				//Animate Transform
				switch (moveMethod) {
				case MoveMethod.Flat:
					if (positionItemList [offset].transform.GetComponent<BeizerSpline> () == null)
						pp += -Time.deltaTime * (positionItemList [offset].transform.position - positionItemList [offset + 1].transform.position) / durationItemList [offset].duration;
					else {
						var scale = (timer / durationItemList [offset].duration);
						pp = positionItemList [offset].transform.GetComponent<BeizerSpline> ().GetPoint (scale);
					}
					break;
				case MoveMethod.Lelp:
					pp = Vector3.Lerp (positionItemList [offset].transform.position, positionItemList [offset + 1].transform.position,curveItemList [offset].curve.Evaluate (timer));
					break;
				case MoveMethod.Slerp:
					pp = Vector3.Slerp (positionItemList [offset].transform.position, positionItemList [offset + 1].transform.position,curveItemList [offset].curve.Evaluate (timer));
					break;
				}
				transform.position =  pp;
			
				//transform.rotation = Quaternion.Lerp (positionItemList [offset].transform.rotation, positionItemList [offset + 1].transform.rotation, curveItemList [offset].curve.Evaluate (timer));
				//transform.localScale = Vector3.Lerp (positionItemList [offset].transform.localScale, positionItemList [offset + 1].transform.localScale, curveItemList [offset].curve.Evaluate (timer));
				//Finish Transform Scope
				for(int i =0;i<eventItemList.Count;i++){
					if(eventItemList[i].targetIndex == offset){
						for(int j=0;j<eventItemList[i].eventTimeList.Count;j++){
							if(eventItemList[i].eventTimeList[j]<=timer){
								if (eventItemList [i].OccurEvent != null&&!eventItemList[i].isUsed[j]) {
									eventItemList [i].OccurEvent.Invoke ();
									eventItemList [i].isUsed [j] = true;
								}
							}
						}
					}
				}

				timer += Time.deltaTime;
			} else {
				timer = 0;
				offset++;
				pp = positionItemList [offset].transform.position;
			}
		}
	}

	void ProcessLoopWrapMode(){
		ProcessDefaultWrapMode ();

		if (offset >= size - 1) {
			offset = 0;
			pp = positionItemList [offset].transform.position;
			for(int i =0;i<eventItemList.Count;i++){
				eventItemList [i].UndoUsedInfo ();
			}
		}
	}

	void ProcessPingPongWrapMode(){
		if (offset < size - 1 && !isPong) {
			if (timer <= durationItemList [offset].duration) {
				//Animate Transform
				switch (moveMethod) {
				case MoveMethod.Flat:
					if (positionItemList [offset].transform.GetComponent<BeizerSpline> () == null)
						pp += -Time.deltaTime * (positionItemList [offset].transform.position - positionItemList [offset + 1].transform.position) / durationItemList [offset].duration;
					else {
						var scale = (timer / durationItemList [offset].duration);
						pp = positionItemList [offset].transform.GetComponent<BeizerSpline> ().GetPoint (scale);
					}
					break;
				case MoveMethod.Lelp:
					pp = Vector3.Lerp (positionItemList [offset].transform.position, positionItemList [offset + 1].transform.position,curveItemList [offset].curve.Evaluate (timer));
					break;
				case MoveMethod.Slerp:
					pp = Vector3.Slerp (positionItemList [offset].transform.position, positionItemList [offset + 1].transform.position,curveItemList [offset].curve.Evaluate (timer));
					break;
				}

				transform.position =  pp;
				//transform.rotation = Quaternion.Lerp (positionItemList [offset].transform.rotation, positionItemList [offset + 1].transform.rotation, curveItemList [offset].curve.Evaluate (timer));
				//transform.localScale = Vector3.Lerp (positionItemList [offset].transform.localScale, positionItemList [offset + 1].transform.localScale, curveItemList [offset].curve.Evaluate (timer));
				//
				for (int i = 0; i < eventItemList.Count; i++) {
					if (eventItemList [i].targetIndex == offset) {
						for (int j = 0; j < eventItemList [i].eventTimeList.Count; j++) {
							if (eventItemList [i].eventTimeList [j] <= timer) {
								if (eventItemList [i].OccurEvent != null && !eventItemList [i].isUsed [j]) {
									eventItemList [i].OccurEvent.Invoke ();
									eventItemList [i].isUsed [j] = true;
								}
							}
						}
					}
				}

				timer += Time.deltaTime;
			} else {
				timer = 0;
				offset++;
				pp = positionItemList [offset].transform.position;
			}
		} else {
			if (!isPong) {
				isPong = true;
				timer = durationItemList [offset-1].duration;
				offset--;
			}
			if (offset >= 0) {
				if (timer >= 0) {
					//Animate Transform
					switch (moveMethod) {
					case MoveMethod.Flat:
						if (positionItemList [offset].transform.GetComponent<BeizerSpline> () == null)
							pp += Time.deltaTime * (positionItemList [offset].transform.position - positionItemList [offset + 1].transform.position) / durationItemList [offset].duration;
						else {
							var scale = (timer / durationItemList [offset].duration);
							pp = positionItemList [offset].transform.GetComponent<BeizerSpline> ().GetPoint (scale);
						}
						break;
					case MoveMethod.Lelp:
						pp = Vector3.Lerp (positionItemList [offset].transform.position, positionItemList [offset + 1].transform.position,curveItemList [offset].curve.Evaluate (timer));
						break;
					case MoveMethod.Slerp:
						pp = Vector3.Slerp (positionItemList [offset].transform.position, positionItemList [offset + 1].transform.position,curveItemList [offset].curve.Evaluate (timer));
						break;
					}

					transform.position =  pp;
					//transform.rotation = Quaternion.Lerp (positionItemList [offset].transform.rotation, positionItemList [offset + 1].transform.rotation, curveItemList [offset].curve.Evaluate (timer));
					//transform.localScale = Vector3.Lerp (positionItemList [offset].transform.localScale, positionItemList [offset + 1].transform.localScale, curveItemList [offset].curve.Evaluate (timer));
					//
					for (int i = 0; i < eventItemList.Count; i++) {
						if (eventItemList [i].targetIndex == offset) {
							for (int j = 0; j < eventItemList [i].eventTimeList.Count; j++) {
								if (eventItemList [i].eventTimeList [j] >= timer) {
									if (eventItemList [i].OccurEvent != null && eventItemList [i].isUsed [j]) {
										eventItemList [i].OccurEvent.Invoke ();
										eventItemList [i].isUsed [j] = false;
									}
								}
							}
						}
					}
					timer -= Time.deltaTime;
				} else {
					if (offset-1 >= 0) {
						timer = durationItemList [offset-1].duration;
						pp = positionItemList [offset].transform.position;
					}
					offset--;
				}
			} else {
				offset = 0;
				pp = positionItemList [0].transform.position;
				for(int i =0;i<eventItemList.Count;i++){
					eventItemList [i].UndoUsedInfo ();
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
		pp = positionItemList [0].transform.position;
		for(int i=0;i<eventItemList.Count;i++){
			eventItemList [i].StoreUsedInfo ();
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