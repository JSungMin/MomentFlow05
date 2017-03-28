using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicBusLight : MonoBehaviour
{
    private Vector3 dir;
    private float speed = 5f;
    public bool isLeftOne;
    private const float maxTime = 16.0f;
    private float maxTimer = 0.0f;

    public TweenAlpha tweenAlpha;
    // xv = 3
    
    // zv = 10
    // s = s0 + v*3

    //  x:-9.5   z: -6
    //  x:-10.5  z: -6
    private void Awake()
    {
        float padding = isLeftOne? 0.5f : -0.5f;
        transform.position = new Vector3(-8.5f + padding - speed * maxTime * 0.1f , 0.0f, -6.5f - speed * maxTime);
        dir = Vector3.forward;
        dir += Vector3.right * 0.1f;
        dir = Vector3.Normalize(dir);
    }

    private void Update()
    {
        maxTimer += Time.deltaTime;
        transform.Translate(dir * Time.deltaTime * speed);

        if(maxTimer >= maxTime*0.8f)
        {
            tweenAlpha.PlayForward();
        }
    }
}
