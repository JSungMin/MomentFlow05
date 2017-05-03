using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatDialogue : MonoBehaviour
{
	private TweenAlpha tweenAlpha;

	private Paging paging;

	private void Awake()
	{
		tweenAlpha = GetComponentInChildren<TweenAlpha>();

		tweenAlpha.enabled = false;

		paging = GetComponent<Paging>();
		tweenAlpha.GetComponent<UI2DSprite>().alpha = 0;
	}

	// Chat 켜고 dialogue를 시작한다
	public void StartChat(){
		tweenAlpha.PlayForward();
		paging.NextPage();
	}

	public void NextPage()
	{
		Debug.Log ("Next");
		paging.NextPage();
	}

	public void HidePage(){
		tweenAlpha.PlayReverse ();
	}

	// dialogue를 끝내고 Chat 끈다
	public void EndChat(){
		tweenAlpha.PlayReverse ();
		paging.StopShowContent ();
	}

	public int GetContentCount()
	{
		return paging.GetContentCount();
	}
}
