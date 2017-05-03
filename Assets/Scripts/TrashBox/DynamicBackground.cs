using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicBackground : MonoBehaviour
{
    public float leftestPosX;
    public float rightestPosX;
    public float moveSpeed;
    private bool isMove;

    private void Awake()
    {
        leftestPosX = -21.0f;
        moveSpeed = 2.2f;
    }

    private void Update()
    {
        if (isMove)
        {
            transform.Translate(Vector3.left * Time.deltaTime * moveSpeed);
        }
    }

    public IEnumerator StopSlowly()
    {
        while(moveSpeed>0)
        {
            moveSpeed -= 0.01f;
            yield return new WaitForSeconds(0.01f);
        }

        moveSpeed = 0.0f;
    }
}
