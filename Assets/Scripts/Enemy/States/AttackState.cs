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

	public void InitAttackInfo(EnemyAttackType type, GameObject bulletPref, int fireN) {
		attackType = type;
		bullet = bulletPref;
		fireDelay = enemyScript.attackDelay;
		fireNum = fireN;
		enemyScript.fireBullets = enemyScript.FireBullets (fireNum);
	}

	private bool IsOnAttackableRange ()
	{
		return enemyScript.attackRange <= Vector2.Distance (enemyScript.playerObject.transform.position, enemyObj.transform.position);
	}
	
	private void GunShoot ()
	{
		if (attackType == EnemyAttackType.Gun) {
			enemyScript.fireBullets = enemyScript.FireBullets (fireNum);
			enemyScript.anim.Shoot ();
			enemyScript.StartCoroutine (enemyScript.fireBullets);
			fireDelayTimer = 0;
		}
	}
	
	private bool IsCoolTimeOver ()
	{
		return fireDelayTimer >= fireDelay;
	}

	public override void OnStateEnter(){
		fireDelayTimer = fireDelay;	
		enemyBulletPool = enemyObj.transform.parent.GetChild (0);
		enemyScript.AimToObject (enemyScript.playerObject.transform.position);
	}


	public override void OnStateStay(){
		enemyScript.AimToObject (enemyScript.playerObject.transform.position);
		if (!enemyScript.isAttack) {
			var bInfo = enemyScript.FindNearestCollisionInBrowseInfos (enemyScript.moveSpeed * 1.2f);
			if (IsOnAttackableRange ()) {
				if (bInfo.layer != LayerMask.NameToLayer ("Collision")) {
					dir = (enemyScript.playerObject.transform.position - enemyObj.transform.position).normalized;
					dir.x = (dir.x > 0) ? 1 : ((dir.x < 0) ? -1 : 0);
					Run (enemyObj.transform, dir * enemyScript.moveSpeed * 1.2f);
				}
			} else {
				Idle ();
				if (IsCoolTimeOver ()) {
					GunShoot ();
				} 
				else {
					fireDelayTimer += Time.deltaTime;
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
