using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class GrayScaleEffect : MonoBehaviour {

	public float intensity;
	private Material material;

	private bool isActived;

	public bool IsActived {
		get {
			return isActived;
		}
		set {
			isActived = value;
		}
	}

	void Awake(){
		material = new Material (Shader.Find("Hidden/GrayScaleImageEffect"));
	}

	void OnRenderImage (RenderTexture source, RenderTexture destination){
		if(intensity == 0){
			Graphics.Blit (source,destination);
		}

		material.SetFloat ("_bwBlend",intensity);
		Graphics.Blit (source,destination,material);
	}

	void Update(){
		if (isActived) {
			intensity = Mathf.Lerp (intensity, 1, Time.deltaTime * 2);
		} else {
			intensity = Mathf.Lerp (intensity, 0, Time.deltaTime * 2);
		}
	}
}
