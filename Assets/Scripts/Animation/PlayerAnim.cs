using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// play 할 수 있는 모든 케릭터들의 애니메이션을 컨트롤하는 클래스 (알릭, 셀마, 에일린)
public class PlayerAnim : AnimationBase {
	Player player;

    private AudioSource audioSource;

	bool isAttack;
	bool isDamaged;

    private new void Awake()
    {
        base.Awake();
        player = GetComponent<Player>();
        audioSource = GetComponent<AudioSource>();
    }

	public void SetDefaultIdle(){
		setAnimation (0, "Idle", true, 1);
	}

	public void SetDefaultSit(){
		setAnimation (0, "Sit", true, 1);
	}

	public void SetDefaultHide(){
		setAnimation (0, "Hide", true, 1);
	}

	public void SetDefaultSitWalk(){
		setAnimation (0, "Run", true, 1);
	}

	public void SetDefaultWalk(){
		if (player.isSit) {
			SetDefaultSitWalk ();
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
    
	void Update () {
        if (curAnimation[1] == "Walk")
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
			case MyObject.State.Attack:
				setAnimation (0, "Lobo_Punch", false, 1);
				break;
			case MyObject.State.GrabCorner:
				SetGrabCorner ();
				break;
			case MyObject.State.ClimbLadder:
				setAnimation (0,"Run", true, 1);
				break;
			case MyObject.State.ClimbCorner:
				SetClimb ();
				break;
			}
		}
	}
}
