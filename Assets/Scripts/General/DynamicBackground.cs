using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicBackground : MonoBehaviour
{
    public float leftestPosX;
    public float rightestPosX;
    private float moveSpeed;
    private bool isMove;

    private void Awake()
    {
        leftestPosX = -21.0f;
        moveSpeed = 1.0f;
        isMove = false;
        StartCoroutine(MoveStartDelay());
    }

    private void Update()
    {
        if (isMove)
        {
            transform.Translate(Vector3.left * Time.deltaTime * moveSpeed);
            if (transform.position.x < leftestPosX)
                transform.position = new Vector3(rightestPosX, transform.position.y, transform.position.z);
        }
    }

    private IEnumerator MoveStartDelay()
    {
        yield return new WaitForSeconds(3.0f);

        isMove = true;
    }
}
