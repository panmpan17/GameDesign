using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PhysicSimulate
{
    [Min(0.001f)]
    public float Mass = 1;
    public Vector3 Gravity = new Vector3(0, -9.8f, 0);

    private Vector3 _position;
    private Vector3 _accelerateion;
    private Vector3 _velocity;

    public Vector3 Position => _position;
    public Vector3 Velocity => _velocity;

    public void SetPositionAndVelocity(Vector3 position, Vector3 velocity)
    {
        _position = position;
        _velocity = velocity;
    }

    public void Update(float deltaTime)
    {
        _accelerateion = Vector3.zero;
        _accelerateion += Gravity;
        _velocity += (_accelerateion * deltaTime) / Mass;
        _position += _velocity * deltaTime;
    }
}
