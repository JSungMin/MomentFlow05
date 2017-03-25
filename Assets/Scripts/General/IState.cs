﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IState
{
    protected GameObject enemyObj;
    protected EnemyScript enemyScript;

	protected Vector3 dir;
	protected float speed;

    // TODO!!!!!!!!!!!!!!!!!!!!!!!!!!
    // 부모에서 awake를 구현하고 자식에서 따로 오버라이드 하지 않으면 아마 부모의 awake가 불릴걸? 시험해봐야함
    public IState() { }


    public IState(GameObject obj)
    {
        enemyObj = obj;
		enemyScript = enemyObj.GetComponent<EnemyScript> ();
		if(enemyScript == null){
			enemyScript = enemyObj.transform.GetComponentInChildren<EnemyScript> ();
			if(enemyScript == null){
				Debug.LogError (enemyObj.name + " dosen't have EnemyScript");
			}
		}
    }

	public void Idle(){
		enemyScript.anim.setAnimation (0, "Idle", true, 1);
	}

    public void Hold()
    {
		enemyScript.anim.setAnimation (0, "Idle", true, 1);
    }

	public void Walk(Transform t,Vector3 velocity)
    {
		enemyScript.anim.setAnimation (0, "Walk", true, 1);
		t.localScale = new Vector3 (Mathf.Sign (velocity.x) * Mathf.Abs(t.localScale.x), t.localScale.y, t.localScale.z);
		t.Translate(velocity * Time.deltaTime);
    }

	public void SurpriseStart(){
		enemyScript.anim.setAnimation (0,"Sit",false,1);
		Debug.Log ("SurpriseStart");
		//TODO:아래 함수로 바꿔야 함 아직 Surprise Animation이 없음
		//enemyScript.anim.setAnimation (0,"Surprise",false,1);
	}
	public void LookAround(){

	}

    public abstract void OnStateEnter(GameObject obj);
    public abstract void OnStateStay(GameObject obj);
    public abstract void OnStateExit(GameObject obj);
	public abstract State ChangeState(State obj);
	public abstract bool CheckState (GameObject obj);
}