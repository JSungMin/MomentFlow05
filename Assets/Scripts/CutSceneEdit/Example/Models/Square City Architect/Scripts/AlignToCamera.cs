using UnityEngine;
/// <summary>
/// Use for UI elements in 3D space
/// </summary>
public class AlignToCamera : MonoBehaviour
{
	
	void LateUpdate () 
	{
		transform.LookAt(Camera.main.transform);
	}

	/// <summary>
	/// Aligns object to camera.
	/// </summary>
	public void OnClick()
	{
		//if building is higer that last level, turn of the UI element
		if (transform.position.y + 0.5f > GameSettings.Instance.GetMiddleLevel()+0.5f)
		{
			gameObject.SetActive(false);
		}
		else transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
	}
}
