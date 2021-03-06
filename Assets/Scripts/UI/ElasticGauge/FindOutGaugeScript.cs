using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FindOutGaugeScript : MonoBehaviour {

	public EnemyScript escr;

	public float maxWidth = 150;
	public float nowWidth = 0;
	private const float offsetX = 75;
	private const float offsetY = 25;

	public Gradient egGradient;

	private UI2DSprite gaugeImage;

	public void InitFindOutGaugeScript(EnemyScript e){
		escr = e;
		gaugeImage = GetComponent<UI2DSprite> ();
	}

	public void FollowEnemyObject(){
		var tmpPos = escr.transform.position;
		tmpPos.z = transform.position.z;
		transform.position = tmpPos;
		transform.localPosition -= new Vector3 (offsetX, offsetY);
	}

	// Update is called once per frame
	void Update () {
		gaugeImage.width = (int)(maxWidth * (Mathf.Min(escr.findOutGauge*0.01f,1)));
		gaugeImage.color = egGradient.Evaluate ((escr.findOutGauge * 0.01f));
		FollowEnemyObject ();
	}
}
