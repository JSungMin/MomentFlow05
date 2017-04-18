﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Spine;
using Spine.Unity;
using Spine.Unity.Modules;

public class AnimationBase : MonoBehaviour
{
	public SkeletonAnimation skel { private set; get; }
	protected string[] curAnimation;
    public string charAnimName;

    protected enum Direction
    {
        Left,
        Right
    }

	protected Direction dir;
	public bool isCutScene { private set; get; }
    
    // base 클래스들은 awake 함수를 재정의하고 base.Awake()를 호출하여야 한다
    protected void Awake()
    {
        skel = GetComponentInChildren<SkeletonAnimation>();
        curAnimation = new string[2];
    }

    public void MakeIsCutSceneTrue()
    {
        isCutScene = true;
    }

    public void setAnimation(int trackIndex, string animationName, bool loop, float time)
	{
        if (curAnimation[trackIndex] == animationName)
        {
            return;
        }
        else
        {
            skel.state.SetAnimation(trackIndex, animationName, loop).TimeScale = time;
            curAnimation[trackIndex] = animationName;
        }
	}

    public void SetDir(bool isToLeft)
    {
        if (dir != Direction.Left && isToLeft)
        {
            skel.state.ClearTracks();
            dir = Direction.Left;
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            skel.state.ClearTracks();
            dir = Direction.Right;
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

	public void PastColor(){
		setAnimation (0, "PastColor", false, 1);
	}

	public void PresentColor(){
		setAnimation (0, "PresentColor", false, 1);
	}

    // idle 애니메이션을 실행시킨다
	public void Idle(){
        setAnimation(1, charAnimName + "_Idle", true, 1);
	}

	public void Walk(){
		setAnimation (1, charAnimName + "_Walk", true, 1);
	}

	public void SuspiciousWalk(){
        setAnimation(1, charAnimName + "_Suspicious_Walk", true, 1);
	}

	public void Run(){
		setAnimation (1, charAnimName + "_Run", true, 1);
	}

	public void Shoot(){
        setAnimation(1, charAnimName + "_Shoot", true, 1);
	}

	public void Stun(){

	}
}
