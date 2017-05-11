using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : InteractInterface
{
    private ThrowableObjectScript bigBox;

    private new void Awake()
    {
        base.Awake();
        bigBox = GetComponentInChildren<ThrowableObjectScript>();

        interact += Activate;
    }

    private void Activate()
    {
        if(!isInteracted)
        {
            isInteracted = true;
            bigBox.applyGravity = true;
        }
    }

    private void InActivate()
    {

    }
}
