using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableObjectScript : InteractInterface
{
    public TimeLayer pTimeLayer { protected set; get; }
    private Transform initRootTrans;
    private Transform playerHands;

    private bool isGrabbed;
    private RaycastHit[][] hitsX;
    private RaycastHit[][] hitsY;

    public LayerMask mask;
    public LayerMask alertMask;
    public bool applyGravity = true;
    public float flingPower;
    public int rayDensity;
    public Vector2 velocity;

    public float alertRadius;

    public float threshold;

    public void CalculateThrowVelocity()
    {
        Vector3 mp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 dir = (mp - transform.position).normalized;
        velocity.x = dir.x * flingPower * 2;
        velocity.y = dir.y * flingPower;
    }

    //if this object collides with collision layer object 
    //below function will be used.
    public void AlertToNearEnemy()
    {
		var colliders = Physics.OverlapSphere(transform.position, alertRadius, alertMask);

        for (int i = 0; i < colliders.Length; i++)
        {
            var escr = colliders[i].GetComponent<EnemyScript>();

			RaycastHit hit;
			if (Physics.Raycast (transform.position, (escr.transform.position - transform.position).normalized,out hit, alertRadius, mask)) {

			}
            Debug.DrawLine(transform.position, transform.position + (escr.transform.position - transform.position).normalized * alertRadius, Color.red);
            if (null != escr)
            {
                if (TimeLayer.EqualTimeLayer(escr.pTimeLayer, pTimeLayer))
                {
                    if (null != hit.collider && hit.collider.gameObject.layer != LayerMask.NameToLayer("Collision"))
                    {
                        if (!escr.GetSpecifiedState<DetectionState>(State.Detection).isDetection)
                        {
                            escr.GetSpecifiedState<SuspiciousState>(State.Suspicious).InitSuspiciousInfo(transform.position, escr.moveSpeed * 0.5f);
                            escr.SetState(State.Suspicious);
                            escr.InitToTransition();
                            Debug.Log(escr.name + " alerted");
                        }
                    }
                }
            }
        }
    }

    //Interact Function
    public void GrabObject()
    {
        if (!isInteracted)
        {
            transform.parent = playerHands.transform;
            transform.localPosition = Vector3.zero;

            velocity = Vector3.zero;
            applyGravity = false;
            isGrabbed = true;
        }
    }

    //Stop Interact Function
    public void ReleaseObject()
    {
        isInteracted = false;
        GameObject.FindObjectOfType<InteractiveManager>().nowInteract = false;
        transform.parent = initRootTrans;

        applyGravity = true;
        isGrabbed = false;
    }

    //triggered when is grabbed and mouse left button click
    public void ThrowObject()
    {
        ReleaseObject();
        CalculateThrowVelocity();
    }

    public Collider FindOutNearestCollider(Collider[] cols)
    {
        float minDis = 0;
        int minIndex = 0;
        List<Collider> colList = new List<Collider>();

        for (int i = 0; i < cols.Length; i++)
        {
            if (TimeLayer.EqualTimeLayer(cols[i].transform.GetComponentInParent<TimeLayer>(), pTimeLayer) ||
                cols[i].transform.CompareTag("Ground") || cols[i].transform.CompareTag("GrabableGround"))
            {
                colList.Add(cols[i]);
            }
        }
        
        if (colList.Count == 0)
            return null;
        else if (colList.Count == 1)
            return colList[0];
        // 충돌체가 여러 개 라면
        else
        {
            minDis = Vector2.Distance(colList[0].transform.position, transform.position);
            minIndex = 0;
            
            for (int i = 1; i < colList.Count; i++)
            {
                if (Vector2.Distance(colList[i].transform.position, transform.position) <= minDis)
                {
                    minDis = Vector2.Distance(colList[i].transform.position, transform.position);
                    minIndex = i;
                }
            }
            return colList[minIndex];
        }
    }

    float maxX, maxY;
    float minX, minY;
    float colXLen;
    float colYLen;

    int findOutIndex = 0;
    public void RayCastByVelocity()
    {
        hitsX = new RaycastHit[rayDensity][];
        hitsY = new RaycastHit[rayDensity][];
        maxX = GetComponent<Collider>().bounds.max.x;
        maxY = GetComponent<Collider>().bounds.max.y;
        minX = GetComponent<Collider>().bounds.min.x;
        minY = GetComponent<Collider>().bounds.min.y;

        Vector2 xDir = Vector2.zero;
        Vector2 yDir = Vector2.zero;

        for (int i = 0; i < rayDensity; i++)
        {
            Collider[] tmpColsX = new Collider[0];
            Collider[] tmpColsY = new Collider[0];
            if (velocity.x > 0)
            {
                xDir = Vector2.right;
                hitsX[i] = Physics.RaycastAll(
                    new Vector2(maxX, minY + colYLen * (i / (rayDensity - 1))),
                    xDir, Mathf.Abs(velocity.x) * Time.deltaTime, mask);
                tmpColsX = new Collider[hitsX[i].Length];
                for (int j = 0; j < tmpColsX.Length; j++)
                {
                    tmpColsX[j] = hitsX[i][j].collider;
                }
            }
            else if (velocity.x < 0)
            {
                xDir = Vector2.left;
                hitsX[i] = Physics.RaycastAll(
                    new Vector2(minX, minY + colYLen * (i / (rayDensity - 1))),
                    xDir, Mathf.Abs(velocity.x) * Time.deltaTime, mask);
                tmpColsX = new Collider[hitsX[i].Length];
                for (int j = 0; j < tmpColsX.Length; j++)
                {
                    tmpColsX[j] = hitsX[i][j].collider;
                }
            }

            if (velocity.y > 0)
            {
                yDir = Vector2.up;
                hitsY[i] = Physics.RaycastAll(
                    new Vector2(minX + colXLen * (i / (rayDensity - 1)), maxY),
                    yDir, Mathf.Abs(velocity.y) * Time.deltaTime, mask);
                tmpColsY = new Collider[hitsY[i].Length];
                for (int j = 0; j < tmpColsY.Length; j++)
                {
                    tmpColsY[j] = hitsY[i][j].collider;
                }

            }
            else if (velocity.y < 0)
            {
                yDir = Vector2.down;
                hitsY[i] = Physics.RaycastAll(
                    new Vector2(minX + colXLen * (i / (rayDensity - 1)), minY),
                    yDir, Mathf.Abs(velocity.y) * Time.deltaTime, mask);
                tmpColsY = new Collider[hitsY[i].Length];
                for (int j = 0; j < tmpColsY.Length; j++)
                {
                    tmpColsY[j] = hitsY[i][j].collider;
                }
            }

            Vector2 reflectVec = Vector2.zero;

            var nearestX = FindOutNearestCollider(tmpColsX);
            if (nearestX != null)
            {
                if (Vector3.Magnitude(velocity) >= threshold)
                    AlertToNearEnemy();
                velocity += Vector2.Reflect(xDir * Mathf.Abs(velocity.x) * 1.2f, hitsX[i][findOutIndex].normal);
            }

            var nearestY = FindOutNearestCollider(tmpColsY);
            if (nearestY != null)
            {
                if (Vector3.Magnitude(velocity) >= threshold)
                    AlertToNearEnemy();
                velocity += Vector2.Reflect(yDir * Mathf.Abs(velocity.y), hitsY[i][findOutIndex].normal);
            }
        }
    }

    // Use this for initialization
    void Start()
    {
        pTimeLayer = transform.GetComponentInParent<TimeLayer>();
        initRootTrans = transform.parent;
        playerHands = GameObject.FindObjectOfType<Player>().transform.GetChild(1);
        interact += GrabObject;
        stopInteract += ReleaseObject;
    }

    void Update()
    {
        if (applyGravity)
        {
            velocity += Vector2.down * 9.8f * Time.deltaTime;
        }
        
        if (isGrabbed)
        {
            if (Input.GetMouseButtonDown(0))
            {
                ThrowObject();
            }
        }

        RayCastByVelocity();
        transform.Translate(velocity * Time.deltaTime);

        velocity.x = Mathf.Lerp(velocity.x, 0, Time.deltaTime * 3f);
    }
}
