using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArticulationDevice : BaseActuator
{
    /// <summary>
    /// The main parent of ALL articulation bodies for this Robot/Vehicle
    /// </summary>
    public GameObject AbsoluteParent;
    public GameObject[] childrenArticulationBodies;
    private ArticulationBody[] articulationBodies;
    public float[] MassPerArticulationBody;
    public bool[] UseGravity;
    public bool[] AutomaticCenterOfGravity;
    public bool[] AutomaticTensor;
    public bool[] Immovable;
    public float[] LinearDamping;
    public float[] AngularDamping;
    /// <summary>
    /// 1 - Discrete;
    /// 2 - Continuous;
    /// 3 - Continuous Dynamic;
    /// 4 - Continuous Speculative;
    /// </summary>
    public uint[] CollisionDetection;
    public bool[] MatchAnchors;
    public Vector3[] AnchorPosition;
    public Vector3[] AnchorRotation;
    public ArticulationJointType[] ArticulationJointTypes;
    /// <summary>
    /// 1 - X;
    /// 2 - Y;
    /// 3 - Z;
    /// </summary>
    public uint[] AxisIfPrismatic;
    /// <summary>
    /// 1 - Free;
    /// 2 - Limited;
    /// </summary>
    public uint[] MotionIfPrismaticOrRevolut;
    public uint[] SwingYIfSpherical;
    public uint[] SwingZIfSpherical;
    public uint[] TwistIfSpherical;
    public Vector3[] DriveStiffness;
    public Vector3[] DriveDamping;
    public Vector3[] DriveForceLimit;
    public Vector3[] DriveTarget;
    public Vector3[] DriveTargetVelocity;
    public ArticulationDriveType[] DriveTypes;


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
}
