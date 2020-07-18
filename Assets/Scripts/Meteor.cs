using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the rotation and movement of a meteor.
/// </summary>
public class Meteor : MonoBehaviour
{
    /// <summary>
    /// The Rigidbody component of the meteor.
    /// </summary>
    public Rigidbody rigidbody;

    /// <summary>
    /// The amount of acceleration to apply to the meteor.
    /// </summary>
    public float force = 0.5f;

    /// <summary>
    /// The amount of torque to apply to the meteor on the x axis.
    /// </summary>
    public float xTorque = 0.05f;

    /// <summary>
    /// The amount of torque to apply to the meteor on the y axis.
    /// </summary>
    public float yTorque = 0.025f;

    /// <summary>
    /// The amount of torque to apply to the meteor on the z axis.
    /// </summary>
    public float zTorque = 0.001f;

    /// <summary>
    /// Every physics frame we update the rotation and position of the meteor.
    /// </summary>
    void Update()
    {
        rigidbody.AddTorque(new Vector3(xTorque, yTorque, zTorque), ForceMode.Impulse);

        rigidbody.AddForce(new Vector3(0, 0, force), ForceMode.Acceleration);
    }
}
