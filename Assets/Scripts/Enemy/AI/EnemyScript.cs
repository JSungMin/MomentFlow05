using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Spine.Unity;
using Spine;

public class EnemyScript : MonoBehaviour {

	public Player playerObject { protected set; get; }

	public State enemyState;
	//유지될 상태
	public State defaultState;

	public IState[] istate;

	public AnimationBase anim;
	public string charAnimName;
	public GameObject aim_bone;
	public GameObject bullet;

	public GameObject emotionBox;

	public FindOutGaugeScript findOutGaugeScr;

	public float maxHp;
	public float hp;

	public Vector3 patrolDir;
	public float moveSpeed;

    public float holdDuration;
	public float walkDuration;

	public float patrolDuration;
	protected float patrolDurationTimer = 0;

	public float detectionDuration;
	protected float detectionDurationTimer = 0;

	public bool isAttack = false;
	public float attackRange;
	public float attackDelay;
	public Vector3 velocity;

	public bool canAlert;
	public bool canEscape;
	public float escapeHP;

	public float findOutSight = 1;
	public float findOutGauge = 0;
	public float findOutGaugeIncrement;
	public int browseDensity = 3;
	public LayerMask browseMask;

	public float transitionDuration;
	protected float transitionDurationTimer = 0;

	protected void InitEnemy(){
		playerObject = GameObject.FindObjectOfType<Player> ();
		maxX = GetComponent<Collider2D> ().bounds.max.x;
		maxY = GetComponent<Collider2D> ().bounds.max.y;
		minX = GetComponent<Collider2D> ().bounds.min.x;
		minY = GetComponent<Collider2D> ().bounds.min.y;
		colXLen = GetComponent<Collider2D> ().bounds.size.x;
		colYLen = GetComponent<Collider2D> ().bounds.size.y;

		InitToTransition ();
		GetSpecifiedState<PatrolState>(State.Patrol).InitPatrolInfo (patrolDir, moveSpeed);
	}

	public void Awake(){
		Debug.Log ("In Parent");
		if (findOutGaugeScr == null) {
			Debug.LogError ("There is No FindOutGagueScirpt for this Enemy : " + gameObject.name);
		} else {
			findOutGaugeScr.InitFindOutGaugeScript (this);
		}
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

	public void InitToTransition(){
		transitionDurationTimer = 0;
	}

	public void SetDefaultState(){
		switch(defaultState){
		case State.Idle:
			SetState (State.Idle);
			break;
		case State.Sit:
			SetState (State.Sit);
			break;
		case State.Patrol:
			//GetSpecifiedState<PatrolState> (State.Patrol).InitPatrolInfo(patrolDir,moveSpeed);
			SetState (State.Patrol);
			break;
		case State.Suspicious:
			GetSpecifiedState<SuspiciousState> (State.Suspicious).InitSuspiciousInfo(playerObject.transform.position,moveSpeed*0.5f);
			SetState (State.Suspicious);
			break;
		}
	}

	public void PlayEmotion(string animName){
		if(!emotionBox.activeSelf){
			emotionBox.SetActive (true);
			emotionBox.GetComponentInChildren<AnimationBase> ().setAnimation (0, animName, false, 1);
		}
	}

	public void StopEmotion(){
		if(emotionBox.activeSelf){
			//emotionBox.GetComponentInChildren<AnimationBase> ().setAnimation (0, "None", false, 1);
			emotionBox.SetActive (false);
		}
	}

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
			anim.skel.skeleton.SetBonesToSetupPose ();
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

			if (browseHits [i].collider != null && TimeLayer.EqualTimeLayer(gameObject,browseHits[i].collider.gameObject)) {
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

	public bool CheckEscapeConditioin(){
		if(canEscape&&hp<=escapeHP){
			return true;
		}
		return false;
	}
}
