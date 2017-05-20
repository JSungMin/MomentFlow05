using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Spine.Unity.Modules;
using Spine.Unity;
using Spine;

public class EnemyScript : MonoBehaviour
{
    public Player playerObject { protected set; get; }
    public TimeLayer pTimeLayer { protected set; get; }

    public State enemyState;
    //유지될 상태
    public State defaultState;

    public IState[] istate;
    public List<int> canUseStateList = new List<int>();

    public AnimationBase anim;
    public GameObject aim_bone;
    public GameObject bullet;

    public GameObject emotionBox;

    public Transform findOutGaugePool;
    public GameObject findOutGaugePref;
    private FindOutGaugeScript findOutGaugeScr;

    const float skinWidth = .015f;

    public float maxHp;
    public float hp;

    public Vector3 patrolDir;
    public float moveSpeed;

    public float holdDuration;
    public float walkDuration;

    public float patrolDuration;
    [HideInInspector]
    public float patrolDurationTimer = 0;

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
    public int browseDensity = 4;
    public LayerMask browseMask;

    //public Dictionary<int, State> levelValue = new Dictionary<int, State>();
    public State[] levelValue = new State[8];

    public float transitionDuration;
    protected float transitionDurationTimer = 0;

    public void Awake()
    {
        Debug.Log("In Parent");
        if (findOutGaugeScr == null)
        {
            GameObject newGauge = Instantiate(findOutGaugePref, Vector3.zero, Quaternion.identity);
            newGauge.transform.parent = findOutGaugePool;
            newGauge.transform.localPosition = new Vector3(0, 0, -10000);
            newGauge.transform.localScale = Vector3.one;
            findOutGaugeScr = newGauge.GetComponent<FindOutGaugeScript>();
        }

        findOutGaugeScr.InitFindOutGaugeScript(this);
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        skeletonGost = GetComponentInChildren<SkeletonGhost>();
        boxCollider = GetComponent<BoxCollider>();
    }

    protected void InitEnemy()
    {
        playerObject = GameObject.FindObjectOfType<Player>();
        maxX = GetComponent<Collider>().bounds.max.x;
        maxY = GetComponent<Collider>().bounds.max.y;
        minX = GetComponent<Collider>().bounds.min.x;
        minY = GetComponent<Collider>().bounds.min.y;
        colXLen = GetComponent<Collider>().bounds.size.x;
        colYLen = GetComponent<Collider>().bounds.size.y;
        
        GetSpecifiedState<PatrolState>(State.Patrol).InitPatrolInfo(patrolDir, moveSpeed);
    }

    //this function will return IState
    public IState GetState(State enemyState)
    {
        return istate[(int)enemyState];
    }

    //this function will return derived class from IState
    public T GetSpecifiedState<T>(State enemyState) where T : IState
    {
        return ((T)GetState(enemyState));
    }
    public T GetSpecifiedState<T>() where T : IState
    {
        return ((T)GetState(enemyState));
    }

    public int GetStateLayerKey(State s)
    {
        for (int i = 0; i < levelValue.Length; i++)
        {
            if (levelValue[i] == s)
            {
                return i;
            }
        }
        return -1;
    }

    //when Use below function then change state (also use OnStateEnter) or stay state (also use OnStateStay)
    public void SetState(State newState)
    {
        enemyState = GetState(newState).ChangeState(enemyState);
    }

    public void AddStateToListWithCheckingOverlap(int layerLevel)
    {
        if (!canUseStateList.Contains(layerLevel))
        {
            canUseStateList.Add(layerLevel);
        }
    }

    public void DeleteStateToList(int layerLevel)
    {
        if (canUseStateList.Contains(layerLevel))
        {
            canUseStateList.Remove(layerLevel);
        }
    }

    float maxX, maxY;
    float minX, minY;
    float colXLen;
    float colYLen;

    public void SetDefaultState()
    {
        switch (defaultState)
        {
            case State.Idle:
                SetState(State.Idle);
                break;
            case State.Sit:
                SetState(State.Sit);
                break;
            case State.Patrol:
                SetState(State.Patrol);
                break;
            case State.Suspicious:
                GetSpecifiedState<SuspiciousState>(State.Suspicious).InitSuspiciousInfo(playerObject.transform.position, moveSpeed * 0.5f);
                SetState(State.Suspicious);
                break;
        }
    }

    public void PlayEmotion(string animName)
    {
        //TODO: 이후 추가해야 한다.
        //if(!emotionBox.activeSelf){
        //	emotionBox.SetActive (true);
        //	emotionBox.GetComponentInChildren<AnimationBase> ().setAnimation (0, animName, false, 1);
        //}
    }

    public void StopEmotion()
    {
        if (emotionBox.activeSelf)
        {
            //TODO: PlayEmotion 추가 후 아래 코드 추가
            //emotionBox.GetComponentInChildren<AnimationBase> ().setAnimation (0, "None", false, 1);
            emotionBox.SetActive(false);
        }
    }

    public void AimToObject(Vector3 targetPos)
    {
        if (aim_bone != null)
        {
            float dis_x;
            float dis_y;
            float degree;
            aim_bone.GetComponent<SkeletonUtilityBone>().mode = SkeletonUtilityBone.Mode.Override;
            if (targetPos.x > transform.position.x)
            {
                dis_x = aim_bone.transform.position.x - targetPos.x;
                dis_y = targetPos.y - aim_bone.transform.position.y;
                degree = Mathf.Atan2(dis_x, dis_y) * Mathf.Rad2Deg;
            }
            else
            {
                dis_x = -(aim_bone.transform.position.x - targetPos.x);
                dis_y = targetPos.y - aim_bone.transform.position.y;
                degree = Mathf.Atan2(dis_x, dis_y) * Mathf.Rad2Deg;
            }
            aim_bone.transform.localRotation = Quaternion.Euler(0, 0, degree + 90);
        }
        return;
    }

    public void AimToForward()
    {
        if (aim_bone != null)
        {
            anim.skel.skeleton.SetBonesToSetupPose();
            aim_bone.GetComponent<SkeletonUtilityBone>().mode = SkeletonUtilityBone.Mode.Follow;
        }
    }

    public struct BrowseInfo
    {
        public GameObject hittedObj;
        public float distance;
        public Vector3 normal;
        public Vector3 point;
        public string tag;
        public int layer;

        public void InitBrowseInfo(GameObject obj, float d, Vector3 n, Vector3 p, string t, int l)
        {
            hittedObj = obj;
            distance = d;
            normal = n;
            point = p;
            tag = t;
            layer = l;
        }
    }

    public BrowseInfo FindPlayerInBrowseInfos(float rayLen)
    {
        var infos = Browse(rayLen);
        for (int i = 0; i < infos.Length; i++)
        {
            if (infos[i].layer == LayerMask.NameToLayer("Player"))
            {
                return infos[i];
            }
        }
        return new BrowseInfo();
    }

    public BrowseInfo[] FindCollisionsInBrowseInfos(float rayLen)
    {
        var infos = Browse(rayLen);
        List<BrowseInfo> collisions = new List<BrowseInfo>();
        for (int i = 0; i < infos.Length; i++)
        {
            if (infos[i].layer == LayerMask.NameToLayer("Collision"))
            {
                collisions.Add(infos[i]);
            }
        }
        return collisions.ToArray();
    }

    //infos에 가까운 것 부터 담기는지 확인 필요
    public BrowseInfo FindNearestCollisionInBrowseInfos(float rayLen)
    {
        var infos = Browse(rayLen);
        for (int i = 0; i < infos.Length; i++)
        {
            if (infos[i].layer == LayerMask.NameToLayer("Collision"))
            {
                return infos[i];
            }
        }
        return new BrowseInfo();
    }

    public bool IsFindPlayer(float rayLen)
    {
        Browse(rayLen);
        for (int i = 0; i < browseDensity; i++)
        {
            for (int j = 0; j < gBrowseHits[i].Length; j++)
            {
                if (TimeLayer.EqualTimeLayer(pTimeLayer, gBrowseHits[i][j].collider.GetComponentInParent<TimeLayer>()))
                {
                    if (gBrowseHits[i][j].collider.gameObject.layer == LayerMask.NameToLayer("Collision"))
                    {
                        break;
                    }
                    else if (gBrowseHits[i][j].collider.gameObject.layer == LayerMask.NameToLayer("Player"))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

	public void AlertToNearEnemy(float alertRadius, LayerMask alertMask)
	{
		Debug.Log ("Alert");
		var colliders = Physics.OverlapSphere(transform.position, alertRadius, alertMask);

		for (int i = 0; i < colliders.Length; i++)
		{
			var escr = colliders[i].GetComponent<EnemyScript>();

			RaycastHit hit;
			Physics.Raycast(transform.position, (escr.transform.position - transform.position).normalized, out hit, alertRadius, alertMask);

			if (null != escr && escr != this)
			{
				if (TimeLayer.EqualTimeLayer(escr.pTimeLayer, pTimeLayer))
				{
					if (!escr.GetSpecifiedState<DetectionState>(State.Detection).isDetection)
					{
							var targetPos = transform.position;
							targetPos.y = escr.transform.position.y;
							escr.GetSpecifiedState<SuspiciousState>(State.Suspicious).InitSuspiciousInfo(
								Vector3.Lerp(escr.transform.position,targetPos,0.9f)
								, escr.moveSpeed * 1.0f);
							escr.AddStateToListWithCheckingOverlap(escr.GetStateLayerKey(State.Suspicious));
					}
				}
			}
		}
	}

    RaycastHit[][] gBrowseHits;
    //만약 탐지 가능한 객체(EqualTimeLayer == true)가 있으면 !null 요소가 만약 객체가 없으면 null
    public BrowseInfo[] Browse(float rayLen)
    {
        gBrowseHits = new RaycastHit[browseDensity][];
        BrowseInfo[] bInfos = new BrowseInfo[0];

        maxX = GetComponent<Collider>().bounds.max.x;
        maxY = GetComponent<Collider>().bounds.max.y;
        minX = GetComponent<Collider>().bounds.min.x;
        minY = GetComponent<Collider>().bounds.min.y;

        for (int i = 0; i < browseDensity; i++)
        {
            if (transform.localScale.x > 0)
            {
                gBrowseHits[i] = Physics.RaycastAll(
					new Vector3(minX - 0.1f, maxY - colYLen * (i / (browseDensity - 1)), transform.position.z),
					Vector3.right, rayLen + colXLen, browseMask);
            }
            else
            {
                gBrowseHits[i] = Physics.RaycastAll(
					new Vector3(maxX + 0.1f, maxY - colYLen * (i / (browseDensity - 1)), transform.position.z),
					Vector3.left, rayLen + colXLen, browseMask);
            }

            bInfos = new BrowseInfo[gBrowseHits[i].Length];

            for (int j = 0; j < gBrowseHits[i].Length; j++)
            {
                if (gBrowseHits[i][j].collider != null)
                {
                    if (TimeLayer.EqualTimeLayer(pTimeLayer, gBrowseHits[i][j].transform.GetComponentInParent<TimeLayer>()))
                    {
                        bInfos[j].InitBrowseInfo(gBrowseHits[i][j].collider.gameObject, gBrowseHits[i][j].distance, gBrowseHits[i][j].normal, gBrowseHits[i][j].point, gBrowseHits[i][j].transform.tag, gBrowseHits[i][j].transform.gameObject.layer);
                    }
                }
            }
        }
        return bInfos;
    }

    protected void VerticalCollisions()
    {
        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y * Time.deltaTime);

        RaycastHit[][] browseHits = new RaycastHit[browseDensity][];

        maxX = GetComponent<Collider>().bounds.max.x;
        maxY = GetComponent<Collider>().bounds.max.y;
        minX = GetComponent<Collider>().bounds.min.x;
        minY = GetComponent<Collider>().bounds.min.y;

        for (int i = 0; i < browseDensity; i++)
        {
            var rayOrigin = new Vector3(minX + colXLen * (i / (browseDensity - 1)), ((velocity.y >= 0) ? maxY : minY), transform.position.z);
            Debug.DrawLine(rayOrigin, rayOrigin + Vector3.up * rayLength * directionY, Color.red);
            browseHits[i] = Physics.RaycastAll(rayOrigin, Vector3.up * directionY, rayLength, browseMask);

            for (int j = 0; j < browseHits[i].Length; j++)
            {
                // Debug.Log(browseHits[i][j].collider.name);
                if (browseHits[i][j].collider != null)
                {
                    if (TimeLayer.EqualTimeLayer(pTimeLayer, browseHits[i][j].transform.GetComponentInParent<TimeLayer>()) ||
                        browseHits[i][j].transform.CompareTag("Ground") ||
                        browseHits[i][j].transform.CompareTag("GrabableGround"))
                    {
                        velocity.y = 0;
                    }
                }
            }
        }
    }

    public IEnumerator fireBullets;
    public void Fire()
    {
        var newBullet = Instantiate(bullet, Vector3.zero, Quaternion.identity) as GameObject;
        newBullet.transform.position = aim_bone.transform.position;
        newBullet.GetComponent<Bullet>().pTimeLayer = pTimeLayer;
        newBullet.GetComponent<Bullet>().dir = (playerObject.GetComponent<Collider>().bounds.center - GetComponent<Collider>().bounds.center).normalized;
    }

    public IEnumerator FireBullets(int num)
    {
        int n = 0;
        isAttack = true;
        while (n < num)
        {
            Fire();
            n++;
            yield return new WaitForSeconds(0.25f);
        }
        isAttack = false;
    }

    public bool CheckEscapeConditioin()
    {
        if (canEscape && hp <= escapeHP)
        {
            return true;
        }
        return false;
    }
    
    private MeshRenderer meshRenderer;
    private SkeletonGhost skeletonGost;
    private BoxCollider boxCollider;

    public void SetMaterialAlpha(float alpha)
    {
        meshRenderer.material.color =
            new Color(meshRenderer.material.color.r,
            meshRenderer.material.color.g,
            meshRenderer.material.color.b,
            alpha);
    }

    public void DisableGhosting()
    {
        skeletonGost.ghostingEnabled = false;
    }
}
