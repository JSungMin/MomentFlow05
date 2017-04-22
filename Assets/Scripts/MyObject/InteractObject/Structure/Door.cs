using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : InteractInterface
{
    private BoxCollider2D boxCollider;

    private new void Awake()
    {
        base.Awake();
        interact += OpenTheDoor;
        boxCollider = GetComponent<BoxCollider2D>();
    }
    
    private void OpenTheDoor()
    {
        if (!isInteract)
        {
            boxCollider.enabled = false;
            Debug.Log("door is opened");
            isInteract = true;
        }
    }
}
