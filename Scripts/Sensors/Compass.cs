using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compass : BaseSensor
{
    // Take direction of North as input, if not assume X as North
    public override void InitialiseSensor()
    {
        throw new System.NotImplementedException();
    }

    public override T TakeReading<T>()
    {
        throw new System.NotImplementedException();
    }
    public override T TakeReading<T>(uint index)
    {
        throw new System.NotImplementedException();
    }
}
