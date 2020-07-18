<h1 align="center">Rotating Rigidbodies with PID Controllers</h1>

<p align="center">A demo repository for an article on rotating Rigidbodies with PID controllers.<p>

The contents of this README are the same as the article on my [website](https://robertcorponoi.com/rotating-rigidbodies-with-pid-controllers/). If you came from the website, you can just download or clone the repo and open the project in Unity to see the demo.

## What is a PID controller?

A PID controller is a type of control system that uses past values, current values, and estimated future values to drive a system towards a target value. We're going to create a PID controller and use it to drive a Rigidbody towards a desired angle.

The inspiration for this article came about when I was doing research on the best ways to rotate a Rigidbody and I stumbled upon a [forum post](https://forum.unity.com/threads/rigidbody-lookat-torque.146625/#post-1005645) that mentioned using a PID controller. The person also linked an example project which most of the code below is based off of and after I saw what it did I decided to learn more about PID controllers myself and eventually I decided to write this article to help the next person that stumbles upon this same scenario that wants a detail explanation and guide.

I'd highly recommend checking out the above linked forum post but I'm going to go over the code and expand on it a bit and an even more complex example can be seen in this repo.

Now for a quick overview of PID controllers:

- The P in PID is for proportional error. The proportional error is the flat amount that we are from the target. For example, let's say that your Rigidboy is at 50 degrees and the goal is 90 degrees. This would mean that the proportional error is 40 degrees since that''s the difference between the two. As with the other two values, this is also multiplied by a gain factor to control how much contribution it has to the overall output value.

- The I in PID is for the integral error. The integral error is a sum of the proportional values. An integral error can be helpful if you have an opposing force acting on your Rigidbody. Image a situation where you're trying to rotate your Rigidbody to a specified angle but there's force pushing the Rigidbody in the opposite direction. The force might not be big so your Rigidbody might get to 85 out of 90 degrees with just proportional errors but the opposing force is always pushing you back by 5 degrees and your Rigidbody will be stuck at 95 degrees. The integral error comes in handy here because it keeps a running total of proportional values and will apply a bigger output if the desired output is not being achieved.

- The D in PID is for the derative error. The derative error is used to provide a way of dampening the motion of the body. This is used to slow down the motion so that we don't have to do too much over/under correction.

- Each of the errors are multiplied by a constant, called a gain, before they are finally all added together and returned as the output. These gain values are used to control how much that error contributes to the overall output. For our example we'll just experiment with the gain values in the inspector as our use is not very complicated.

- If this seems like a bunch of mumbo jumbo it's ok because it was to me too but stick around for the examples as maybe it'll click in the context of Rigidbodies.

## Project Setup

To set up the project for this article we're going to create a new 3D game add a spaceship model I got from a [Kenney](https://www.kenney.nl/assets/space-kit) kit. Next we add a Rigidbody to the model which we'll apply the torque to.

Now we need to create two scripts: `PID` and `ShipController`. The PID script is where we're going to put all of the logic while the ShipController is going to use the values output by the PID controller to rotate the ship.

Let's open up the `PID` script and create the controller.

## Creating the PID Controller

In the `PID` script we don't need to do anything on `Start` or `Update` so we can just delete those and we should be left with the following:

```cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A simple PID controller component class.
/// https://forum.unity.com/threads/rigidbody-lookat-torque.146625/#post-1005645
/// </summary>
public class PID : MonoBehaviour
{
}
```

The first thing we want to set up are the properties we need:

- We need the gain values discussed earlier, The proportional, integral, and derivative each need their own gain value. Values can be set to 0 to omit it from the final output (outside the scope of this tutorial).

- We need to set up the variables to track the current proportional, integral, and derative error values.

- We need to set up a variable to keep track of what the error was the last time the output was requested. This is used by the derivative error to calculate the rate of change.

```cs
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
}
```

Believe it or not we're almost done with our simple PID controller here. We just need to create the method that takes in the current error and calculates the output which we pass to `AddTorque` in the `ShipController` script.

```cs
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
    return P*Kp + I*Ki + D*Kd;
}
```

And that's it for our PID class. If you're still a bit confused don't worry we still have example to get to. Also don't expect to fully understand PID controllers from this tutorial, but instead you should understand how to use them for tasks such as rotating Rigidbodies. If you really want to learn more about PID controllers there are great videos on the topic that helped me grasp the concept.

## Instancing the PID Script

Next we're going to make the script that gets attached to our ship and controls the Rigidbody using the PID output but before that we should attach our PID script to the ship.

So back out in the inspector we click on our ship game object and drag over the PID script twice. You read it right we need two instances of the PID class because we want to control the angle of the Rigidbody but also the angular velocity so that we can avoid abrupt changes to the angular velocity when applying the torque to change the angle.

This is also where the `Name` property in the PID script comes in handy. We can better distinguish them by naming the first one "Angle Controller" and the second one "Angular Velocity Controller".

We're going to leave the public variables alone for now until we set up our ship and have a working demo.

## Ship Controls

Now to test our PID script we have to actually use it to change our ship's rotation.

Same as before, open up the ShipController script and delete the `Start` and `Update` methods since we won't be using them and make sure you just have the following:

```cs
/// <summary>
/// Used to control the ship and simulate the effects of the PID script.
/// </summary>
public class ShipController : MonoBehavious
{
}
```

First things we want to do here is set up the variables we'll need:

- We need a public variable that defines our turn speed. This will affect how fast we get to our final angle destination.

- We also need a public variable that defines the maximum angular velocity for our Rigidbody. This will help us avoid some crazy spins.

- Next we need a private variable that holds the current angle we want to rotate to.

- Also two more private variables, one for each of our scripts attached to the game object.

- Finally one more private variable that will hold the calculated torque to pass to `AddTorque`.

```cs
/// <summary>
/// Used to control the ship and simulate the effects of the PID script.
/// </summary>
public class ShipController : MonoBehavious
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
    /// A reference to the PID instance for the angle.
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
}
```

When the ship is initialized we:

- Get the reference to the Rigidbody so we can get the angular velocity and apply the torque later.
- Get the references to both PID controller instances.
- Set the Rigidbody's max angular velocity to the value set by the user in the public variable.
- Set the initial target angle to the Rigidbody's Y Euler angle.

```cs
void Awake()
{
    // Get the reference to the Rigidbody which will be used when we apply the torque.
    rb = gameObject.GetComponent<Rigidbody>();

    // Get the references to the PID instances so we can use them as controllers.
    angleController = gameObject.GetComponents<PID>()[0];
    angularVelocityController = gameObject.GetComponents<PID>()[1];

    // Set the maximum angular velocity of the Rigidbody and set the initial value of the
    // targetAngle to the Y Euler angle. We do this so that we can easily make adjustments
    // by just adding or subtracting from it.
    rb.SetMaxAngularVelocity(maxAngularVelocity);
    targetAngle = transform.eulerAngles.y;
}
```

Now it's time for the fun stuff, the actual rotation and usage of the PID controllers. In the FixedUpdate we:

- Get the fixed delta time because we need to pass it to the PID controllers.
- Adjust the value of the `targetAngle` depending on the user input, the turn speed, and the fixed delta time.
- Get the angle error (the difference between our current angle and the angle we want to turn to) by using `DeltaAngle`. Then we pass this value to the `angleController` to get the torque that we should apply this frame.
- Get the negative of the angular velocity and pass this to the `angularVelocityController` to get the torque that we should apply this frame.
- Lastly add the two torques together and apply it to the Rigidbody.

```cs
void FixedUpdate()
{
    // Get the fixed update delta time.
    float dt = Time.fixedDeltaTime;

    // So currently the targetAngle contains the current rotation of the ship.
    // Now we need to set the targetAngle to a combination of the user's horizontal
    // input, the turn speed, and fixed delta time.
    targetAngle += Input.GetAxis("Horizontal") * turnSpeed * dt;

    // The angle controller drives the ship's angle towards the target angle.
    // This PID controller takes in the error between the ship's current rotation angle
    // and the target rotation angle as input, and returns a torque magnitude.
    // We use `DeltaAngle` to find the shortest difference between the two angles in degrees
    // which is what we need to pass to the PID controller.
    float angleError = Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle);
	float torqueCorrectionForAngle = angleController.GetOutput(angleError, dt);

    // The angular velocity controller drives the ship's angular velocity to 0.
    // This PID controller takes in the negated angular velocity of the ship and returns
    // a torque magnitude.
    float angularVelocityError = -rb.angularVelocity.y;
	float torqueCorrectionForAngularVelocity = angularVelocityController.GetOutput(angularVelocityError, dt);

    // The total torque from both controller is now applied to the ship. If the gains
    // are set correctly then the ship should rotate to the correct angle and stay there.
    torque = transform.up * (torqueCorrectionForAngle + torqueCorrectionForAngularVelocity);
	rb.AddTorque(torque);
}
```

At this point you should add the `ShipController` script to the Ship GameObject and your ship should look something like this in the inspector:

![Inspector](../../imag)
es/jul/rotating-rigidbodies-with-pid-controllers/inspector.png
## The Gain Values

Now we have to go back to the inspector and fill out the gains for both PID controllers. This can be done by running the scene and tweaking the values until you find a nice combination or you can use a set of values below highlighted by the forum poster:

### Critically Damped Controls

These gain values result in a control system that's critically damped, or at least very close to it. This means that there is the smallest amount of oscillation, which is probably not even noticable. So in our example that means that when the ship is turned, it will stay where it is.

**Angle Controller**

| Gain | Value      |
| ---- | ---------- |
| Kp   | 9.244681   |
| Ki   | 0          |
| Kd   | 0.06382979 |

**Angular Velocity Controller**

| Gain | Value      |
| ---- | ---------- |
| Kp   | 33.7766    |
| Ki   | 0          |
| Kd   | 0.2553191  |

### Fast Controls

These gain values result in a control system that has fast controls meaning that the turning is very responsive and quick in comparison to other gain values.

**Angle Controller**

| Gain | Value      |
| ---- | ---------- |
| Kp   | 33.51064   |
| Ki   | 0          |
| Kd   | 0.02127661 |

**Angular Velocity Controller**

| Gain | Value      |
| ---- | ---------- |
| Kp   | 46.54256   |
| Ki   | 0          |
| Kd   | 0.1808511  |

### Spongy Controls

These gain values result in a control system that has slow, less responsive controls.

**Angle Controller**

| Gain | Value      |
| ---- | ---------- |
| Kp   | 0.7093059  |
| Ki   | 0          |
| Kd   | 0          |

**Angular Velocity Controller**

| Gain | Value      |
| ---- | ---------- |
| Kp   | 11.17021   |
| Ki   | 0          |
| Kd   | 0          |

### Springy Controls

These gain values result in a control system that has springy controls so when you turn the ship you'll notice that it overshoots the target angle and bounces back to the correct angle.

**Angle Controller**

| Gain | Value      |
| ---- | ---------- |
| Kp   | 0.7093059  |
| Ki   | 0          |
| Kd   | 0.1648936  |

**Angular Velocity Controller**

| Gain | Value      |
| ---- | ---------- |
| Kp   | 0          |
| Ki   | 0          |
| Kd   | 0          |

Note that the values are for a Rigidbody without a collision shape. You'll need to tweak your values if you add a collision shape.

## Conclusion

All that's left now is to play the scene and try turning the ship with the left/right arrow keys and seeing it in action. Feel free to try out different combinations of PID values while the game is running to find the perfect combination for you.

Also check out the example in the repo for a more complex demo.