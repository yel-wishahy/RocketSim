using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;

public class PhysicsEngine : MonoBehaviour
{
    private static List<PhysicsRigidBody> PhysicsRigidBodies;
    // Start is called before the first frame update
    
    //phys constants
    public static double GRAV = 6.67E-11;
    public static double EARTH_RADIUS = 6378E3;
    public static double EARTH_MASS = 5.972E24;
    public static Vector3 EARTH_CENTER = Vector3.zero;
    

    //returns gravitational accelaration acting on body 
    public static float calculateGravity(PhysicsRigidBody body, PhysicsRigidBody body2)
    {
        double distance = Mathd.Abs(body.Position.magnitude - body2.Position.magnitude);
        return (float) (GRAV * body2.Mass/ Mathd.Pow(distance, 2));
    }
    
    //returns gravitational accelaration acting on body 1 due to earth
    public static float calculateGravityEarth(PhysicsRigidBody body)
    {
        float distance = Math.Abs(body.Position.magnitude - EARTH_CENTER.magnitude);
        return (float) (GRAV * EARTH_MASS * body.Mass/ (distance * distance));
    }

    public static Vector3 getDirToPos(Vector3 v1, Vector3 v2)
    {
        return (v2 - v1).normalized;
    }
}
