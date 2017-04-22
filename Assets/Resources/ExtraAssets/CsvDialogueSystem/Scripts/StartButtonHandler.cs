using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Disable dialog start button while dialog is active
/// </summary>
public class StartButtonHandler : MonoBehaviour
{
	void Update ()
    {
                                                                                    // While dialog not started button is interactive
                                                                                    // otherwise it is inactive
        GetComponent<Image>().raycastTarget = !DialogueManager.Instance.IsDialogInProgress();
	}
}
