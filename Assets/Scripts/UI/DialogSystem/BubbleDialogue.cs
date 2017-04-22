using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleDialogue : MonoBehaviour
{
    public GameObject followObject;

	private TweenAlpha tweenAlpha;
    private TweenPosition tweenPosition;

    private Paging paging;

    private void Awake()
    {
        tweenAlpha = GetComponentInChildren<TweenAlpha>();
        tweenPosition = GetComponentInChildren<TweenPosition>();

        tweenAlpha.enabled = false;
        tweenPosition.enabled = false;

        paging = GetComponent<Paging>();
        tweenAlpha.GetComponent<UI2DSprite>().alpha = 0;
    }

    // bubble을 켜고 dialogue를 시작한다
    public void StartBubble(){
        tweenAlpha.PlayForward();
        tweenPosition.PlayForward();
        paging.NextPage();
	}

    public void NextPage()
    {
        paging.NextPage();
    }

    // dialogue를 끝내고 bubble을 끈다
    public void EndBubble(){
        tweenAlpha.PlayReverse ();
        tweenPosition.PlayForward ();
        paging.StopShowContent ();
	}

    public int GetContentCount()
    {
        return paging.GetContentCount();
    }

    private void Update()
    {
		var followPos = followObject.transform.position;
		followPos.z = transform.position.z;
		transform.position = followPos;
    }
}
