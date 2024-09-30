using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

public class Bump : BaseSensor
{
    public BoxCollider[] BoxColliders;
    public Vector3[] BoxSizes;
    public Vector3[] BoxCenterPose;

    public SphereCollider[] SphereColliders;
    public Vector3[] SphereCenterPose;
    public float[] SphereRadius;

    public CapsuleCollider[] CapsuleColliders;
    public Vector3[] CapsuleCenterPose;
    public float[] CapsuleRadius;
    public float[] CapsuleHeight;
    /// <summary>
    /// Direction represented in int (0 = X, 1 = Y, 2 = Z)
    /// </summary>
    public int[] CapsuleDirection;

    /// <summary>
    /// Mesh colliders are computationally intensive and may cause instability if too dense or colliding with other Mesh colliders.
    /// Ideally should be made convex, which becomes mandatory when using articulation bodies instead of rigibodies
    /// </summary>
    public MeshCollider[] MeshColliders;
    public bool[] IsConvex;
    public Mesh[] MeshesForCollider;

    public bool[] IsTrigger;
    public PhysicMaterial[] physicMaterials;
    private PhysicMaterial _defaultMaterial = new PhysicMaterial();

    private uint _collidersCount;
    private bool[] _contacts;
    private DeviceData<bool>[] _bumpData;

    public override void ConfigureDevice()
    {
        _collidersCount = (uint)(BoxColliders.Length+SphereColliders.Length+CapsuleColliders.Length+MeshColliders.Length);
        _contacts = new bool[_collidersCount];
        _bumpData = new DeviceData<bool>[_collidersCount];

        for (uint i = 0; i < _collidersCount; i++)
        {
            if (physicMaterials[i] == null)
            {
                physicMaterials[i] = _defaultMaterial;
            }
            if (i < BoxColliders.Length)
            {
                BoxColliders[i].size = BoxSizes[i];
                BoxColliders[i].center = BoxCenterPose[i];
                BoxColliders[i].isTrigger = IsTrigger[i];
                BoxColliders[i].material = physicMaterials[i];
            }
            else if (i < (BoxColliders.Length+SphereColliders.Length))
            {
                uint u = i-(uint)BoxColliders.Length;
                SphereColliders[u].center = SphereCenterPose[u];
                SphereColliders[u].radius = SphereRadius[u];
                BoxColliders[i].isTrigger = IsTrigger[i];
                SphereColliders[u].material = physicMaterials[i];
            }
            else if (i < (BoxColliders.Length+SphereColliders.Length+CapsuleColliders.Length))
            {
                uint y = i-(uint)BoxColliders.Length-(uint)SphereColliders.Length;
                CapsuleColliders[y].center = CapsuleCenterPose[y];
                CapsuleColliders[y].radius = CapsuleRadius[y];
                CapsuleColliders[y].height = CapsuleHeight[y];
                CapsuleColliders[y].isTrigger = IsTrigger[i];
                CapsuleColliders[y].material = physicMaterials[i];
            }
            else
            {
                uint t = i-(uint)BoxColliders.Length-(uint)SphereColliders.Length-(uint)CapsuleColliders.Length;
                MeshColliders[t].convex = IsConvex[t];
                MeshColliders[t].sharedMesh = MeshesForCollider[t];
                MeshColliders[t].isTrigger = IsTrigger[i];
                MeshColliders[t].material = physicMaterials[i];
            }
        }
    }
    public override void InitialiseSensor()
    {
        throw new System.NotImplementedException();
    }

    public override T TakeReading<T>()
    {
        for (uint i = 0; i < _collidersCount; i++)
        {
            _bumpData[i].Time = "XXXXXX";    // Would the time be correct here since the reading does not take place here?
            _bumpData[i].Data = _contacts[i];
            _contacts[i] = false;
        }
        return (T)(object)_bumpData;
    }
    public override T TakeReading<T>(uint index)
    {
        _bumpData[index].Time = "XXXXXX";  // Would the time be correct here since the reading does not take place here?
        _bumpData[index].Data = _contacts[index];
        _contacts[index] = false;
        return (T)(object)_bumpData[index];
    }

    private void OnTriggerEnter(Collider other)
    {
        for (uint i = 0; i < _collidersCount; i++)
        {
            if (i < BoxColliders.Length)
            {
                if (other == BoxColliders[i])
                {
                    _contacts[i] = true;
                }
            }
            else if (i < (BoxColliders.Length+SphereColliders.Length))
            {
                if (other == SphereColliders[i-(uint)BoxColliders.Length])
                {
                    _contacts[i] = true;
                }
            }
            else if (i < (BoxColliders.Length+SphereColliders.Length+CapsuleColliders.Length))
            {
                if (other == CapsuleColliders[i-(uint)BoxColliders.Length-(uint)SphereColliders.Length])
                {
                    _contacts[i] = true;
                }
            }
            else
            {
                if (other == MeshColliders[i-(uint)BoxColliders.Length-(uint)SphereColliders.Length-(uint)CapsuleColliders.Length])
                {
                    _contacts[i] = true;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        for (uint i = 0; i < _collidersCount; i++)
        {
            if (i < BoxColliders.Length)
            {
                if (other == BoxColliders[i])
                {
                    _contacts[i] = true;
                }
            }
            else if (i < (BoxColliders.Length+SphereColliders.Length))
            {
                if (other == SphereColliders[i-(uint)BoxColliders.Length])
                {
                    _contacts[i] = true;
                }
            }
            else if (i < (BoxColliders.Length+SphereColliders.Length+CapsuleColliders.Length))
            {
                if (other == CapsuleColliders[i-(uint)BoxColliders.Length-(uint)SphereColliders.Length])
                {
                    _contacts[i] = true;
                }
            }
            else
            {
                if (other == MeshColliders[i-(uint)BoxColliders.Length-(uint)SphereColliders.Length-(uint)CapsuleColliders.Length])
                {
                    _contacts[i] = true;
                }
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        for (uint i = 0; i < _collidersCount; i++)
        {
            if (i < BoxColliders.Length)
            {
                if (other.collider == BoxColliders[i])
                {
                    _contacts[i] = true;
                }
            }
            else if (i < (BoxColliders.Length+SphereColliders.Length))
            {
                if (other.collider == SphereColliders[i-(uint)BoxColliders.Length])
                {
                    _contacts[i] = true;
                }
            }
            else if (i < (BoxColliders.Length+SphereColliders.Length+CapsuleColliders.Length))
            {
                if (other.collider == CapsuleColliders[i-(uint)BoxColliders.Length-(uint)SphereColliders.Length])
                {
                    _contacts[i] = true;
                }
            }
            else
            {
                if (other.collider == MeshColliders[i-(uint)BoxColliders.Length-(uint)SphereColliders.Length-(uint)CapsuleColliders.Length])
                {
                    _contacts[i] = true;
                }
            }
        }
    }
}
