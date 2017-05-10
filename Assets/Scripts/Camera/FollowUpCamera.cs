﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowUpCamera : MonoBehaviour
{

    public Transform followTarget;
    public TweenAlpha fadeOut;
    public CameraFilterPack_TV_Vintage elasticFilter;

    public LayerMask mask;
    private Camera mainCamera;

    public float followSpeed = 2;
    //if -1 => don't limit distance
    public float maxDis = -1;

    public Vector3 paddingVector;

    public bool isActive = true;
    public bool isZoomIn = false;

    public AnimationCurve zoomInCurve;

    private float initOrthosize;

    Vector2 input;

    private void Awake()
    {
    }

    void Start()
    {
        mainCamera = GetComponent<Camera>();
        initOrthosize = mainCamera.orthographicSize;
    }

    bool isToLeft = false;
    bool isToRight = false;

    public void RaycastHorizontal()
    {
        var max = followTarget.GetComponent<BoxCollider>().bounds.max;
        var min = followTarget.GetComponent<BoxCollider>().bounds.min;

        var len = max.y - min.y;

        for (int i = 0; i < 3; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(max.x, min.y + len * ((float)i / (float)2)), Vector2.right, 1.6f, mask);
            Debug.DrawLine(new Vector2(max.x, min.y + len * ((float)i / (float)2)), new Vector2(max.x, min.y + len * ((float)i / (float)2)) + Vector2.right * 2);
            if (hit.collider != null)
            {

                isToRight = true;
            }
            else
            {
                isToRight = false;
            }
        }
        for (int i = 0; i < 3; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(min.x, min.y + len * ((float)i / (float)2)), Vector2.left, 1.6f, mask);
            Debug.DrawLine(new Vector2(min.x, min.y + len * ((float)i / (float)2)), new Vector2(max.x, min.y + len * ((float)i / (float)2)) + Vector2.left * 2);
            if (hit.collider != null)
            {
                isToLeft = true;
            }
            else
            {
                isToLeft = false;
            }
        }
    }

    void ProcessZoomIn()
    {
        if (isZoomIn)
        {
            if (tape <= 1)
            {
                mainCamera.orthographicSize = (initOrthosize) * (1 - zoomInCurve.Evaluate(tape) * 0.35f);
                tape += Time.deltaTime;
            }
            else
            {
                tape = 0;
                mainCamera.orthographicSize = initOrthosize;
                isZoomIn = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (null != followTarget)
        {
            RaycastHorizontal();
            var targetPos = followTarget.position;
            targetPos.z = 0;
            var nowPos = mainCamera.transform.position;
            nowPos.z = 0;
            var tmpVector = paddingVector;
            tmpVector.x *= Input.GetAxis("Horizontal");
            var pos = Vector3.Lerp(nowPos, targetPos + tmpVector, followSpeed * Time.deltaTime);
            pos.z = -30;
            mainCamera.transform.position = pos;

            ProcessZoomIn();
        }
    }
    private float tape = 0;
    public void StartInstantZoomIn()
    {
        isZoomIn = true;
        tape = 0;
    }

    public void SetFollowTargetToNull()
    {
        followTarget = null;
    }

    public void SetFollowTarget(Transform target)
    {
        if (null != target)
            followTarget = target;
        else
            SetFollowTargetToNull();
    }

    public void DoElasticEffect()
    {
        StartCoroutine(DoElasticEffectCo());
    }

    private IEnumerator DoElasticEffectCo()
    {
        for (int i = 0; i < 3; i++)
        {
            yield return StartCoroutine(IncreaseElasticEffectDistortion(1.0f));
            
            yield return StartCoroutine(DecreaseElasticEffectDistortion(1.0f));
        }
    }

    private const float deltaTm = 0.02f;
    private IEnumerator IncreaseElasticEffectDistortion(float excutionTm)
    {
        elasticFilter.Distortion = 0.0f;
        float iterTm = excutionTm / deltaTm;
        for (int j = 0; j < (int)iterTm; j++)
        {
            elasticFilter.Distortion += 10 / iterTm;
            yield return new WaitForSeconds(deltaTm);
        }
    }

    private IEnumerator DecreaseElasticEffectDistortion(float excutionTm)
    {
        elasticFilter.Distortion = 10.0f;
        float iterTm = excutionTm / deltaTm;
        for (int j = 0; j < (int)iterTm; j++)
        {
            elasticFilter.Distortion -= 10 / iterTm;
            yield return new WaitForSeconds(deltaTm);
        }
    }
}