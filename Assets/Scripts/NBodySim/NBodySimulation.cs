using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Research.Oslo;
using NBodySim;
using UnityEditor;
using UnityEngine;
//using Microsoft.Research.Oslo;

public class NBodySimulation : MonoBehaviour
{
    static NBodySimulation instance;
    public StellarBody[] StellarBodies;
    public SimMode SimMode;
    public float currTime;

    public static float gravitationalConstant = 0.0001f;
    public static float timeStep = 0.01f;

    private void Awake()
    {
        Time.fixedDeltaTime = timeStep;
        StellarBodies = FindObjectsOfType<StellarBody> ();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        currTime += Time.fixedDeltaTime;
        foreach (StellarBody body in StellarBodies)
            body.UpdateVelocity(StellarBodies, SimMode);
        
        foreach (StellarBody body in StellarBodies)
            body.UpdatePosition(SimMode);
    }

    public Vector3 calculateAccel(Vector3 position)
    {
        Vector3 accel = Vector3.zero;
        foreach (StellarBody body in StellarBodies)
        {
            Vector3 forceDir = (body._rigidbody.position - position).normalized;
            float distanceSqr = (body._rigidbody.position - position).sqrMagnitude;
            accel += (gravitationalConstant * body.mass / distanceSqr)*forceDir;
        }

        return accel;
    }

    public List<IEnumerable<SolPoint>> TwoBody_setup_ODE(StellarBody[] stellarBodies)
    {
        List<IEnumerable<SolPoint>> createdOdes = new List<IEnumerable<SolPoint>>();
        foreach (var body in stellarBodies)
        {
            StellarBody otherBody = stellarBodies[0];
            if (body == stellarBodies[0]) 
                otherBody = stellarBodies[1];
            //initial condition for two body problem : initial velocities @ t=0
            Vector initConditions = new Vector(body.initialVelocity.x, body.initialVelocity.y
                , body.initialVelocity.z);
            
            float const1 = gravitationalConstant * otherBody.mass;
            float const2 = (float) Math.Pow( (otherBody._rigidbody.position - body._rigidbody.position).magnitude, 3);
            float const3 = const1 / const2;
            
            // var sol = Ode.RK547M(
            //     0,
            //     initConditions,
            //     (t, x) => new Vector(const3 * (otherBody._rigidbody.position.x - x[0]),
            //         const3 * (otherBody._rigidbody.position.y - x[1]),
            //         const3 * (otherBody._rigidbody.position.z - x[2])));
            
            Vector odeRHS(double t, Vector x)
            {
                Vector3 pos = vectorToVector3(x);
                Vector3 ans = gravitationalConstant * otherBody.mass *
                              (otherBody._rigidbody.position - pos).normalized /
                              (otherBody._rigidbody.position - pos).sqrMagnitude;

                return vector3ToVector(ans);
            }
            
            var sol = Ode.RK547M(
                0,
                initConditions,odeRHS);
            
            body.Ode = sol;
            createdOdes.Add(sol);
        }

        return createdOdes;
    }
    public SolPoint[][] TwoBody_solve_ODE(StellarBody[] stellarBodies, float finalTime, float timeStep)
    {
        List<IEnumerable<SolPoint>> odes = TwoBody_setup_ODE(stellarBodies);
        var sols = new SolPoint[odes.Count][];
        for(int i = 0; i < sols.Length; i++)
            sols[i] = odes[i].SolveFromToStep(0 , finalTime,timeStep).ToArray();
        return sols;
    }

    public static NBodySimulation Instance
    {
        get
        {
            if(instance == null)
                instance = FindObjectOfType<NBodySimulation>();
            return instance;
        }
    }
    
    public static Vector3d vectorToVector3d(Vector vector)
    {
        return new Vector3d(vector[0], vector[1], vector[2]);
    }
    
    public static Vector3 vectorToVector3(Vector vector)
    {
        return new Vector3((float)vector[0], (float)vector[1], (float)vector[2]);
    }

    public static Vector vector3ToVector(Vector3 vector)
    {
        return new Vector(vector.x, vector.y, vector.z);
    }
    
}
