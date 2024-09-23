using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSensor : MonoBehaviour
{
    public abstract void InitialiseSensor();

    public abstract void ConfigureDevice();
    public abstract T TakeReading<T>();
    public abstract T TakeReading<T>(uint index);
}
