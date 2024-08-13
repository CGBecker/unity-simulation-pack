using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSensor : MonoBehaviour
{
    public abstract void InitialiseSensor();

    public abstract float[] TakeReading();
    public abstract float TakeReading(uint index);
}
