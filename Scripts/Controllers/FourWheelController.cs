using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FourWheelController : MonoBehaviour
{
    public ArticulationBody RootArticulationBody;
    public ArticulationBody[] WheelsMotors;

    public ArticulationBody[] WheelsSuspensions;

    public ArticulationBody[] WheelsSteering;
    public float MaxSteeringForce;
    private float steeringForce;

    public ArticulationBody[] WheelsWithBrakes;



    void Start() 
    {
        for (int wheelsSteeringIndex = 0; wheelsSteeringIndex < WheelsSteering.Length; wheelsSteeringIndex++)
            {
                WheelsSteering[wheelsSteeringIndex].SetDriveForceLimit(ArticulationDriveAxis.Y, MaxSteeringForce);
                Debug.Log("Setting maximum steering force: " + WheelsSteering[wheelsSteeringIndex].transform.name
                            + " to " + MaxSteeringForce);
            }
    }
    
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.W))
        {
            for (int motorsIndex = 0; motorsIndex < WheelsMotors.Length; motorsIndex++)
            {
                WheelsMotors[motorsIndex].AddRelativeTorque(Vector3.right * 200);
                Debug.Log("Trying to add torque to: " + WheelsMotors[motorsIndex].transform.name);
            }
        }

        if (Input.GetKey(KeyCode.S))
        {
            for (int brakesIndex = 0; brakesIndex < WheelsWithBrakes.Length; brakesIndex++)
            {
                WheelsWithBrakes[brakesIndex].AddRelativeTorque(Vector3.left * 100);
                Debug.Log("Trying to brake: " + WheelsWithBrakes[brakesIndex].transform.name);
            }
        }

        if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            steeringForce = -100f;
        }
        else if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
        {
            steeringForce = 100f;
        }
        else
        {
            steeringForce = 0f;
        }

        if (steeringForce != 0f)
        {
            for (int wheelsSteeringIndex = 0; wheelsSteeringIndex < WheelsSteering.Length; wheelsSteeringIndex++)
            {
                WheelsSteering[wheelsSteeringIndex].AddRelativeTorque(new Vector3(0, 0, steeringForce));
                Debug.Log("Trying to add steer: " + WheelsSteering[wheelsSteeringIndex].transform.name);
            }
        }
    }
}
