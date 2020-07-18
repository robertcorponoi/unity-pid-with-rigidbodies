using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Uses the PID controllers to control the yaw and roll of the ship.
/// </summary>
public class ShipController : MonoBehaviour
{
    /// <summary>
    /// The speed at which the ship will turn at.
    /// </summary>
    public float turnSpeed = 500.0f;

    /// <summary>
    /// The maximum angular velocity of the Rigidbody.
    /// </summary>
    public float maxAngularVelocity = 20.0f;

    /// <summary>
    /// The current angle to turn towards.
    /// </summary>
    private float targetAngle;

    /// <summary>
    /// A reference to the PID instance for the yaw controller.
    /// </summary>
    private PID angleController;

    /// <summary>
    /// A reference to the PID instance for the angular velocity.
    /// </summary>
    private PID angularVelocityController;

    /// <summary>
    /// The torque to apply to the Rigidbody.
    /// </summary>
    private Vector3 torque;

    /// <summary>
    /// A reference to the Rigidbody component.
    /// <summary>
    private Rigidbody rb;

    /// <summary>
    /// The current yaw input.
    /// </summary>
    private float angleInput;

    /// <summary>
    /// On Awake we get the references to the Rigidbody and the PID controller script
    /// instances and set the starting values for the angles.
    /// </summary>
    void Awake()
    {
        // Get the reference to the Rigidbody which will be used when we apply the torque.
        rb = gameObject.GetComponent<Rigidbody>();

        // Get the references to the PID instances so we can use them as controllers.
        angleController = gameObject.GetComponents<PID>()[0];
        angularVelocityController = gameObject.GetComponents<PID>()[1];

        // Set the maximum angular velocity of the Rigidbody.
        rb.SetMaxAngularVelocity(maxAngularVelocity);

        // Set the initial yawAngle to the Y Euler angle and the initial roll angle to the
        // X Euler angle so we can +/- from them as needed.
        targetAngle = transform.eulerAngles.z;
    }

    /// <summary>
    /// Every physics frame we calculate the current yaw and roll and apply the
    /// forces to the Rigidbody.
    /// </summary>
    void FixedUpdate()
    {
        // Get the fixed update delta time.
        float dt = Time.fixedDeltaTime;

        // Now we need to adjust the yawAngle and rollAngle based on the user's horizontal
        // input, the turn speed, and fixed delta time.
        targetAngle += angleInput * turnSpeed * dt;

        // The angle controller drives the ship's angle towards the target angle.
        // This PID controller takes in the error between the ship's current rotation angle
        // and the target rotation angle as input, and returns a torque magnitude.
        // We use `DeltaAngle` to find the shortest difference between the two angles in degrees
        // which is what we need to pass to the PID controller.
        float angleError = Mathf.DeltaAngle(transform.eulerAngles.z, targetAngle);
        float torqueCorrectionForAngle = angleController.GetOutput(angleError, dt);

        // The angular velocity controller drives the ship's angular velocity to 0.
        // This PID controller takes in the negated angular velocity of the ship and returns
        // a torque magnitude.
        float angularVelocityError = -rb.angularVelocity.z;
        float torqueCorrectionForAngularVelocity = angularVelocityController.GetOutput(angularVelocityError, dt);

        // The total torque from both controller is now applied to the ship. If the gains
        // are set correctly then the ship should rotate to the correct angle and stay there.
        torque = transform.forward * (torqueCorrectionForAngle + torqueCorrectionForAngularVelocity);
        rb.AddTorque(torque);
    }

    /// <summary>
    /// When the user presses the q or e keys we update the value that gets used
    /// by the roll calculations.
    /// </summary>
    /// <param name="value">The callback context for the roll input.</param>
    void OnRoll(InputValue value)
    {
        angleInput = value.Get<float>();
    }
}
