using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverSeer : MonoBehaviour
{
    public Vector3 velocity;
    private float gravity;
    private Controller2D controller;


    void ProcessMove()
    {
        velocity.y += gravity * Time.deltaTime;
    }

    void Start()
    {
        gravity = -9.8f;
    }

    // Update is called once per frame
    void Update()
    {
        ProcessMove();
        velocity.x = 0;
        controller = GetComponent<Controller2D>();
        controller.Move(velocity * Time.deltaTime);
    }
}
