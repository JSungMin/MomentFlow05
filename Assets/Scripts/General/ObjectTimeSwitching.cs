using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTimeSwitching : MonoBehaviour {

    public TimeLayer timeLayer;

    private Player player;
    private SpriteRenderer sp;

    public IEnumerator appearObject;
    public IEnumerator disappearObject;

    public IEnumerator DisappearObject()
    {
        Color tmpAlpha = sp.color;
        var tmpColor = Color.gray;
        tmpColor.a = 0.3f;
        while (tmpAlpha.a >= 0.3f)
        {
            tmpAlpha.a -= Time.deltaTime;
            tmpAlpha = Color.Lerp(tmpAlpha, tmpColor, Time.deltaTime);
            sp.color = tmpAlpha; 
            yield return new WaitForEndOfFrame();
        }
        tmpAlpha.a = Mathf.Max(0.3f, tmpAlpha.a);
        tmpAlpha = tmpColor;
        sp.color = tmpAlpha;
    }
    public IEnumerator AppearObject()
    {
        Color tmpAlpha = sp.color;
        var tmpColor = Color.white;
        tmpColor.a = 1;
        while (tmpAlpha.a <= 1)
        {
            tmpAlpha.a += Time.deltaTime;
            tmpAlpha = Color.Lerp(tmpAlpha, tmpColor, Time.deltaTime);
            sp.color = tmpAlpha;
            yield return new WaitForEndOfFrame();
        }
        tmpAlpha.a = Mathf.Min(1, tmpAlpha.a);
        tmpAlpha = tmpColor;
        sp.color = tmpAlpha;
    }

	// Use this for initialization
	void Start () {
        if (timeLayer == null)
        {
            timeLayer = GetComponent<TimeLayer>();
        }
        sp = GetComponent<SpriteRenderer>();
        player = GameObject.FindObjectOfType<Player>();
        appearObject = AppearObject();
        disappearObject = DisappearObject();
	}
	
	// Update is called once per frame
	void Update () {
        if (TimeLayer.EqualTimeLayer(player.transform.gameObject,gameObject))
        {
            StopCoroutine(disappearObject);
            appearObject = AppearObject();
            StartCoroutine(appearObject);
        }else
        {
            StopCoroutine(appearObject);
            disappearObject = DisappearObject();
            StartCoroutine(disappearObject);
        }
	}
}
