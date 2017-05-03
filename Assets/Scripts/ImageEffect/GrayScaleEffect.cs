using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class GrayScaleEffect : MonoBehaviour {

	public float intensity;
	private Material material;

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
}
