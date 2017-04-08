﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnim : AnimationBase {
	Player player;

    private AudioSource audioSource;

	bool isAttack;
	bool isDamaged;
		
	// Use this for initialization
	void Start () {
		player = GetComponent<Player> ();		
        audioSource = GetComponent<AudioSource>();
    }

	public void SetSit(){
		setAnimation (0, "Sit", true, 1);
	}

	public void SetHide(){
		setAnimation (0, "Hide", true, 1);
	}

	public void SetSitWalk(){
		setAnimation (0, "Run", true, 1);
		//TODO : setAnimation (0, "Sit_Walk",true,1);
	}

	public void SetWalk(){
		if (player.isSit) {
			SetSitWalk ();
		} else {
			setAnimation (0, "Run", true, 1);
		}
    }

	//TODO: Run -> GrabCorner
	public void SetGrabCorner(){
		setAnimation (0, "Run", true ,1);
	}

	//TODO: Run -> Climb
	public void SetClimb(){
		setAnimation (0, "Run", true, 1);
	}

	// Update is called once per frame
	void Update () {
        if (cur_animation[1] == "Walk")
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

			if (pastDir != dir) {
				//skel.state.ClearTracks ();
				//setAnimation (0, characterName [characterIndex] + "_" + dir.ToString (), true, 1);
			}

			switch(player.state){
			case MyObject.State.Walk:
				setAnimation (0, "Run", true, 1);
				break;
			case MyObject.State.Idle:
				setAnimation (0, "Idle", true, 1);
				break;
			case MyObject.State.Sit:
				setAnimation (0, "Sit", true, 1);
				break;
			case MyObject.State.Jump:
				
				break;
			case MyObject.State.Fall:

				break;
			case MyObject.State.GrabCorner:
				SetGrabCorner ();
				break;
			case MyObject.State.ClimbCorner:
				SetClimb ();
				break;
			}
		}
	}
}
