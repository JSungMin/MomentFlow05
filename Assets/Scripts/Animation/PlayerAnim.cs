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
		setAnimation (0, "Grab", true ,3);
	}

	//TODO: Run -> Climb
	public void SetClimb(){
		setAnimation (0, "Climb", false, 1.25f);
	}
    
	public void SetLoboWalk(){
		setAnimation (0, "Lobo_Walk", true, 1);
	}

	public void SetLoboPunch(){
		setAnimation (0, "Lobo_Punch", false, 1);
	}
	public void SetBackAttack(){
		setAnimation (0, "BackAttack", false, 1);
	}
	public void SetDie(){
		setAnimation (0, "Die", false, 1);
	}
	public void SetJumpDown(){
		setAnimation (0, "Jump_Down",false, 1.5f);
	}
	public void SetJumpUp(){
		setAnimation (0, "Jump_Up", false, 1.5f);
	}
	public void SetGrayColor(){
		GetComponentInChildren<MeshRenderer> ().material.SetColor("_Color",new Color(125,125,125));
	}
	public void SetWhiteColor(){
		GetComponentInChildren<MeshRenderer> ().material.SetColor("_Color",new Color(255,255,255));
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
			if(player.enabled){
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
					SetJumpUp ();
					break;
				case MyObject.State.Fall:
					SetJumpDown ();
					break;
				case MyObject.State.Attack:
					setAnimation (0, "BackAttack", false, 1);
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
}
