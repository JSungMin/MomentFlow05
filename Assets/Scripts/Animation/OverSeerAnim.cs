using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Spine;
using Spine.Unity;
using Spine.Unity.Modules;

public class OverSeerAnim : MonoBehaviour
{
    private SkeletonAnimation skel;
    private string cur_animation;

    private void Awake()
    {
        skel = GetComponentInChildren<SkeletonAnimation>();
    }

    private void setAnimation(int index, string name, bool loop, float time)
    {
        if (cur_animation == name)
        {
            return;
        }
        else
        {
            skel.state.SetAnimation(index, name, loop).TimeScale = time;
            cur_animation = name;
        }
    }

    public void SetWalk()
    {
        setAnimation(0, "Walk", true, 1);
    }

    public void SetIdle()
    {
        setAnimation(0, "Idle", true, 1);
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
}
