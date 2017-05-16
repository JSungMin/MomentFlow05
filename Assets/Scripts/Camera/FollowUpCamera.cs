using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowUpCamera : MonoBehaviour
{
    public Transform followTarget;
    public TweenAlpha fadeOut;
    public TweenAlpha whiteOut;
    public CameraFilterPack_TV_Vintage elasticFilter;

    public LayerMask mask;
    private Camera mainCamera;

	public Vector3 centerPosition;

    public float followSpeed = 2;
    //if -1 => don't limit distance
    public float maxDis = -1;

    public Vector3 paddingVector;

    public bool isActive = true;
    public bool isZoomIn = false;

    public AnimationCurve zoomInCurve;

    private float initOrthosize;

	public bool isCamLock = true;

	Vector2 input;

	public Vector3 offset;

	private Vector3 vHitPos;
	private Vector3 hHitPos;

	public bool isToLeft = false;
	public bool isToRight = false;

	public bool isToTop = false;
	public bool isToBottom = false;

	float colWidth;
	float colHeight;


	public float horizontalRayLength, verticalRayLength; 

    void Start()
    {
        mainCamera = GetComponent<Camera>();
        initOrthosize = mainCamera.orthographicSize;
		horizontalRayLength = Mathf.Abs(Camera.main.ScreenToWorldPoint (new Vector3 (Screen.width, Screen.height, 0)).x) * 0.25f;
		verticalRayLength = Camera.main.ScreenToWorldPoint (new Vector3 (Screen.width, Screen.height, 0)).y * 0.25f;
    }

	public void SetHorizontalHitObjectInfo(RaycastHit hit){
		hHitPos = hit.point;
		colWidth = hit.collider.bounds.size.x;
	}
	public void SetVerticalHitObjectInfo(RaycastHit hit){
		vHitPos = hit.point;
		colHeight = hit.collider.bounds.size.y;
	}

    public void RaycastHorizontal()
    {
		var max = followTarget.GetComponent<BoxCollider> ().bounds.max;
		var min = followTarget.GetComponent<BoxCollider> ().bounds.min;

		var len = max.y - min.y;

		for (int i = 0; i < 3; i++) {
			RaycastHit[] hits = Physics.RaycastAll (new Vector3 (max.x, min.y + len * ((float)i / (float)2), followTarget.position.z), Vector3.right, horizontalRayLength, mask);
			Debug.DrawLine (new Vector3 (max.x, min.y + len * ((float)i / (float)2), followTarget.position.z), new Vector3 (max.x, min.y + len * ((float)i / (float)2), followTarget.position.z) + Vector3.right * horizontalRayLength);

			isToRight = false;
			for(int j = 0; j < hits.Length; j++){
				var hit = hits [j];
				if (hit.collider != null && hit.collider.CompareTag("Bound")) {
					SetHorizontalHitObjectInfo (hit);
					isToRight = true;
					break;
				}
			}
		}
		for (int i = 0; i < 3; i++) {
			RaycastHit[] hits = Physics.RaycastAll (new Vector3 (min.x, min.y + len * ((float)i / (float)2), followTarget.position.z), Vector3.left, horizontalRayLength,mask);
			Debug.DrawLine (new Vector3 (min.x, min.y + len * ((float)i / (float)2),followTarget.position.z), new Vector3 (max.x, min.y + len * ((float)i / (float)2), followTarget.position.z) + Vector3.left*horizontalRayLength);
			isToLeft = false;

			for(int j = 0; j < hits.Length; j++){
				var hit = hits [j];
				if (hit.collider != null && hit.collider.CompareTag("Bound")) {
					SetHorizontalHitObjectInfo (hit);
					isToLeft = true;
					break;
				}
			}
		}
    }

	public void RaycastVertical(){
		var max = followTarget.GetComponent<BoxCollider> ().bounds.max;
		var min = followTarget.GetComponent<BoxCollider> ().bounds.min;

		var len = max.x - min.x;

		for (int i = 0; i < 3; i++) {
			RaycastHit[] hits = Physics.RaycastAll (new Vector3 (min.x + len * ((float)i / (float)2), max.y, followTarget.position.z), Vector3.up, verticalRayLength,mask);
			Debug.DrawLine (new Vector3 (min.x+ len * ((float)i / (float)2), max.y), new Vector3 (min.x + len * ((float)i / (float)2), max.y) + Vector3.up*verticalRayLength);

			isToTop = false;
			for(int j = 0; j < hits.Length; j++){
				var hit = hits [j];
				if (hit.collider != null && hit.collider.CompareTag("Bound")) {
					SetVerticalHitObjectInfo (hit);
					isToTop = true;
					break;
				}
			}
		}
		for (int i = 0; i < 3; i++) {
			RaycastHit[] hits = Physics.RaycastAll (new Vector3 (min.x + len * ((float)i / (float)2), min.y,followTarget.position.z), Vector3.down, verticalRayLength,mask);
			Debug.DrawLine (new Vector3 (min.x+ len * ((float)i / (float)2), max.y), new Vector3 (min.x + len * ((float)i / (float)2), max.y) + Vector3.down*verticalRayLength);

			isToBottom = false;
			for(int j = 0; j < hits.Length; j++){
				var hit = hits [j];
				if (hit.collider != null && hit.collider.CompareTag("Bound")) {
					SetVerticalHitObjectInfo (hit);
					isToBottom = true;
					break;
				}
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
    
	public void InitCenterPosition(){
		centerPosition = Vector3.zero;
		centerPosition = followTarget.position;
		centerPosition.z = transform.position.z;
	}

	private void LockCamera(){
		if ((!isToLeft && !isToRight) &&
			(!isToTop && !isToBottom)) {
			transform.position = Vector3.Lerp (transform.position, centerPosition, Time.deltaTime * 4.0f);
		} else {
			Vector3 dir = Vector3.zero;
			if (isToLeft && isToRight) {
				hHitPos = centerPosition;
			} else {
				if (isToLeft) {
					dir.x = 1f;
				}
				else if (isToRight) {
					dir.x = -1f;
				}
				else if(!isToLeft && !isToRight){
					hHitPos = centerPosition;
				}
			}

			if (isToTop && isToBottom) {
				vHitPos = centerPosition;
			} else {
				if (isToTop) {
					dir.y = -1f;
				}
				else if (isToBottom) {
					dir.y = 1f;
				}
				else if(!isToTop && !isToBottom){
					vHitPos = centerPosition;
				}
			}

			var tmpOffset = offset;
			tmpOffset.x = hHitPos.x + dir.x * (horizontalRayLength - colWidth * 0.5f);
			tmpOffset.y = vHitPos.y + dir.y * (verticalRayLength - colHeight * 0.5f);
			tmpOffset.z = transform.position.z;
			transform.position = Vector3.Lerp (transform.position, tmpOffset, Time.deltaTime * 2.0f);
		}
	}

    void Update()
    {
        if (null != followTarget)
        {
			RaycastHorizontal ();
			RaycastVertical ();

			{//Must go in order -> 순서를 지켜야만 함
				if (isCamLock) {
					InitCenterPosition ();
					LockCamera ();
				} else {
					InitCenterPosition ();
					var targetPos = followTarget.position;
					targetPos.z = 0;
					var nowPos = centerPosition;
					nowPos.z = 0;
					var tmpVector = paddingVector;
					tmpVector.x *= Input.GetAxis("Horizontal");
					var pos = Vector3.Lerp(nowPos, targetPos + tmpVector, followSpeed * Time.deltaTime);
					pos.z = -30;
					mainCamera.transform.position = pos;
				}
			}

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
    
    public IEnumerator DoElasticEffect()
    {
        whiteOut.enabled = true;
        whiteOut.ResetToBeginning();

        yield return new WaitForSeconds(whiteOut.duration);

        whiteOut.enabled = false;
        whiteOut.ResetToBeginning();
    }
	//Be Careful this is 임시방편
	public void DoDistortionEffect()
	{
		StartCoroutine(DoDistortionEffectCo());
	}

	private IEnumerator DoDistortionEffectCo()
	{
		elasticFilter.enabled = true;
		yield return StartCoroutine(IncreaseDistortion(0.1f));
		yield return StartCoroutine(DecreaseDistortion(0.0f));
		elasticFilter.enabled = false;
	}

	private const float deltaTm = 0.02f;
	private const float DISTORTION_MAX = 5.0f;
	private IEnumerator IncreaseDistortion(float excutionTm)
	{
		float nowTm = 0.0f;
		elasticFilter.Distortion = 0.0f;
		while(nowTm < excutionTm)
		{
			elasticFilter.Distortion = (nowTm / excutionTm) * DISTORTION_MAX;
			nowTm += deltaTm;
			yield return new WaitForSeconds(deltaTm);
		}
	}

	private IEnumerator DecreaseDistortion(float excutionTm)
	{
		float nowTm = 0.0f;
		elasticFilter.Distortion = 5.0f;
		while (nowTm < excutionTm)
		{
			elasticFilter.Distortion = (1 - (nowTm / excutionTm)) * DISTORTION_MAX;
			nowTm += deltaTm;
			yield return new WaitForSeconds(deltaTm);
		}
	}

}