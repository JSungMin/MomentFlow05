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
        // TODO: 버그 처리하기, 왜 이것이 없으면 안되는가
        stopInteract += OpenOrCloseDoor;
    }

    private void OpenOrCloseDoor()
    {
        if (!isInteract)
        {
            if (door.isOpened)
                door.CloseDoor();
            else
                door.OpenDoor();
            isInteract = true;
        }
    }
}
