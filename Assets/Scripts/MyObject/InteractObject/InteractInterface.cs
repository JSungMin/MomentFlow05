using System.Collections;
using System.Collections.Generic;
using cakeslice;
using UnityEngine;

public class InteractInterface : MonoBehaviour
{
    // 이미 인터렉트를 했냐를 구분하는 변수
	public bool isInteracted = false;

	public delegate void InteractWithObject();
	public delegate void StopInteractWithObject ();

	protected InteractWithObject interact;
	protected StopInteractWithObject stopInteract;

    protected Outline outline;

    protected void Awake()
    {
        outline = GetComponent<Outline>();
        if (outline == null)
            outline = GetComponentInChildren<Outline>();
        if (outline == null)
            outline = gameObject.AddComponent<Outline>();
    }

    public void Interact()
    {
        if (interact != null)
            interact();
    }
    
    public void StopInteract()
    {
        if(stopInteract != null)
            stopInteract();
    }
}
