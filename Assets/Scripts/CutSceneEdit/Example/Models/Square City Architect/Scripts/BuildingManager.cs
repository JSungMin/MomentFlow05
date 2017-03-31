using UnityEngine;
/// <summary>
/// Script responsible for building single object
/// </summary>
public class BuildingManager : MonoBehaviour
{

	[SerializeField] private GameObject[] bottom;
	[SerializeField] private GameObject[] middle;
	[SerializeField] private GameObject[] top;
	private Animator animator;
	private GameSettings.MapStatus status;

	void Awake()
	{
		GameSettings.OnUpdateStatus += OnUpdateStatus;
		animator = GetComponent<Animator>();
	}

	/// <summary>
	/// When object is enabled, function checks at what height it is. Then modifies the look of the building accordingly.
	/// </summary>
	void OnEnable()
	{
		if (transform.position.y < GameSettings.Instance.GetBottomLevel())
		{
			Debug.Log("bottom");
			for (int i = 0; i < bottom.Length; i++)
			{
				bottom[i].SetActive(true);
			}
		}
		else if (GameSettings.Instance.GetMiddleLevel() > transform.position.y && transform.position.y >= GameSettings.Instance.GetBottomLevel())
		{
			Debug.Log("middle");
			for (int i = 0; i < middle.Length; i++)
			{
				middle[i].SetActive(true);
			}
		}
		else if (GameSettings.Instance.GetMiddleLevel() <= transform.position.y)
		{
			Debug.Log("top");
			for (int i = 0; i < top.Length; i++)
			{
				top[i].SetActive(true);
			}
		}
		else
		{
			Debug.Log("Unknown Level");
		}
	}

	/// <summary>
	/// Function that checks what is the current status of buildings and sets trigger accordingly.
	/// </summary>
	/// <param name="status">Current status of the buildings in the scene.</param>
	void OnUpdateStatus(GameSettings.MapStatus status)
	{
		switch (status)
		{
			case GameSettings.MapStatus.AllowBuilding:
				//animator.SetTrigger("Show");
				break;
			case GameSettings.MapStatus.ExampleCity:
				animator.SetTrigger("ShowExampleCity");
				break;
			case GameSettings.MapStatus.HideCity:
				animator.SetTrigger("Hide");
				break;
		}
	}

	
	void OnDestroy() { GameSettings.OnUpdateStatus -= OnUpdateStatus; }

	void Disable()
	{
		gameObject.SetActive(false);
	}
}
