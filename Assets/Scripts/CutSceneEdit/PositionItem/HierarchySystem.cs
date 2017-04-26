using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class HierarchySystem : MonoBehaviour {
	[HideInInspector]
	public CutSceneUnit unit;
	public PositionPool positionPool;
	private Transform parent;
	public int index;

	public void SetParent(Transform p,CutSceneUnit u){
		parent = p;
		positionPool = parent.GetComponent<PositionPool> ();
		unit = u;
		transform.localScale = positionPool.transform.localScale;
	}
	public void CleanHierachy(){
		for(int i =0;i<parent.childCount;i++){
			if (parent.GetChild (i).GetComponent<HierarchySystem> () == null)
				DestroyImmediate (parent.GetChild (i).gameObject);
		}
	}
	/*void OnDestroy(){
		transform.GetComponentInParent<PositionPool>().size = Mathf.Max(0,transform.GetComponentInParent<PositionPool>().size-1);
		try{
			transform.GetComponentInParent<PositionPool>().positionItemList.RemoveAt (Mathf.Max(0,index));
			transform.GetComponentInParent<PositionPool>().durationItemList.RemoveAt (Mathf.Max(0,index - 1));
			transform.GetComponentInParent<PositionPool>().curveItemList.RemoveAt (Mathf.Max (0, index - 1));
			CutSceneManager.GetInstance.ReSortPosition ();
		}catch(Exception e){
			Debug.LogWarning ("Destroy Error");
		}
	}*/
}
