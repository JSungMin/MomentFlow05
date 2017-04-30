using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : InteractInterface
{
    private Door door;

    private new void Awake()
    {
        base.Awake();
        door = GetComponentInParent<Door>();
        interact += OpenOrCloseDoor;
    }

    private void OpenOrCloseDoor()
    {
        if (door.isOpened)
            door.CloseDoor();
        else
            door.OpenDoor();
    }
}
