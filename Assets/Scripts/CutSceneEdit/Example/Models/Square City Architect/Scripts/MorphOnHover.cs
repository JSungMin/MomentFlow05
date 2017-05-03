using UnityEngine;

public class MorphOnHover : MonoBehaviour
{

	[SerializeField] private Animator IconAnimator;

	/// <summary>
	/// Morphs the icon on hover.
	/// </summary>
	/// <param name="Morph">Morph forward(true) or reverse morphing(false).</param>
	public void OnHover (bool Morph) 
	{
		IconAnimator.SetBool("Morph", Morph);	
	}
}
