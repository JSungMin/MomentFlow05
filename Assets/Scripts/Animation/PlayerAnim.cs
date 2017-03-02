using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Spine;
using Spine.Unity;
using Spine.Unity.Modules;

public class PlayerAnim : MonoBehaviour {

	public int characterIndex;

	public List<string> characterName;

	SkeletonAnimation skel;
	Player player;

    private AudioSource audioSource;

	enum Direction{
		Left,
		Right
	}

	bool isAttack;
	bool isDamaged;

	public bool isSit;

	string cur_animation;
	Direction dir;

	public bool isCutScene;

	void setAnimation(int index, string name, bool loop, float time)
	{
		if(cur_animation == name)
		{
			return;
		}else
		{
			skel.state.SetAnimation(index, name, loop).TimeScale = time;
			cur_animation = name;
		}
	}
		
	// Use this for initialization
	void Start () {
		player = GetComponent<Player> ();		
		skel = GetComponentInChildren<SkeletonAnimation>();
        audioSource = GetComponent<AudioSource>();

    }

	public void SetDir(bool isToLeft){
		if (isToLeft) {
			player.transform.localScale = new Vector3 (-Mathf.Abs (player.transform.localScale.x), player.transform.localScale.y, player.transform.localScale.z);
		} else {
			player.transform.localScale = new Vector3 (Mathf.Abs (player.transform.localScale.x), player.transform.localScale.y, player.transform.localScale.z);
		}
	}

	public void SetSit(){
		setAnimation (0, "Sit", false, 1);
		isSit = true;
	}

	public void SetWalk(){
		setAnimation (0, "Walk", true, 1);
    }

	// Update is called once per frame
	void Update () {
        if (cur_animation == "Walk")
        {
            if (!audioSource.isPlaying)
                audioSource.Play();
        }
        else
            audioSource.Stop();

        if (!isCutScene) {
			var inputX = player.input.x;
			var pastDir = dir;

			if (inputX > 0) {
				dir = Direction.Right;
				transform.localScale = new Vector3 (1, transform.localScale.y, transform.localScale.z);
			} else if (inputX < 0) {
				dir = Direction.Left;
				transform.localScale = new Vector3 (-1, transform.localScale.y, transform.localScale.z);
			}

			if (Input.GetKeyDown (KeyCode.Space)) {
				if (!isSit) {
					setAnimation (0, "Sit", false, 1);
					isSit = true;
				}
			}

			if (pastDir != dir) {
				skel.state.ClearTracks ();
				setAnimation (0, characterName [characterIndex] + "_" + dir.ToString (), true, 1);
			}

			if (inputX != 0) {
				if (isSit) {
					isSit = false;
					skel.skeleton.SetToSetupPose ();
				}
				setAnimation (0, "Walk", true, 1);
			} else {
				if (!isSit)
					setAnimation (0, "Idle", true, 1);
				else {
					setAnimation (0, "Sit", true, 1);
				}
			}
		}
	}
}
