using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class RocketController : MonoBehaviour
{
    public PhysicsRigidBody rocketRigidBody;
    public float forceMag = 1;
    public float angForceMag = 50;
    public float mass_kg = 1000;
    public float mom_of_intertia;
    public float force;
    

    // Start is called before the first frame update
    void Awake()
    {
        rocketRigidBody = new PhysicsRigidBody(transform, mass_kg, mom_of_intertia);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            rocketRigidBody.NetForce += forceMag * rocketRigidBody.ThurstDir;
        }
        
        if (Input.GetKey(KeyCode.DownArrow))
        {
            if(Vector3.Angle(rocketRigidBody.ThurstDir, rocketRigidBody.NetForce - forceMag * rocketRigidBody.ThurstDir) == 0)
                rocketRigidBody.NetForce -= forceMag * rocketRigidBody.ThurstDir;
            else
                rocketRigidBody.NetForce = Vector3.zero;
            
        }

        if (Input.GetKey(KeyCode.W))
            rocketRigidBody.NetTorque += angForceMag * Vector3.right;
        if (Input.GetKey(KeyCode.S))
            rocketRigidBody.NetTorque += angForceMag * Vector3.left;

        if (Input.GetKey(KeyCode.A))
            rocketRigidBody.NetTorque += angForceMag * Vector3.forward;
        if (Input.GetKey(KeyCode.D))
            rocketRigidBody.NetTorque += angForceMag * Vector3.back;

        if (Input.GetKey(KeyCode.Alpha1))
        {
            rocketRigidBody.NetForce= Vector3.zero;
            rocketRigidBody.Velocity = Vector3.zero;
        }
        if (Input.GetKey(KeyCode.Alpha2))
        {
            rocketRigidBody.NetTorque = Vector3.zero;
            rocketRigidBody.AngularVecloity = Vector3.zero;
            rocketRigidBody.Rotation = Vector3.zero;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 gravAccel = NBodySimulation.Instance.calculateAccel(rocketRigidBody.Position);
        rocketRigidBody.lastGravity = gravAccel.magnitude;
        rocketRigidBody.Velocity += rocketRigidBody.NetForce / rocketRigidBody.Mass * Time.fixedDeltaTime;
        rocketRigidBody.Velocity += gravAccel * Time.fixedDeltaTime;

        rocketRigidBody.AngularVecloity += rocketRigidBody.NetTorque / rocketRigidBody.MomentOfInertia * Time.fixedDeltaTime;

        rocketRigidBody.Rotation += rocketRigidBody.AngularVecloity * Time.fixedDeltaTime;
        rocketRigidBody.Position += rocketRigidBody.Velocity * Time.fixedDeltaTime;
        rocketRigidBody.updateTransform();
    }
}
