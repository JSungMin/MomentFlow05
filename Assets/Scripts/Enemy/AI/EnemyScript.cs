using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Spine.Unity;
using Spine;

public class EnemyScript : MonoBehaviour {

	public Player playerObject { protected set; get; }

	public State enemyState;
    public IState[] istate;

	public AnimationBase anim;
	public string charAnimName;
	public GameObject aim_bone;
	public GameObject bullet;

	public bool canAlert;

	public Vector3 patrolDir;
	public float moveSpeed;

    public float holdDuration;
	public float walkDuration;

	public float detectionDuration;
	public float detectionDurationTimer = 0;

	public bool isAttack = false;
	public float attackRange;
	public float attackDelay;
	public Vector3 velocity;

	public float findOutSight = 1;
	public float findOutGauge = 0;
	public float findOutGaugeIncrement;
	public int browseDensity = 3;
	public LayerMask browseMask;

	public float transitionDuration;
	public float transitionDurationTimer = 0;

	protected void InitEnemy(){
		playerObject = GameObject.FindObjectOfType<Player> ();
		maxX = GetComponent<Collider2D> ().bounds.max.x;
		maxY = GetComponent<Collider2D> ().bounds.max.y;
		minX = GetComponent<Collider2D> ().bounds.min.x;
		minY = GetComponent<Collider2D> ().bounds.min.y;
		colXLen = GetComponent<Collider2D> ().bounds.size.x;
		colYLen = GetComponent<Collider2D> ().bounds.size.y;
	}

	public void Awake(){
		Debug.Log ("In Parent");
		InitEnemy ();
	}

	//this function will return IState
	public IState GetState(State enemyState)
    {
        return istate[(int)enemyState];
    }
	//this function will return derived class from IState
	public T GetSpecifiedState<T>(State enemyState) where T:IState{
		return ((T)GetState (enemyState));
	}
	//when Use below function then change state (also use OnStateEnter) or stay state (also use OnStateStay)
	public void SetState(State newState){
		enemyState = GetState (newState).ChangeState (enemyState);
	}

	float maxX,maxY;
	float minX,minY;
	float colXLen;
	float colYLen;

	public void AimToObject(Vector3 targetPos){
		if(aim_bone!=null){
			float dis_x;
			float dis_y;
			float degree;
			aim_bone.GetComponent<SkeletonUtilityBone> ().mode = SkeletonUtilityBone.Mode.Override;
			if (targetPos.x > transform.position.x) {
				dis_x = aim_bone.transform.position.x - targetPos.x;
				dis_y = targetPos.y - aim_bone.transform.position.y;
				degree = Mathf.Atan2 (dis_x, dis_y) * Mathf.Rad2Deg;
			} else {
				dis_x = -(aim_bone.transform.position.x - targetPos.x);
				dis_y = targetPos.y - aim_bone.transform.position.y;
				degree = Mathf.Atan2 (dis_x, dis_y) * Mathf.Rad2Deg;
			}
			aim_bone.transform.localRotation = Quaternion.Euler (0, 0, degree + 90);
		}
		return;
	}

	public void AimToForward(){
		if (aim_bone != null) {
			GetComponentInChildren<SkeletonAnimation> ().skeleton.SetBonesToSetupPose ();
			aim_bone.GetComponent<SkeletonUtilityBone> ().mode = SkeletonUtilityBone.Mode.Follow;
		}
	}

	public struct BrowseInfo
	{
		public GameObject hittedObj;
		public float distance;
		public Vector2 normal;
		public Vector2 point;
		public string tag;
		public int layer;

		public void InitBrowseInfo(GameObject obj,float d, Vector2 n, Vector2 p, string t,int l){
			hittedObj = obj;
			distance = d;
			normal = n;
			point = p;
			tag = t;
			layer = l;
		}
	}

	//만약 플레이어를 탐지시 return true else then return false
	public BrowseInfo Browse(float rayLen){
		RaycastHit2D[] browseHits = new RaycastHit2D[browseDensity];
		maxX = GetComponent<Collider2D> ().bounds.max.x;
		maxY = GetComponent<Collider2D> ().bounds.max.y;
		minX = GetComponent<Collider2D> ().bounds.min.x;
		minY = GetComponent<Collider2D> ().bounds.min.y;
		for(int i =0;i<browseDensity;i++){
			if (transform.localScale.x > 0) {
				browseHits [i] = Physics2D.Raycast (
					new Vector2 (maxX, minY + colYLen * (i / (browseDensity - 1))),
					Vector2.right,rayLen,browseMask);
			}
			else{
				browseHits [i] = Physics2D.Raycast (
					new Vector2 (minX, minY + colYLen * (i / (browseDensity - 1))),
					Vector2.left,rayLen,browseMask);
			}

			if (browseHits [i].collider != null) {
				BrowseInfo bInfo = new BrowseInfo ();
				bInfo.InitBrowseInfo (browseHits[i].collider.gameObject,browseHits [i].distance, browseHits [i].normal, browseHits[i].point,browseHits [i].transform.tag, browseHits [i].transform.gameObject.layer);
				return bInfo;
			} else {
				return new BrowseInfo();
			}
		}
		return new BrowseInfo();
	}
	public IEnumerator fireBullets;
	public void Fire(){
		var newBullet = MonoBehaviour.Instantiate (bullet, aim_bone.transform.position, Quaternion.identity) as GameObject;
		newBullet.GetComponent<NormalBullet> ().dir = (playerObject.transform.position - transform.position).normalized;
		//newBullet.transform.parent = enemyBulletPool;
	}

	public IEnumerator FireBullets(int num){
		int n = 0;
		isAttack = true;
		while (n < num) {
			Fire ();
			n++;
			yield return new WaitForSeconds (0.25f);
		}
		isAttack = false;
	}
}
