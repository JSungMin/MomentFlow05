using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Spine.Unity;
using Spine;

public class EnemyScript : MonoBehaviour {

	public Player playerObject { protected set; get; }
	public TimeLayer pTimeLayer{ protected set; get;}

	public State enemyState;
	//유지될 상태
	public State defaultState;

	public IState[] istate;

	public AnimationBase anim;
	public string charAnimName;
	public GameObject aim_bone;
	public GameObject bullet;

	public GameObject emotionBox;

	public Transform findOutGaugePool;
	public GameObject findOutGaugePref;
	private FindOutGaugeScript findOutGaugeScr;

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
			GameObject newGauge = Instantiate (findOutGaugePref, Vector3.zero,Quaternion.identity);
			newGauge.transform.parent = findOutGaugePool;
			newGauge.transform.localPosition = new Vector3 (0,0,-10000);
			newGauge.transform.localScale = Vector3.one;
			findOutGaugeScr = newGauge.GetComponent<FindOutGaugeScript> ();
		}
		findOutGaugeScr.InitFindOutGaugeScript (this);
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

	public BrowseInfo FindPlayerInBrowseInfos(float rayLen){
		var infos = Browse (rayLen);
		for(int i = 0; i < infos.Length;i++){
			if(infos[i].layer == LayerMask.NameToLayer("Player")){
				return infos [i];
			}
		}
		return new BrowseInfo();
	}

	public BrowseInfo[] FindCollisionsInBrowseInfos(float rayLen){
		var infos = Browse (rayLen);
		List<BrowseInfo> collisions = new List<BrowseInfo> ();
		for(int i = 0; i< infos.Length;i++){
			if(infos[i].layer == LayerMask.NameToLayer("Collision")){
				collisions.Add(infos [i]);
			}
		}
		return collisions.ToArray();
	}

	//infos에 가까운 것 부터 담기는지 확인 필요
	public BrowseInfo FindNearestCollisionInBrowseInfos(float rayLen){
		var infos = Browse (rayLen);
		for(int i = 0; i< infos.Length;i++){
			if(infos[i].layer == LayerMask.NameToLayer("Collision")){
				return infos [i];
			}
		}
		return new BrowseInfo ();
	}
		
	public bool IsFindPlayer(float rayLen){
		Browse (rayLen);
		for(int i = 0;i < browseDensity;i++){
			for(int j = 0;j < gBrowseHits[i].Length;j++){
				if(TimeLayer.EqualTimeLayer(pTimeLayer,gBrowseHits[i][j].collider.GetComponentInParent<TimeLayer>())){
					if (gBrowseHits [i] [j].collider.gameObject.layer == LayerMask.NameToLayer ("Collision")) {
						break;
					} else if(gBrowseHits [i] [j].collider.gameObject.layer == LayerMask.NameToLayer ("Player")){
						return true;
					}
				}
			}
		}
		return false;
	}

	RaycastHit2D[][] gBrowseHits;
	//만약 탐지 가능한 객체(EqualTimeLayer == true)가 있으면 !null 요소가 만약 객체가 없으면 null
	public BrowseInfo[] Browse(float rayLen){
		gBrowseHits = new RaycastHit2D[browseDensity][];
		BrowseInfo[] bInfos = new BrowseInfo[0];

		maxX = GetComponent<Collider2D> ().bounds.max.x;
		maxY = GetComponent<Collider2D> ().bounds.max.y;
		minX = GetComponent<Collider2D> ().bounds.min.x;
		minY = GetComponent<Collider2D> ().bounds.min.y;
		for(int i =0;i<browseDensity;i++){
			if (transform.localScale.x > 0) {
				gBrowseHits [i] = Physics2D.RaycastAll (
					new Vector2 (maxX, maxY - colYLen * (i / (browseDensity - 1))),
					Vector2.right,rayLen,browseMask);
			}
			else{
				gBrowseHits [i] = Physics2D.RaycastAll (
					new Vector2 (minX, maxY - colYLen * (i / (browseDensity - 1))),
					Vector2.left,rayLen,browseMask);
			}

			bInfos = new BrowseInfo[gBrowseHits [i].Length];

			for(int j = 0; j <gBrowseHits[i].Length; j++){
				if (gBrowseHits [i][j].collider != null) {
					if(TimeLayer.EqualTimeLayer(pTimeLayer,gBrowseHits[i][j].transform.GetComponentInParent<TimeLayer>())){
						bInfos[j].InitBrowseInfo (gBrowseHits[i][j].collider.gameObject,gBrowseHits [i][j].distance, gBrowseHits [i][j].normal, gBrowseHits[i][j].point,gBrowseHits [i][j].transform.tag, gBrowseHits [i][j].transform.gameObject.layer);
					}
				}
			}
		}
		return bInfos;
	}

	protected void VerticalCollisions(){

		RaycastHit2D[][] browseHits = new RaycastHit2D[browseDensity][];

		maxX = GetComponent<Collider2D> ().bounds.max.x;
		maxY = GetComponent<Collider2D> ().bounds.max.y;
		minX = GetComponent<Collider2D> ().bounds.min.x;
		minY = GetComponent<Collider2D> ().bounds.min.y;

		for(int i =0;i<browseDensity;i++){
			if (velocity.y > 0) {
				Debug.DrawLine (new Vector2 (minX + colXLen * (i / (browseDensity - 1)), maxY),
					new Vector2 (minX + colXLen * (i / (browseDensity - 1)), maxY) + Vector2.up*velocity.y * Time.deltaTime);
				browseHits [i] = Physics2D.RaycastAll (
					new Vector2 (minX + colXLen * (i / (browseDensity - 1)), maxY),
					Vector2.up, velocity.y * Time.deltaTime, browseMask);
			} else {
				Debug.DrawLine (new Vector2 (minX + colXLen * (i / (browseDensity - 1)), minY),
					new Vector2 (minX + colXLen * (i / (browseDensity - 1)), minY) + Vector2.up*velocity.y * Time.deltaTime);
				browseHits [i] = Physics2D.RaycastAll (
					new Vector2 (minX + colXLen * (i / (browseDensity - 1)), minY),
					Vector2.up, velocity.y * Time.deltaTime, browseMask);
			}

			for(int j = 0;j<browseHits[i].Length;j++){
				if(browseHits[i][j].collider != null){
					//if(browseHits[i][j].transform.gameObject.layer == LayerMask.NameToLayer("Collision")){
					if(TimeLayer.EqualTimeLayer(pTimeLayer,browseHits[i][j].transform.GetComponentInParent<TimeLayer>())||
						browseHits[i][j].transform.CompareTag("Ground")||
						browseHits[i][j].transform.CompareTag("GrabableGround")){
						velocity.y = 0;
					}
					//}
				}
			}
		}
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
