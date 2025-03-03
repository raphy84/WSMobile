using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Player : MonoBehaviour
{
    public S_JoistickMovement movementJoystick;
    public float playerSpeed;
    private Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (movementJoystick.joystickVEC.y != 0) 
        {
            rb.linearVelocity = new Vector2(movementJoystick.joystickVEC.x * playerSpeed, movementJoystick.joystickVEC.y * playerSpeed);
        }

        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
}
