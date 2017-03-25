using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnim : AnimationBase {
	Player player;

    private AudioSource audioSource;

	bool isAttack;
	bool isDamaged;

	public bool isSit;
		
	// Use this for initialization
	void Start () {
		player = GetComponent<Player> ();		
        audioSource = GetComponent<AudioSource>();
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

			if (pastDir != dir) {
				skel.state.ClearTracks ();
				setAnimation (0, characterName [characterIndex] + "_" + dir.ToString (), true, 1);
			}

			if (player.state == MyObject.State.Walk) {
				setAnimation (0, "Walk", true, 1);
			} else if(player.state == MyObject.State.Sit){
				setAnimation (0, "Sit", true, 1);
			}else if(player.state == MyObject.State.Idle){
				setAnimation (0, "Idle", true, 1);
			}
		}
	}
}
