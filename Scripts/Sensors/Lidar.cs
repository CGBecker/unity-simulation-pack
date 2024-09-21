using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Unity.Jobs;
using Unity.Burst;

public class Lidar : BaseSensor
{
    // Array of lidars
    public Transform[] raycastOrigins;  // Points from where the rays will be cast
    public float[] raycastDistances;  // Max distance of raycasts
    public Vector3[] raycastDirections;  // Target directions for raycasts
    public string layerName;  // layer name to conver to mask
    private LayerMask raycastLayerMask;  // Which layers to raycast against
    private QueryParameters queryParameters;
    private NativeArray<RaycastCommand> raycastCommands;
    private NativeArray<RaycastHit> raycastResults;
    private JobHandle raycastHandle;

    private float[] results;

    /// <summary>
    /// Initialise lidar using raycast command for parallel with settings received
    /// WARNING: Parameters of device must be initialised before calling this
    /// </summary>
    public override void InitialiseSensor()
    {
        raycastCommands = new NativeArray<RaycastCommand>(raycastOrigins.Length, Allocator.Persistent);
        raycastResults = new NativeArray<RaycastHit>(raycastOrigins.Length, Allocator.Persistent);
        results = new float[raycastOrigins.Length];
        raycastDistances = new float[raycastOrigins.Length];
        raycastDirections = new Vector3[raycastOrigins.Length];

        int layerIndex = LayerMask.NameToLayer(layerName);
        if (layerIndex != -1) // Make sure the layer exists
        {
            raycastLayerMask = 1 << layerIndex;
            Debug.Log("Layer Mask: " + raycastLayerMask.value);
        }
        else
        {
            Debug.LogError("Layer does not exist!");
        }
        queryParameters = new QueryParameters(raycastLayerMask, false, QueryTriggerInteraction.Ignore, false);  // Hit only layerMask, do NOT hit multiple faces, ignore triggers and do NOT hit backfaces
    }

    // command method for receiving commands (On-demand?)

    // take reading method to send data back
    public override T TakeReading<T>()
    {
        ScheduleRaycasts();

        return (T)(object)results;
    }
    public override T TakeReading<T>(uint index)
    {
        throw new System.NotImplementedException();
    }

    [BurstCompile]
    private void ScheduleRaycasts()
    {
        for (int i = 0; i < raycastOrigins.Length; i++)
        {
            Vector3 origin = raycastOrigins[i].parent.TransformPoint(raycastOrigins[i].localPosition);
            Vector3 direction = raycastDirections[i];
            raycastCommands[i] = new RaycastCommand(origin, direction, queryParameters);
        }

        raycastHandle = RaycastCommand.ScheduleBatch(raycastCommands, raycastResults, 1);
        raycastHandle.Complete();

        for (int i = 0; i < raycastOrigins.Length; i++)
        {
            if (raycastResults[i].collider != null)
            {
                results[i] = Vector3.Distance(raycastOrigins[i].localPosition, raycastResults[i].point);
            }
            else
            {
                results[i] = raycastDistances[i];
            }
        }
    }

    private void OnDestroy()
    {
        if (raycastCommands.IsCreated) raycastCommands.Dispose();
        if (raycastResults.IsCreated) raycastResults.Dispose();
    }
}
