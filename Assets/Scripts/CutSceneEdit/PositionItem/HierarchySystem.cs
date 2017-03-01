using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class HierarchySystem : MonoBehaviour {
	[HideInInspector]
	public CutSceneUnit unit;
	private Transform parent;
	public int index;

	public void SetParent(Transform p,CutSceneUnit u){
		parent = p;
		unit = u;
		transform.localScale = u.transform.localScale;
	}
	public void CleanHierachy(){
		for(int i =0;i<parent.childCount;i++){
			if (parent.GetChild (i).GetComponent<HierarchySystem> () == null)
				DestroyImmediate (parent.GetChild (i).gameObject);
		}
	}
	void OnDestroy(){
		unit.size = Mathf.Max(0,unit.size-1);
		try{
			unit.positionItemList.RemoveAt (Mathf.Max(0,index));
			unit.durationItemList.RemoveAt (Mathf.Max(0,index - 1));
			unit.curveItemList.RemoveAt (Mathf.Max (0, index - 1));
			CutSceneManager.GetInstance.ReSortPosition ();
		}catch(Exception e){
			Debug.LogWarning ("Destroy Error");
		}
	}
}
