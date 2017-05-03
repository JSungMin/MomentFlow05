using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTimeSwitching : MonoBehaviour {

	private TimeLayer timeLayer;

    private Player player;
    private SpriteRenderer sp;

    private BoxCollider2D bc;

	public float disappearAlpah = 0.3f;

    public IEnumerator appearObject;
    public IEnumerator disappearObject;

    public IEnumerator DisappearObject()
    {
        Color tmpAlpha = sp.color;
        var tmpColor = Color.gray;
		tmpColor.a = disappearAlpah;
		while (tmpAlpha.a >= disappearAlpah)
        {
            tmpAlpha.a -= Time.deltaTime;
            tmpAlpha = Color.Lerp(tmpAlpha, tmpColor, Time.deltaTime);
            sp.color = tmpAlpha; 
            yield return new WaitForEndOfFrame();
        }
		tmpAlpha.a = Mathf.Max(disappearAlpah, tmpAlpha.a);
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

    private void Awake()
    {
        bc = GetComponent<BoxCollider2D>();
    }

    // Use this for initialization
    void Start () {
        if (timeLayer == null)
        {
			timeLayer = transform.GetComponentInParent<TimeLayer>();
        }
        sp = GetComponent<SpriteRenderer>();
        player = GameObject.FindObjectOfType<Player>();
        appearObject = AppearObject();
        disappearObject = DisappearObject();
	}
	
	// Update is called once per frame
	void Update () {
		if (TimeLayer.EqualTimeLayer(player.ParentTimeLayer.gameObject,timeLayer.gameObject))
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
