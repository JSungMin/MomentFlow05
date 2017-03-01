using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplayManager : MonoBehaviour {

	protected Controller2D controller;

	private bool isPlayer;
	private bool isObstacle;
	private bool isEnemy;

	public bool isQ;
	public bool isW;
	public bool isE;

	public enum SkillState{
		PlayerReplay,
		EnemyReplay,
		TimeStopping,
		Current
	};
	public enum AnimState{
		Idle,
		Run,
		Jump,
		Fall,
		Land,
		Damaged,
		Attack,
		Miss,
		Die,
		Created
	};
	public struct AnimationState{
		public AnimState animState;
		public bool xFlip;
	}


	protected SkillState skillState;
	[HideInInspector]
	public List<Vector3> posList;
	[HideInInspector]
	public List<Vector3> rotationList;
	[HideInInspector]
	public List<AnimationState> animList;
	[HideInInspector]
	public List<bool> itemHandHeldList;


	public int posBack = -1;
	public int rotationBack = -1;
	public int animBack = -1;
	public int itemBack = -1;
	public int eventBack = -1;

	protected bool isNext = true;

	public void SavePosition(){
		posList.Add (transform.position);
		posBack++;
	}
	public void SaveRotation(){
		rotationList.Add (transform.rotation.eulerAngles);
		rotationBack++;
	}
	public void SaveAnimation(AnimationState newState){
			animList.Add (newState);
			animBack++;
	}

	public void SaveItemState(bool _isHand){
		itemHandHeldList.Add (_isHand);
		itemBack++;
	}

	public void DeleteFirstPosition(){
		if (posBack >= 0) {
			posList.RemoveAt (0);
			posBack--;
		}
	}
	public void DeleteFirstRotation(){
		if (rotationBack >= 0) {
			rotationList.RemoveAt (0);
			rotationBack--;
		}
	}
	public void DeleteFirstAnimation(){
		if (animBack >= 0) {
			animList.RemoveAt (0);
			animBack--;
		}
	}
	public void DeleteFirstItemState(){
		if (itemBack >= 0) {
			itemHandHeldList.RemoveAt (0);
			itemBack--;
		}
	}



	public void Replay(){
		if (posBack >= 0) {
			transform.position = posList [posBack];
			posList.RemoveAt (posBack);
			posBack--;
		}
		if (rotationBack >= 0) {
			transform.rotation = Quaternion.Euler (rotationList [rotationBack]);
			rotationList.RemoveAt (rotationBack);
			rotationBack--;
		}
	}
	protected void InitReplayManager(){
		isPlayer = transform.CompareTag ("Player");
		isObstacle = transform.CompareTag ("Obstacle");

		skillState = SkillState.Current;
		posList = new List<Vector3> ();
		animList = new List<AnimationState> ();
		if(!transform.CompareTag("Obstacle"))
			controller = GetComponent<Controller2D> ();
	}

	public void SkillStateMachine(){
		isQ = Input.GetKey (KeyCode.Q);
		isW = Input.GetKey (KeyCode.W);
		isE = Input.GetKey (KeyCode.E);

		if (isQ)
			skillState = SkillState.PlayerReplay;
		if (isW)
			skillState = SkillState.EnemyReplay;
		if (isE)
			skillState = SkillState.TimeStopping;
		if (!isQ && !isW && !isE)
			skillState = SkillState.Current;
	}

	public void ProcessEvent(AnimState aState){
		switch (aState) {
		case AnimState.Created:
			DestroyObject (this.gameObject);
			break;
		case AnimState.Damaged:

			break;
		case AnimState.Die:

			break;
		}
	}

	virtual public void ProcessSkill(){
		Debug.Log ("In Parent");
	}
}
