using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractInterface : MonoBehaviour {
	public bool isInteract = false;

	public delegate void InteractWithObject();
	public delegate void StopInteractWithObject ();

	protected InteractWithObject interact;
	protected StopInteractWithObject stopInteract;

    public void Interact()
    {
        interact();
    }

    public void StopInteract()
    {
        stopInteract();
    }
}
