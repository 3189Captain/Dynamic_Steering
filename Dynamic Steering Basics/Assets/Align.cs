﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Align
{
    public Kinematic character;
    public GameObject target;

    float maxAngularAcceleration = 100f; // 5
    float maxRotation = 45f; // maxAngularVelocity

    // the radius for arriving at the target
    float targetRadius = 1f;

    // the radius for beginning to slow down
    float slowRadius = 10f;

    // the time over which to achieve target speed
    float timeToTarget = 0.1f;

    // returns the angle in degrees that we want to align with
    // Align will rotate to match the target's oriention
    // sub-classes can overwrite this function to set a different target angle e.g. to face a target
    public virtual float getTargetAngle()
    {
        return target.transform.eulerAngles.y;
    }

    public SteeringOutput getSteering()
    {
        SteeringOutput result = new SteeringOutput();

        // get the naive direction to the target
        //float rotation = Mathf.DeltaAngle(character.transform.eulerAngles.y, target.transform.eulerAngles.y);
        float rotation = Mathf.DeltaAngle(character.transform.eulerAngles.y, getTargetAngle());
        //Debug.Log(rotation);
        float rotationSize = Mathf.Abs(rotation);

        // check if we are there, return no steering
        //if (rotationSize < targetRadius)
        //{
        //    return null;
        //}

        // if we are outside the slow radius, then use maximum rotation
        float targetRotation = 0.0f;
        if (rotationSize > slowRadius)
        {
            targetRotation = maxRotation;
        }
        else // otherwise use a scaled rotation
        {
            targetRotation = maxRotation * rotationSize / slowRadius;
        }

        // the final targetRotation combines speed (already in the variable) and direction
        targetRotation *= rotation / rotationSize;
        //Debug.Log(targetRotation);

        // acceleration tries to get to the target rotation
        float currentAngularVelocity = float.IsNaN(character.angularVelocity) ? 0f : character.angularVelocity;
        result.angular = targetRotation - currentAngularVelocity;
        //Debug.Log(character.angularVelocity);
        result.angular /= timeToTarget;
        //Debug.Log(result.angular);

        // check if the acceleration is too great
        float angularAcceleration = Mathf.Abs(result.angular);
        if (angularAcceleration > maxAngularAcceleration)
        {
            result.angular /= angularAcceleration;
            result.angular *= maxAngularAcceleration;
        }
        //Debug.Log(result.angular);

        result.linear = Vector3.zero;
        return result;
    }
}