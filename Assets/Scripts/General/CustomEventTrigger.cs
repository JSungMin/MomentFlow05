using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CustomEventTrigger : MonoBehaviour
{
    private Player player;
    public UnityEvent triggerEvent;
    public FrontWall frontWall;

    private void Awake()
    {
        player = GameObject.FindObjectOfType<Player>();
    }

    public void StartTriggerEvent()
    {
        triggerEvent.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Player>() != null)
        {
            if (other.transform.position.x > transform.position.x)
                frontWall.TransparentSlowly();
            else
                frontWall.UnTransparentSlowly();
        }
    }
}
