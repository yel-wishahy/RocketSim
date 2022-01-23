using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TelemetryText : MonoBehaviour
{ 
    private Text _text;

    public RocketController RocketController;
    // Start is called before the first frame update
    void Awake()
    {
        _text = GetComponent<Text>();

    }

    // Update is called once per frame
    void Update()
    {
        string msg = "Thust: " + RocketController.rocketRigidBody.NetForce.magnitude + 
                     " N\nVelocity: " 
                     + RocketController.rocketRigidBody.Velocity.magnitude+ " m/s\n" +
                     "Distance: " + RocketController.rocketRigidBody.Position.magnitude + " m\n" +
                     "Gravity: " + RocketController.rocketRigidBody.lastGravity/9.81d + " g\n"
                     + "Thrust Vector: " + RocketController.rocketRigidBody.ThurstDir + "\n" +
                     "Rotation: " + RocketController.rocketRigidBody.Rotation+ "(rad)\n" +
                     "Angular Velocity: " + RocketController.rocketRigidBody.AngularVecloity + "(rad/s)\n";
                     
        _text.text = msg;
    }
}
