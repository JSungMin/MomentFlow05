using UnityEngine;
using Random = UnityEngine.Random;
using System.Collections.Generic;

public class GameSettings : Singleton<GameSettings>
{

	[SerializeField] private float bottomLevel = 0.9f;
	[SerializeField] private float middleLevel = 4.6f;
	[SerializeField] private GameObject[] buildings;
	[SerializeField] private MapStatus status;
	[SerializeField] private Notifications notifications;
	private bool start = true;
    public BuildingType selectedBuilding;

    private List<GameObject> createdBuildings;

    public enum BuildingType
    {
        CityHall,
        Apartment,
        Shop,
        Office,
        Elevator,
        Power,
        None
    }

	public enum MapStatus
	{
		ExampleCity = 0,
		HideCity = 1,
		AllowBuilding = 2,
	};

	public delegate void UpdateStatus(MapStatus Status);
	public static event UpdateStatus OnUpdateStatus;

	void Awake ()
	{
		Instance = this;
	}


	void Start()
	{
        createdBuildings = new List<GameObject>();
        ChangeStatus(status);
	}

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (createdBuildings.Count > 0)
            {
                DestroyBuilding(createdBuildings[createdBuildings.Count - 1]);
                createdBuildings.RemoveAt(createdBuildings.Count - 1);
            }
        }
    }


	public float GetBottomLevel()
	{
		return bottomLevel;
	}

	public float GetMiddleLevel()
	{
		return middleLevel;
	}
   
	/// <summary>
	/// Function to build a building on the map.
	/// </summary>
	/// <param name="target">Place to build building.</param>
	public void Build(Transform target)
	{
		int buildingNumber;
        if (start)
        {
            buildingNumber = 0;
            start = false;
        }
        else
        {
            if (selectedBuilding != BuildingType.None)
            {
                buildingNumber = (int) selectedBuilding;
            }
            else
            {
                buildingNumber = Random.Range(1, 6);
            }
        }
		Vector3 pos = new Vector3(target.position.x, target.position.y - 0.264f, target.position.z);
		GameObject building = Instantiate(buildings[buildingNumber], pos, Quaternion.identity) as GameObject;
		//triggers animation while placing the building
		building.GetComponent<Animator>().SetTrigger("Show");
		notifications.AdvanceDialoque(building, buildingNumber);
        createdBuildings.Add(building);
    }

    public void DestroyBuilding(GameObject building)
    {
        building.GetComponent<Animator>().SetTrigger("Hide");
        //notifications.AdvanceDialoque(building, buildingNumber);
    }

	public void RestartLevel()
	{
		Application.LoadLevel(Application.loadedLevel);
	}

	public void ExitLevel()
	{
		Application.Quit();
	}

	/// <summary>
	/// Function that changes status of the buildings in the level.
	/// </summary>
	/// <param name="status">status of the buildings</param>
	public void ChangeStatus(MapStatus status)
	{
		if (OnUpdateStatus != null)
		{
			OnUpdateStatus(status);
		}
	}

	/// <summary>
	/// Hides city at the beginning of the demo.
	/// </summary>
	public void HideExampleCity()
	{
		ChangeStatus(MapStatus.HideCity);
	}

}
