using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleDialogue : MonoBehaviour
{
    public GameObject followObject;

	public TweenAlpha alpha;
	public TweenPosition position;

	public void StartBubble(){
		alpha.PlayForward ();
		position.PlayForward ();

		GetComponent<Paging> ().NextPage ();
	}

    public void NextPage()
    {
        GetComponent<Paging>().NextPage();
    }

	public void EndBubble(){
		alpha.PlayReverse ();
		position.PlayForward ();
		GetComponent<Paging> ().StopShowContent ();
	}

    public int GetContentCount()
    {
        return GetComponent<Paging>().GetContentCount();
    }

    private void Update()
    {
		var followPos = followObject.transform.position + Vector3.up * 0.5f;
		followPos.z = transform.position.z;
		transform.position = followPos;
        //transform.localScale = Vector3.one;
        //transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0.0f);
    }
}
