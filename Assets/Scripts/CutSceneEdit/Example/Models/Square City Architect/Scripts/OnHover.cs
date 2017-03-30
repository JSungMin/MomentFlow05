using UnityEngine;
using UnityEngine.UI;

public class OnHover : MonoBehaviour
{

	[SerializeField] private Text TextField;
	[SerializeField] private string Text;

	/// <summary>
	/// Assigns text to TextField on hover.
	/// </summary>
	public void OnHoverBtn ()
	{
		TextField.text = Text;
	}

}
