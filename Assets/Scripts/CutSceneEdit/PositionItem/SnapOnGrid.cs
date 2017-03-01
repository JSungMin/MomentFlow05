using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SnapOnGrid : MonoBehaviour {
	// Update is called once per frame
	public void Snap(){
		var prevPos = transform.position;
		var gridSizeX = CutSceneManager.GetInstance.gridSize.x;
		var gridSizeY = CutSceneManager.GetInstance.gridSize.y;
		prevPos.x = (int)(prevPos.x/gridSizeX)*gridSizeX + gridSizeX*0.5f;
		prevPos.y = (int)(prevPos.y/gridSizeY)*gridSizeY - gridSizeY*0.5f;


		transform.position = prevPos;
	}
}
