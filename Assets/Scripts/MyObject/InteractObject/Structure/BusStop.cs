using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusStop : InteractInterface
{
    private Scene01 scene01;

    private new void Awake()
    {
        base.Awake();
        scene01 = GameObject.FindObjectOfType<Scene01>();
        interact = CallBus;
    }

    private void CallBus()
    {
        if (!isInteracted)
        {
            isInteracted = true;
            scene01.MakeBusComes();
        }
    }
}
