using System.Collections;
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
        if (isToLeft)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    public void StopAnimation()
    {
        SetAnimationTimeScale(0);
    }

    public void SetAnimationTimeScale(float value)
    {
        skel.AnimationState.TimeScale = value;
    }

    public void PastColor()
    {
        setAnimation(0, "PastColor", false, 1);
    }

    public void PresentColor()
    {
        setAnimation(0, "PresentColor", false, 1);
    }
    
    public void Idle()
    {
        setAnimation(1, charAnimName + "Idle", true, 1);
    }

    public void Walk()
    {
        setAnimation(1, charAnimName + "Walk", true, 1);
    }

    public void SuspiciousWalk()
    {
        setAnimation(1, charAnimName + "Suspicious_Walk", true, 1);
    }

    public void Run()
    {
        setAnimation(1, charAnimName + "Run", true, 1);
    }

    public void Shoot()
    {
        setAnimation(1, charAnimName + "Shoot", true, 1);
    }

    public void Stun()
    {
        setAnimation(1, charAnimName + "Stun", false, 1);
    }

    public void SetStrangle()
    {
        setAnimation(1, "Strangle", false, 1);
    }
}