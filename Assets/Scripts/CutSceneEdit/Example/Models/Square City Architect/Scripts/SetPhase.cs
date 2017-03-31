using UnityEngine;
using System.Collections;

public class SetPhase : MonoBehaviour
{

	[SerializeField] private Renderer treeRenderer;
	private Material treeMaterial;
	
	void Start ()
	{
		treeMaterial = treeRenderer.material;
		treeMaterial.SetFloat("_Phase", Random.Range(0, 5));
	}

}
