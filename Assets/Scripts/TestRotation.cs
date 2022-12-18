using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRotation : MonoBehaviour
{
    public Transform targetPlain;
    public Transform target;

    public float rotationSpeed = 45.0f;

    public Space space;

    public bool updateRotation;

    void Update()
    {
        // transform.Rotate(targetPlain.up, rotationSpeed * Time.deltaTime, space);

        if (updateRotation)
        {
            transform.rotation = targetPlain.rotation;
            updateRotation = false;
        }

        transform.RotateAround(transform.position, transform.up, rotationSpeed * Time.deltaTime);

        // Look at the target object
        // transform.LookAt(target);
    }
}
