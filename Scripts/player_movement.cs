using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_movement : MonoBehaviour
{
    public Joystick joystick;
    public float speed = 4f;
    Rigidbody rb;
    Vector3 velocity_vector = Vector3.zero;
    float x_movement_input;
    float z_movement_input;
    Vector3 x_speed;
    Vector3 z_speed;
    Vector3 resultant_vel;
    private float max_value=2f;
    private float tiltamount = 10;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // TAKING JOYSTICK INPUT

        x_movement_input = joystick.Horizontal;
        z_movement_input = joystick.Vertical;

        //CALCULATING VELOCITY VECTORS

        x_speed = transform.right * x_movement_input;
        z_speed = transform.forward * z_movement_input;

        //CALCULATING RESULTANT VELOCITY

        resultant_vel = (x_speed + z_speed).normalized * speed;
        move(resultant_vel);
        transform.rotation = Quaternion.Euler(joystick.Vertical * speed * tiltamount, 0, (-1)*joystick.Horizontal * speed * tiltamount);
    }

    private void move(Vector3 resultant_vel)
    {
        velocity_vector = resultant_vel;
    }
    private void FixedUpdate()
    {
        Vector3 velocity = rb.velocity;
        Vector3 velocity_change = (velocity_vector - velocity);
        velocity_change.x = Mathf.Clamp(velocity_change.x, -max_value, max_value);
        velocity_change.z = Mathf.Clamp(velocity_change.z, -max_value, max_value);
        velocity_change.y = 0;
        rb.AddForce(velocity_change,ForceMode.Acceleration);
    }
}
