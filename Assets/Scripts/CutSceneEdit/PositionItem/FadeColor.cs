using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif
[ExecuteInEditMode]
public class FadeColor : BaseUnit {
	public Gradient color;

	bool isSprite = false;
	bool isUGUI = false;
	bool isNGUI = false;

	[HideInInspector]
	public Texture2D tex;
	void OnEnable(){
		InitUnit ();
		if (hi.unit.GetComponent<SpriteRenderer> () != null)
			isSprite = true;
		else if (hi.unit.GetComponent<Image> () != null)
			isUGUI = true;
		else if (hi.unit.GetComponent<UI2DSprite> () != null)
			isNGUI = true;
	}
		
	void Update(){
		#if UNITY_EDITOR
		if ((isSprite || isUGUI||isNGUI)&&transform.GetComponentInParent<PositionPool>().offset == hi.index&&EditorApplication.isPlaying) {
			if (hi.unit.timer >= startTime) {
				if (isSprite) {
					hi.unit.GetComponent<SpriteRenderer> ().color = color.Evaluate ((hi.unit.timer - startTime) / endTime);
				} else if (isUGUI) {
					hi.unit.GetComponent<Image> ().color = color.Evaluate ((hi.unit.timer - startTime) / endTime);
				}else if(isNGUI){
					hi.unit.GetComponent<UI2DSprite>().color = color.Evaluate ((hi.unit.timer - startTime) / endTime);
				}
			}
		} else if(!isSprite&&!isUGUI&&!isNGUI){
			Debug.LogWarning ("This component only work with Image or SpriteRenderer or UI2DSprite");
		}
		#endif
		if ((isSprite || isUGUI||isNGUI)&&hi.unit.transform.GetComponentInParent<PositionPool>().offset == hi.index) {
			if (hi.unit.timer >= startTime) {
				if (isSprite) {
					hi.unit.GetComponent<SpriteRenderer> ().color = color.Evaluate ((hi.unit.timer - startTime) / endTime);
				} else if (isUGUI) {
					hi.unit.GetComponent<Image> ().color = color.Evaluate ((hi.unit.timer - startTime) / endTime);
				}else if(isNGUI){
					hi.unit.GetComponent<UI2DSprite>().color = color.Evaluate ((hi.unit.timer - startTime) / endTime);
				}
			}
		} else if(!isSprite&&!isUGUI&&!isNGUI){
			Debug.LogWarning ("This component only work with Image or SpriteRenderer or UI2DSprite");
		}
	}
}
