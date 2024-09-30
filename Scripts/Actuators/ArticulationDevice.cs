using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArticulationDevice : BaseActuator
{
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
}
