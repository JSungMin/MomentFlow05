using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
	public Vector3 dir;
	public float speed;

	public int damage;
	public LayerMask collisionMask;
	//p = parent
	public TimeLayer pTimeLayer;
	public Collider2D bc;
	protected ParticleSystem ps;

	private float bcMaxHeight;
	private float bcMaxWidth;

	private RaycastHit2D[][] gHitInfo;
	private int rayDensity = 3;

	public bool CheckCollideable(RaycastHit2D tmpHit){
		if (pTimeLayer == null)
			return false;
		if((TimeLayer.EqualTimeLayer(pTimeLayer,tmpHit.transform.GetComponentInParent<TimeLayer>())||
			tmpHit.transform.CompareTag("Ground")||
			tmpHit.transform.CompareTag("GrabableGround")) && 
			tmpHit.transform != transform &&
			!tmpHit.collider.isTrigger){
			return true;
		}
		return false;
	}

	private void InitBoxColliderInfo(){
		bcMaxWidth = bc.bounds.size.x;
		bcMaxHeight = bc.bounds.size.y;
	}

	protected void RaycastHorizontal(){
		InitBoxColliderInfo ();
		gHitInfo = new RaycastHit2D[rayDensity][];

		for(int i = 0;i<rayDensity;i++){
			if (dir.x >= 0) {
				gHitInfo [i] = Physics2D.RaycastAll (Vector3.right*bc.bounds.max.x + Vector3.up*bc.bounds.max.y * (i / (rayDensity - 1)),
					Vector3.right*dir.x, Mathf.Abs(dir.x*speed * Time.deltaTime),
					collisionMask);
			} else {
				gHitInfo [i] = Physics2D.RaycastAll (Vector3.right*bc.bounds.min.x + Vector3.up*bc.bounds.max.y * (i / (rayDensity - 1)),
					Vector3.right*dir.x, Mathf.Abs(dir.x*speed * Time.deltaTime),
					collisionMask);
			}
			for(int j = 0; j<gHitInfo[i].Length;j++){
				var tmpHit = gHitInfo [i] [j];
				if(CheckCollideable(tmpHit)){
					ProcessCollide (tmpHit.collider.gameObject);
				}
			}
		}
	}

	protected void RaycastVertical(){
		InitBoxColliderInfo ();
		gHitInfo = new RaycastHit2D[rayDensity][];

		for(int i = 0;i<rayDensity;i++){
			if (dir.y >= 0) {
				gHitInfo [i] = Physics2D.RaycastAll (Vector3.up*bc.bounds.max.y + Vector3.right*bc.bounds.max.x * (i / (rayDensity - 1)),
					Vector3.up*dir.y, Mathf.Abs(dir.y*speed * Time.deltaTime),
					collisionMask);
			} else {
				gHitInfo [i] = Physics2D.RaycastAll (Vector3.up*bc.bounds.min.y + Vector3.right*bc.bounds.max.x * (i / (rayDensity - 1)),
					Vector3.up*dir.y, Mathf.Abs(dir.y*speed * Time.deltaTime),
					collisionMask);
			}
			for(int j = 0; j<gHitInfo[i].Length;j++){
				var tmpHit = gHitInfo [i] [j];
				if(CheckCollideable(tmpHit)){
					ProcessCollide (tmpHit.collider.gameObject);
				}
			}
		}
	}

	protected virtual void ProcessCollide(GameObject obj){ }

	public void Move(){
		transform.Translate (dir * speed * Time.deltaTime);	
	}
}
