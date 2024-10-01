using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Unity.Mathematics;
using Unity.Burst;

public class BaseMotor : BaseActuator
{
    // Set array of motors to be controlled and reprogrammed

    // initialise array of motors with anticulation body type and settings

    // Command method to receive commands in torque or friction to initialise tasks
    // Must be able to stop previous tasks

    public override void InitialiseDevice()
    {
        throw new System.NotImplementedException();
    }

    public override void ConfigureDevice()
    {
        throw new System.NotImplementedException();
    }

    public override bool Command<T>(T targetTorque)
    {
        throw new System.NotImplementedException();
    }

    public override float[] TakeReading()
    {
        throw new System.NotImplementedException();
    }

    public override float TakeReading(uint index)
    {
        throw new System.NotImplementedException();
    }

    // Tasks to perform actions
}
