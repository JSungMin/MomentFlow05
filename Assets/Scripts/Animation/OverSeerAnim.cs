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
    
    public BubbleDialogue bubble;

    private void Awake()
    {
        skel = GetComponentInChildren<SkeletonAnimation>();
        SetDir(true);
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
    
    public IEnumerator MakeWalkAndStopAndTalkCo()
    {
        setAnimation(0, "Idle", true, 1);
        yield return new WaitForSeconds(1.0f);

        SetDir(false);
        setAnimation(0, "Walk", true, 1);
        yield return StartCoroutine(MakeWalkFor(1.0f));

        setAnimation(0, "Idle", true, 1);

        yield return new WaitForSeconds(0.0f);

        bubble.StartBubble();

        for (int i = 0; i < bubble.GetContentCount() - 1; i++)
        {
            yield return new WaitForSeconds(2.0f);
            bubble.NextPage();
        }

        yield return new WaitForSeconds(1.0f);
    }

    private IEnumerator MakeWalkFor(float sec)
    {
        float now = 0.0f;
        while (now < sec)
        {
            now += 0.01f;
            transform.Translate(Vector3.left * 0.01f);
            yield return new WaitForSeconds(0.01f);
        }
    }
}
