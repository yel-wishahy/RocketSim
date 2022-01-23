using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PhysicsRigidBody
{
    public float lastGravity;
    private Vector3 position;
    private Vector3 rotation;
    private Vector3 velocity;
    private Vector3 angularVecloity;
    private Vector3 netForce;
    private Vector3 netTorque;
    private Vector3 thurstDir;
    public  Transform _transform;
    private float mass;
    private float momentOfInertia;
    // Start is called before the first frame update

    public PhysicsRigidBody(Transform transform, float mass, float momentOfInertia)
    {
        _transform = transform;
        this.mass = mass;
        position = _transform.position;
        rotation = Vector3.zero;
        velocity = Vector3.zero;
        angularVecloity = Vector3.zero;
        netForce = Vector3.zero;
        netTorque = Vector3.zero;
        thurstDir = _transform.up;
        this.momentOfInertia = momentOfInertia;
    }

    public void updateTransform()
    {
        _transform.rotation = Quaternion.Euler(rotation) * _transform.rotation;
        thurstDir = _transform.up;
        _transform.position = position;
    }

    public Vector3 Position
    {
        get => position;
        set => position = value;
    }

    public Vector3 Velocity
    {
        get => velocity;
        set => velocity = value;
    }

    public Vector3 AngularVecloity
    {
        get => angularVecloity;
        set => angularVecloity = value;
    }

    public Vector3 NetForce
    {
        get => netForce;
        set => netForce = value;
    }

    public float Mass
    {
        get => mass;
        set => mass = value;
    }

    public Vector3 ThurstDir
    {
        get => thurstDir;
        set => thurstDir = value;
    }

    public Vector3 NetTorque
    {
        get => netTorque;
        set => netTorque = value;
    }

    public float MomentOfInertia
    {
        get => momentOfInertia;
        set => momentOfInertia = value;
    }

    public Vector3 Rotation
    {
        get => rotation;
        set => rotation = value;
    }
}
