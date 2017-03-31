using UnityEngine;
/// <summary>
/// Camera functionality
/// </summary>
public class CameraOrbit : MonoBehaviour
{
	[SerializeField] private AnimationCurve rotationSmoothing;
	[SerializeField] private AnimationCurve fadeDeltaRotation;
	[SerializeField] private Vector3 mapOrigin;
	[SerializeField] private float speed = 0.04f;
	private float frontalRotation = 22.5f;
	private float topRotation = 90.0f;
	private Transform t;
	private bool easeOut = false;
	private float endDelta;
	private float fadeTimer = 1.0f;
	private float ScaledDeltaSpeedPrevX;
	
	//Mouse
	private Vector3 prevMousePos = new Vector3(0,0,0);
	private Vector3 DeltaMouse;

	[SerializeField] private bool Touch = true;
	[SerializeField] private bool Mouse = false;
	[SerializeField] private bool Keyboard = false;
	[SerializeField] private bool Gamepad = false;
	[SerializeField] private bool Showcase = true;
	private Vector2 ScaledDeltaSpeed;

	void Start()
	{
		t = transform;
	}

	/// <summary>
	/// Takes care of the Orbit function when user interacts wit keyboard, mouse or touch.
	/// </summary>
	void LateUpdate()
	{
		//for demonstration, constant rotation
		if (Showcase)
		{
			RotateAround(Vector3.up, 0.5f);
		}
		if (Mouse)
		{
			if (Input.GetMouseButton(0))
			{
				easeOut = false;
				fadeTimer = 1.0f;
				DeltaMouse = prevMousePos - Input.mousePosition;
				Vector3 DeltaMouseSpeed = DeltaMouse/Time.deltaTime;
				DeltaMouseSpeed = new Vector2(DeltaMouseSpeed.x*-1, DeltaMouseSpeed.y);
				ScaledDeltaSpeed = DeltaMouseSpeed*(speed*0.3f);
			}
			
			if (Input.GetMouseButtonUp(0) && (ScaledDeltaSpeed.x > 2f || ScaledDeltaSpeed.x < -2f))
			{
				easeOut = true;
				endDelta = Mathf.Clamp(ScaledDeltaSpeed.x / 2, -11, 11); //Clamp maximum speed of rotation
			}

			prevMousePos = Input.mousePosition;
		}

		if (Touch)
		{
			if (Input.touchCount == 1)
			{
				if (easeOut) ScaledDeltaSpeedPrevX = 0;
				easeOut = false;
				fadeTimer = 1.0f;
				Touch OneFinger = Input.GetTouch(0);
				Vector2 DeltaInputSpeed = OneFinger.deltaPosition/OneFinger.deltaTime;

				DeltaInputSpeed = new Vector2(DeltaInputSpeed.x, DeltaInputSpeed.y*-1);
				ScaledDeltaSpeed = DeltaInputSpeed* ( speed * 0.3f );

				if (OneFinger.phase == TouchPhase.Moved)
				{
					ScaledDeltaSpeedPrevX = ScaledDeltaSpeed.x;
				}
			}
		}

		if (Input.touchCount == 0 && (ScaledDeltaSpeedPrevX > 2f || ScaledDeltaSpeedPrevX < -2f))
		{
			endDelta = Mathf.Clamp(ScaledDeltaSpeedPrevX, -15, 15); //Clamp maximum speed of rotation  + ScaledDeltaSpeed.x)/2
			easeOut = true;
		}

		if (Input.touchCount == 1 || Input.GetMouseButton(0))
		{
			if (float.IsNaN(ScaledDeltaSpeed.x) || float.IsNaN(ScaledDeltaSpeed.y) || float.IsInfinity(ScaledDeltaSpeed.x) || float.IsInfinity(ScaledDeltaSpeed.y)) ScaledDeltaSpeed = new Vector2(0,0);

			RotateAround(Vector3.up, ScaledDeltaSpeed.x);

			if (frontalRotation <= t.eulerAngles.x + ScaledDeltaSpeed.y && topRotation >= t.eulerAngles.x + ScaledDeltaSpeed.y)
			{
				RotateAround(t.right, ScaledDeltaSpeed.y);
			}
		}

		if (easeOut) { EaseOutRotation(endDelta); }

		if (Keyboard)
		{
			if (Input.GetKey(KeyCode.LeftArrow))
			{
				RotateAround(Vector3.up, 1);
			}
			else if (Input.GetKey(KeyCode.RightArrow))
			{
				RotateAround(Vector3.up, -1);
			}

			if (Input.GetKey(KeyCode.DownArrow))
			{
				if (t.eulerAngles.x >= (frontalRotation + speed)) RotateAround(t.right, -1);
			}
			else if (Input.GetKey(KeyCode.UpArrow))
			{
				if (t.eulerAngles.x < (topRotation - speed)) RotateAround(t.right, 1);
			}
		}

		if (Gamepad)
		{
			
		}
	}

	/// <summary>
	/// Modified RotateAround that uses curve as smoothing when near the borders of orbit manipulation.
	/// </summary>
	/// <param name="axis">Axis of the rotation,</param>
	/// <param name="direction">Direction of rotation.</param>
	void RotateAround(Vector3 axis, float direction)
	{
		float angle;
		if (axis == t.right)
		{
			float curvePos = (t.eulerAngles.x - frontalRotation) / (topRotation - frontalRotation);
			if (curvePos < 0.25f && direction > 0) curvePos = Mathf.Clamp01(curvePos + 0.5f);
			else if (curvePos > 0.75f && direction < 0) curvePos = Mathf.Clamp01(curvePos - 0.5f);
			angle = direction * rotationSmoothing.Evaluate(Mathf.Abs(curvePos)); //- up, + down, front 0, top 1
		}
		else angle = direction;
		t.RotateAround( mapOrigin, axis, angle);
	}

	/// <summary>
	/// Function that fades the rotation when draged. Fading depends on the curve.
	/// </summary>
	/// <param name="ScaledDeltaSpeedX">Delta speed to fade out.</param>
	void EaseOutRotation(float ScaledDeltaSpeedX)
	{
		RotateAround(Vector3.up, ScaledDeltaSpeedX * Mathf.Abs(fadeDeltaRotation.Evaluate(fadeTimer)));
		fadeTimer = fadeTimer - (Time.deltaTime * 0.5f);
		if (fadeTimer <= 0)
		{
			easeOut = false;
			fadeTimer = 1.0f;
			ScaledDeltaSpeedPrevX = 0;
		}
	}

	/// <summary>
	/// Turns off turntable rotation at the begining of the demo.
	/// </summary>
	public void ShowcaseOff()
	{
		Showcase = false;
	}
}
