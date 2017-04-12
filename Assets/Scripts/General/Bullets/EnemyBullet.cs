using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : Bullet {

	// Use this for initialization
	void Start () {
		bc = GetComponent<Collider2D> ();
		ps = GetComponent<ParticleSystem> ();
		if(ps == null)
			ps = transform.GetComponentInChildren<ParticleSystem> ();
	}
	
	// Update is called once per frame
	void Update () {
		RaycastHorizontal ();
		RaycastVertical ();
		Move ();	
	}

	public void DestroyBullet(){
		ps.Stop ();
		//DestroyObject (this.gameObject);
	}

	protected override void ProcessCollide(GameObject obj){
		if(obj.layer == LayerMask.NameToLayer("Player")){
			//Add Damage to Player
			obj.transform.GetComponent<Player> ().Damaged (damage);
		}
		DestroyBullet ();
	}
}
