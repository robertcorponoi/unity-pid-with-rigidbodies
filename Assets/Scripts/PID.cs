using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A simple PID controller component class.
/// https://forum.unity.com/threads/rigidbody-lookat-torque.146625/#post-1005645
/// </summary>
public class PID : MonoBehaviour
{
    /// <summary>
    /// This is completely optional and is used just to help you idenfity what this PID
    /// script is for as you can use multiple PID scripts per game object to control
    /// various values.
    /// </summary>
    public string name;

    /// <summary>
    /// The gain of the proportional error. This defines how much weight the proportional
    /// error has in the final output.
    /// </summary>
    public float Kp = 1.0f;

    /// <summary>
    /// The gain of the integral error. This defines how much weight the integral
    /// error has in the final output.
    /// </summary>
    public float Ki = 0.0f;

    /// <summary>
    /// The gain of the derivative error. This defines how much weight the derivative
    /// error has in the final output.
    /// </summary>
    public float Kd = 0.1f;

    /// <summary>
    /// Tracks the current values of the proportional, integral, and derative errors.
    /// </summary>
    private float P, I, D;

    /// <summary>
    /// Used to keep track of what the error value was the last time the output was requested.
    /// This is used by the derivative to calculate the rate of change.
    /// </summary>
    private float previousError;

    /// <summary>
    /// Returns the amount of torque that should be applied to the ship this frame
    /// to reach the target rotation.
    /// </summary>
    /// <returns>
    /// The output of the PID calculation.
    /// </returns>
    /// <param name="currentError">How far away the body is from the target rotation angle.</param>
    /// <param name="deltaTime">The delta value from FixedUpdate.</param>
    public float GetOutput(float currentError, float deltaTime)
    {
        // First we set the proportional to how how far we are from the target value.
        P = currentError;

        // Since the integral is the sum of the proportional error over time we have
        // to multiply the propertional by the delta time and then add it on to the
        // current I value.
        I += P * deltaTime;

        // Set the derative to the rate of change of error by subtracting the current
        // error from the previous error to get the difference and then dividing it by
        // the amount of time it took to get to this change.
        D = (P - previousError) / deltaTime;

        // Now we set the previous error to the current error to prepare for the next
        // time the output is requested.
        previousError = currentError;

        // Finally calculate the output using the values above multiplied by their gain.
        return P * Kp + I * Ki + D * Kd;
    }
}