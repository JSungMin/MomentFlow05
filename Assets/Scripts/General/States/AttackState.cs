using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : IState {

	public EnemyAttackType attackType;
	public GameObject bullet;

	private Transform enemyBulletPool;

	private int fireNum = 0;

	private float fireDelay;
	private float fireDelayTimer = 0;


	public AttackState (GameObject obj) : base (obj){}

	public void InitAttackInfo(EnemyAttackType type, GameObject bulletPref, int fireN){
		attackType = type;
		bullet = bulletPref;
		fireDelay = enemyScript.attackDelay;
		fireNum = fireN;
		enemyScript.fireBullets = enemyScript.FireBullets (fireNum);
	}
	public override void OnStateEnter(){
		fireDelayTimer = fireDelay;	
		enemyBulletPool = enemyObj.transform.parent.GetChild (0);
		enemyScript.AimToObject (enemyScript.playerObject.transform.position);
	}

	public override void OnStateStay(){
		if (!enemyScript.isAttack) {
			var bInfo = enemyScript.Browse (enemyScript.moveSpeed * 1.2f);
			if (enemyScript.attackRange <= Vector2.Distance (enemyScript.playerObject.transform.position, enemyObj.transform.position)) {
				if (bInfo.layer != LayerMask.NameToLayer ("Collision")) {
					Walk (enemyScript.charAnimName+"_Run", enemyObj.transform, (enemyScript.playerObject.transform.position - enemyObj.transform.position).normalized * enemyScript.moveSpeed * 1.2f);
				}
			} else {
				if (fireDelayTimer >= fireDelay) {
					if (attackType == EnemyAttackType.Gun) {
						enemyScript.fireBullets = enemyScript.FireBullets (fireNum);
						enemyScript.anim.setAnimation (1, enemyScript.charAnimName + "_Shoot", true, 1);
						enemyScript.StartCoroutine (enemyScript.fireBullets);
						fireDelayTimer = 0;
					} else {
					}
				} else {
					fireDelayTimer += Time.deltaTime;
					enemyScript.anim.setAnimation (1, enemyScript.charAnimName + "_Idle", true, 1);
				}
			}
		}
	}

	public override void OnStateExit(){
		fireDelayTimer = fireDelay;
		enemyScript.StopCoroutine (enemyScript.fireBullets);
		enemyScript.fireBullets = enemyScript.FireBullets (fireNum);
		enemyScript.AimToForward ();
	}
	public override State ChangeState(State nowState){
		if (nowState == State.Attack) {
			OnStateStay ();
		} else {
			if(CheckState())
				OnStateEnter ();
		}
		return State.Attack;	
	}
	public override bool CheckState (){
		return true;
	}
}
