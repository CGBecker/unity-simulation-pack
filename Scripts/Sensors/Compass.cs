using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compass : BaseSensor
{
    // Take direction of North as input, if not assume X as North

    public Vector3 NorthDirection;
    public Transform[] PointerNeedles;
    private Transform[] _parents;
    private bool _usePointers = false;
    private float _rotationSpeed = 1f;

    /// <summary>
    /// Initialises Compass sensor
    /// OBS.: If no Pointer needles have been provided, it only returns sensor data instead of pointing North
    /// OBS2.: If no North direction has been provided, assumes (1, 0, 0) as North
    /// </summary>
    public override void InitialiseSensor()
    {
        if (NorthDirection == null)
        {
            Debug.LogWarning("North Direction not specified, assuming (1, 0, 0)");
            NorthDirection = new Vector3(1, 0, 0);
        }

        if (PointerNeedles == null)
        {
            Debug.LogWarning("Pointer Needles not assigned, using data reading only!");
        }
        else
        {
            _usePointers = true;
            _parents = new Transform[PointerNeedles.Length];
            for (int i = 0; i < PointerNeedles.Length; i++)
            {
                _parents[i] = PointerNeedles[i].parent;
            }
        }
    }

    private void FixedUpdate() 
    {
        if (_usePointers)
        {
            PointNeedlesNorth();
        }
        else
        {
            return;
        }
    }

    private void PointNeedlesNorth()
    {
        Vector3 normalizedDirection = NorthDirection.normalized;
        Quaternion targetRotation = Quaternion.LookRotation(normalizedDirection);
        for (int i = 0; i < PointerNeedles.Length; i++)
        {
            PointerNeedles[i].rotation = Quaternion.Lerp(PointerNeedles[i].rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Takes the reading in Vector4 of the degrees from each direction for each pointer needle:
    /// Returning an Array of Vector4(DegreesToNorth, DegreesToEast, DegreesToWest, DegreesToSouth)
    /// Depending on how many Pointer needles were created
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>Array of Vector4(DegreesToNorth, DegreesToEast, DegreesToWest, DegreesToSouth)</returns>
    public override T TakeReading<T>()
    {
        Vector3 normalizedNorthDirection = NorthDirection.normalized;
        Vector3 normalizedEastDirection = new Vector3(normalizedNorthDirection.x+90f,normalizedNorthDirection.y,normalizedNorthDirection.z);
        Vector3 normalizedWestDirection = new Vector3(normalizedNorthDirection.x-90f,normalizedNorthDirection.y,normalizedNorthDirection.z);
        Vector3 normalizedSouthDirection = new Vector3(normalizedNorthDirection.x+180f,normalizedNorthDirection.y,normalizedNorthDirection.z);

        Vector4[] degreesToPoles = new Vector4[PointerNeedles.Length];
        for (int i = 0; i < PointerNeedles.Length; i++)
        {
            Vector3 currentForwardDirection = _parents[i].forward;
            degreesToPoles[i] = new Vector4(Vector3.Angle(currentForwardDirection, normalizedNorthDirection),  // North
                                            Vector3.Angle(currentForwardDirection, normalizedEastDirection),   // East
                                            Vector3.Angle(currentForwardDirection, normalizedWestDirection),   // West
                                            Vector3.Angle(currentForwardDirection, normalizedSouthDirection)); // South
        }
        return (T)(object)degreesToPoles;
    }

    /// <summary>
    /// Takes the reading in Vector4 of the degrees from each direction:
    /// Vector4(DegreesToNorth, DegreesToEast, DegreesToWest, DegreesToSouth)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="index"></param>
    /// <returns>Vector4(DegreesToNorth, DegreesToEast, DegreesToWest, DegreesToSouth)</returns>
    public override T TakeReading<T>(uint index)
    {
        Vector3 normalizedNorthDirection = NorthDirection.normalized;
        Vector3 normalizedEastDirection = new Vector3(normalizedNorthDirection.x+90f,normalizedNorthDirection.y,normalizedNorthDirection.z);
        Vector3 normalizedWestDirection = new Vector3(normalizedNorthDirection.x-90f,normalizedNorthDirection.y,normalizedNorthDirection.z);
        Vector3 normalizedSouthDirection = new Vector3(normalizedNorthDirection.x+180f,normalizedNorthDirection.y,normalizedNorthDirection.z);

        Vector3 currentForwardDirection = _parents[index].forward;
        Vector4 degreesToPoles = new Vector4(Vector3.Angle(currentForwardDirection, normalizedNorthDirection),  // North
                                        Vector3.Angle(currentForwardDirection, normalizedEastDirection),   // East
                                        Vector3.Angle(currentForwardDirection, normalizedWestDirection),   // West
                                        Vector3.Angle(currentForwardDirection, normalizedSouthDirection)); // South
        return (T)(object)degreesToPoles;
    }
}
