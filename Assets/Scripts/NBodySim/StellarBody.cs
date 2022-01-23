using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Research.Oslo;
using NBodySim;
using UnityEngine;

public class StellarBody : MonoBehaviour
{
    public Rigidbody _rigidbody;
    public Vector3 initialVelocity;
    public Vector3 velocity;
    public float mass;
    public bool isRocket;
    //ode solution describing evolution of system! (highly experimental)
    private IEnumerable<SolPoint> ODE;
    
    // Start is called before the first frame update
    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.mass = mass;
        velocity = initialVelocity;
    }

    // Update is called once per frame
    public void UpdateVelocity(StellarBody[] stellarBodies, SimMode simMode = 0)
    {
        if (!isRocket)
        {
            if(simMode == SimMode.Iterate)
                UpdateVelocity_Iterate(stellarBodies);
            if (simMode == SimMode.IntegrateODE)
            {
                UpdateVelocity_SolveODE(stellarBodies,0,NBodySimulation.Instance.currTime);
            }
        }
        else
        {
            velocity = FindObjectOfType<RocketController>().rocketRigidBody.Velocity;
        }
        
    }
    
    private void UpdateVelocity_Iterate(StellarBody[] stellarBodies)
    {
        foreach (StellarBody body2 in stellarBodies)
        {
            if (body2 != this)
            {
                Vector3 forceDir = (body2._rigidbody.position - _rigidbody.position).normalized;
                float distanceSqr = (body2._rigidbody.position - _rigidbody.position).sqrMagnitude;
                Vector3 accel = (NBodySimulation.gravitationalConstant * body2.mass / distanceSqr)*forceDir;
                velocity += accel * Time.fixedDeltaTime;
            }
        }
    }
     
    private void UpdateVelocity_SolveODE(StellarBody[] stellarBodies, float startTime, float endTime)
    {
        if(ODE == null)
            NBodySimulation.Instance.TwoBody_setup_ODE(stellarBodies); 
        
        var velocities = ODE.SolveFromToStep(startTime , endTime, NBodySimulation.timeStep).ToArray();
        Vector sol = velocities.Last().X;
        Debug.Log(velocities.Length);
        Debug.Log(sol);
        velocity.Set((float) sol[0],(float)sol[1],(float)sol[2]);
    }

    public void UpdatePosition(SimMode simMode = 0)
    {
        if (!isRocket)
            _rigidbody.MovePosition(_rigidbody.position + velocity * Time.fixedDeltaTime);
        else
            _rigidbody.position = FindObjectOfType<RocketController>().rocketRigidBody.Position;
    }

    public IEnumerable<SolPoint> Ode
    {
        get => ODE;
        set => ODE = value;
    }
    
    
}


