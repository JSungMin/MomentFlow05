using UnityEngine;
using UnityEngine.UI;

public class Notifications : MonoBehaviour
{
	[SerializeField] private Transform target;
	[SerializeField] private Vector2 offset;
	[SerializeField] private string[] dialoques;
	private Animator animator;
	private Text text;

	public void Start()
	{

		animator = GetComponent<Animator>();
		text = gameObject.transform.GetChild(0).GetComponent<Text>();

        if (target == null)
        {
            animator.SetBool("Hide", true);
        }
    }

	/// <summary>
	/// Aligns object in screen space based on the position from target that is in world space.
	/// </summary>
	[ContextMenu("UpdatePosition")]
	void LateUpdate()
	{
        if (target == null) return;

		Vector3 worldPos = new Vector3(target.transform.position.x + offset.x, target.transform.position.y + offset.y, target.transform.position.z);
		Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
		transform.position = new Vector3(screenPos.x, screenPos.y, screenPos.z);
	}

	/// <summary>
	/// Set's the target that object will align to.
	/// </summary>
	/// <param name="TargetObject">target position</param>
	public void SetTarget(GameObject TargetObject)
	{
		target = TargetObject.transform;
	}

	/// <summary>
	/// Moves object to new place and changes text in the child object.
	/// </summary>
	/// <param name="target">target position.</param>
	/// <param name="buildingType">Type of the building. Based on that different text is shown.</param>
	public void AdvanceDialoque(GameObject target,int buildingType)
	{
		SetTarget(target);
		text.text = dialoques[buildingType];
        animator.SetTrigger("Advance");
    }

}
