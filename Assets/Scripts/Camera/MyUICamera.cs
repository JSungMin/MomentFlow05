using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyUICamera : MonoBehaviour
{
    public GameObject flowObject;

    private void Awake()
    {
    }

    private void Update()
    {
        transform.position = flowObject.transform.position + Vector3.up * 0.5f;
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0.0f);
    }
}
