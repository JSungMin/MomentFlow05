﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IState
{
	public int layerLevel;

    protected GameObject enemyObj;
    protected EnemyScript enemyScript;

	protected Vector3 defaultDir;
	protected Vector3 defaultPosition;

	public Vector3 dir;
	protected float speed;

    // TODO!!!!!!!!!!!!!!!!!!!!!!!!!!
    // 부모에서 awake를 구현하고 자식에서 따로 오버라이드 하지 않으면 아마 부모의 awake가 불릴걸? 시험해봐야함
	public IState(){ }
    
	public IState(GameObject obj) 
	{
        enemyObj = obj;
		defaultDir = obj.transform.localScale;
		defaultPosition = obj.transform.position;
		enemyScript = enemyObj.GetComponent<EnemyScript> ();
		if(enemyScript == null){
			enemyScript = enemyObj.transform.GetComponentInChildren<EnemyScript> ();
			if(enemyScript == null){
				Debug.LogError (enemyObj.name + " dosen't have EnemyScript");
			}
		}
    }

	public void Idle() 
	{
        enemyScript.anim.Idle();
		enemyScript.velocity.x = 0;
	}

	public void Hold()
    {
        enemyScript.anim.Idle();
		enemyScript.velocity.x = 0;
    }

	public void Walk(Transform t,Vector3 velocity)
    {
		enemyScript.velocity.x = velocity.x;
        enemyScript.anim.Walk();
		t.localScale = new Vector3 (Mathf.Sign (velocity.x) * Mathf.Abs(t.localScale.x), t.localScale.y, t.localScale.z);
    }

    public void SuspiciousWalk(Transform t,Vector3 velocity)
	{
		enemyScript.velocity.x = velocity.x;
		enemyScript.anim.SuspiciousWalk();
        t.localScale = new Vector3 (Mathf.Sign (velocity.x) * Mathf.Abs(t.localScale.x), t.localScale.y, t.localScale.z);
	}

    public void Run(Transform t,Vector3 velocity)
	{
		enemyScript.velocity.x = velocity.x;
		enemyScript.anim.Run();
        t.localScale = new Vector3 (Mathf.Sign (velocity.x) * Mathf.Abs(t.localScale.x), t.localScale.y, t.localScale.z);
	}

    public void Stun(Transform t, Vector3 velocity)
    {
        enemyScript.velocity = velocity;
        // 단어 통일하자 stun 이랑 strangle이 있다
        enemyScript.anim.SetStrangle();
    }

	public void LookAround()
    {
	}

    public abstract void OnStateEnter();
    public abstract void OnStateStay();
    public abstract void OnStateExit();
	public abstract State ChangeState(State obj);
	public abstract bool CheckState ();
}