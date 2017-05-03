using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CutSceneManager : MonoBehaviour {
	
	private static CutSceneManager instance;

	public static CutSceneManager GetInstance{
		get{
			if (instance == null) {
				instance = GameObject.FindObjectOfType<CutSceneManager> ();
				if (instance == null) {
					var singleton = new GameObject ("CutSceneManager");
					instance = singleton.AddComponent<CutSceneManager> ();
				}
			}
			return instance;
		}
	}
	public List<CutSceneUnit> sceneUnitsList = new List<CutSceneUnit>();

	public float playTime = 0;


	public Color fontColor = new Color (255,255,255);

	[HideInInspector]
	public Vector3 gridSize = new Vector3(0.16f, 0.16f,0.16f);

	public void PlayCutScene(){
		for(int i =0;i<sceneUnitsList.Count;i++){
			sceneUnitsList [i].StartAction ();
		}
	}

	public void PauseCutScene(){
		for(int i =0;i<sceneUnitsList.Count;i++){
			sceneUnitsList [i].PasueAction ();
		}
	}

	public void StopCutScene(){
		for(int i =0;i<sceneUnitsList.Count;i++){
			sceneUnitsList [i].StopAction ();
		}
	}

	public void ResetList(int id, Rect rect){
		sceneUnitsList.Clear ();
		var objs = GameObject.FindObjectsOfType<CutSceneUnit> ();
		for (int i = 0; i < objs.Length; i++) {
			sceneUnitsList.Add (objs [i]);
		}
	}

	public void ReSortPosition(){
		/*for(int i =0;i<sceneUnitsList.Count;i++){
			for(int j=0;j<sceneUnitsList[i].tracks[sceneUnitsList[i].nowTrackIndex].transform.childCount;j++){
				sceneUnitsList [i].positionItemPool.GetChild (j).name = (j + 1).ToString();
				sceneUnitsList [i].positionItemPool.GetChild (j).GetComponent<HierarchySystem> ().index = j;
				sceneUnitsList [i].positionItemList [Mathf.Max (0, j - 1)] = new Struct.PositionItem (Mathf.Max (0, j - 1), sceneUnitsList [i].positionItemList [Mathf.Max (0, j - 1)].transform);
				if (j >= 2) {
					sceneUnitsList [i].durationItemList [Mathf.Max (0, j - 2)] = new Struct.DurationItem (Mathf.Max (0, j - 2), sceneUnitsList [i].durationItemList [Mathf.Max (0, j - 2)].duration);
					sceneUnitsList [i].curveItemList [Mathf.Max (0, j - 2)] = new Struct.CurveItem (Mathf.Max (0, j - 2), sceneUnitsList [i].curveItemList [Mathf.Max (0, j - 2)].curve);
				}
			}
		}*/
	}
	public void Update(){
		playTime += Time.deltaTime;
	}
}
