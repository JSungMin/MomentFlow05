using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Spine;
using Spine.Unity;
using Spine.Unity.Modules;

public class AnimationBase : MonoBehaviour {

	public int characterIndex;

	public List<string> characterName;

	public SkeletonAnimation skel;

	protected string[] cur_animation;

	protected enum Direction{
		Left,
		Right
	}
	protected Direction dir;

	public bool isCutScene;

	public void setAnimation(int index, string name, bool loop, float time)
	{
		if(cur_animation[index] == name)
		{
			return;
		}else
		{
			skel.state.SetAnimation(index, name, loop).TimeScale = time;
			cur_animation[index] = name;
		}
	}

	public void SetDir(bool isToLeft){
		if (dir != Direction.Left&&isToLeft) {
			skel.state.ClearTracks ();
			dir = Direction.Left;
			transform.localScale = new Vector3 (-Mathf.Abs (transform.localScale.x), transform.localScale.y, transform.localScale.z);
		} else {
			skel.state.ClearTracks ();
			dir = Direction.Right;
			transform.localScale = new Vector3 (Mathf.Abs (transform.localScale.x), transform.localScale.y, transform.localScale.z);
		}
	}

	void Awake(){
		skel = GetComponentInChildren<SkeletonAnimation> ();
		cur_animation = new string[2];
	}
}
