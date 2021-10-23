using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sample character script demonstrating how to send inputs to a motor
/// </summary>
public class SamplePlayerCharacter : MonoBehaviour
{
    // The motor we're controlling
    public KinematicPlayerMotor motor;

    private void Update() //update for the new input manager
    {
        // send inputs to motor
        motor.MoveInput(new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical")));
        if (Input.GetButtonDown("Jump"))
        {
            motor.JumpInput();
        }
    }
}
