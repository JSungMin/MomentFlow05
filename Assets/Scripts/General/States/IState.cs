﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IState
{
    protected GameObject enemyObj;
    protected EnemyScript enemyScript;

	public Vector3 dir;
	protected float speed;

    // TODO!!!!!!!!!!!!!!!!!!!!!!!!!!
    // 부모에서 awake를 구현하고 자식에서 따로 오버라이드 하지 않으면 아마 부모의 awake가 불릴걸? 시험해봐야함
	public IState(){ }


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
		enemyScript.anim.Idle (enemyScript.charAnimName);
	}

	public void Hold()
    {
		enemyScript.anim.Idle (enemyScript.charAnimName);
    }

	public void Walk(Transform t,Vector3 velocity)
    {
		enemyScript.anim.Walk (enemyScript.charAnimName);
		t.localScale = new Vector3 (Mathf.Sign (velocity.x) * Mathf.Abs(t.localScale.x), t.localScale.y, t.localScale.z);
		t.Translate(velocity * Time.deltaTime);
    }
	public void SuspiciousWalk(Transform t,Vector3 velocity)
	{
		enemyScript.anim.SuspiciousWalk(enemyScript.charAnimName);
		t.localScale = new Vector3 (Mathf.Sign (velocity.x) * Mathf.Abs(t.localScale.x), t.localScale.y, t.localScale.z);
		t.Translate(velocity * Time.deltaTime);
	}
	public void Run(Transform t,Vector3 velocity)
	{
		enemyScript.anim.Run(enemyScript.charAnimName);
		t.localScale = new Vector3 (Mathf.Sign (velocity.x) * Mathf.Abs(t.localScale.x), t.localScale.y, t.localScale.z);
		t.Translate(velocity * Time.deltaTime);
	}

	public void LookAround(){

	}

    public abstract void OnStateEnter();
    public abstract void OnStateStay();
    public abstract void OnStateExit();
	public abstract State ChangeState(State obj);
	public abstract bool CheckState ();
}