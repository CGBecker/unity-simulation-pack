using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Mathematics;
using UnityEngine;

public class ArticulationDevice : BaseActuator
{
    /// <summary>
    /// The main parent of ALL articulation bodies for this Robot/Vehicle
    /// </summary>
    public GameObject AbsoluteParent;
    public GameObject[] ChildrenArticulationBodies;
    private ArticulationBody[] _articulationBodies;
    public float[] MassPerArticulationBody;
    public bool[] UseGravity;
    public bool[] AutomaticCenterOfGravity;
    public bool[] AutomaticTensor;
    public bool[] Immovable;
    public float[] LinearDamping;
    public float[] AngularDamping;
    public CollisionDetectionMode[] CollisionDetectionModes;
    public bool[] MatchAnchors;
    public Vector3[] AnchorPosition;
    public Quaternion[] AnchorRotation;
    public ArticulationJointType[] ArticulationJointTypes;
    public ArticulationDriveAxis[] ArticulationDrivesAxis;
    private ArticulationDrive[] _articulationDrives;
    private Dictionary<uint2, ArticulationDrive> _articulationDriveDicts;  // Attetmpting to use the dict for indexing the Spherical Articulation Drives for memory reasons (uint(indexOfArty, driveAxis), Arty)
    /// <summary>
    /// OBS.: Not all Articulation Body types can have Locked Motion
    /// </summary>
    public ArticulationDofLock[] MotionIfPrismaticOrRevolut;
    public float2[] LowerAndUpperLimitsIfPrismatic;
    public ArticulationDofLock[] SwingYIfSpherical;
    public ArticulationDofLock[] SwingZIfSpherical;
    public ArticulationDofLock[] TwistIfSpherical;
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
        // create new articulation bodies in array and assign their individual parents

        // set type for all articulation bodies

        // assign all settings if they are not null

        _articulationBodies = new ArticulationBody[ChildrenArticulationBodies.Length];
        _articulationDrives = new ArticulationDrive[ChildrenArticulationBodies.Length];

        for (int i = 0; i < ChildrenArticulationBodies.Length; i++)
        {
            _articulationBodies[i] = ChildrenArticulationBodies[i].AddComponent<ArticulationBody>();
            _articulationBodies[i].mass = MassPerArticulationBody[i];
            _articulationBodies[i].useGravity = UseGravity[i];
            _articulationBodies[i].automaticCenterOfMass = AutomaticCenterOfGravity[i];
            _articulationBodies[i].automaticInertiaTensor = AutomaticTensor[i];
            _articulationBodies[i].immovable = Immovable[i];
            _articulationBodies[i].linearDamping = LinearDamping[i];
            _articulationBodies[i].angularDamping = AngularDamping[i];
            _articulationBodies[i].collisionDetectionMode = CollisionDetectionModes[i];
            _articulationBodies[i].matchAnchors = MatchAnchors[i];
            _articulationBodies[i].anchorPosition = AnchorPosition[i];
            _articulationBodies[i].anchorRotation = AnchorRotation[i];
            _articulationBodies[i].jointType = ArticulationJointTypes[i];

            switch (ArticulationJointTypes[i])
            {
                case ArticulationJointType.PrismaticJoint:
                {
                    switch (ArticulationDrivesAxis[i])
                    {
                        case ArticulationDriveAxis.X:
                        {
                            _articulationDrives[i] = _articulationBodies[i].xDrive;
                            _articulationBodies[i].linearLockX = MotionIfPrismaticOrRevolut[i];
                            _articulationDrives[i].stiffness = DriveStiffness[i].x;
                            _articulationDrives[i].damping = DriveDamping[i].x;
                            _articulationDrives[i].forceLimit = DriveForceLimit[i].x;
                            _articulationDrives[i].target = DriveTarget[i].x;
                            _articulationDrives[i].targetVelocity = DriveTargetVelocity[i].x;
                            break;
                        }
                        case ArticulationDriveAxis.Y:
                        {
                            _articulationDrives[i] = _articulationBodies[i].yDrive;
                            _articulationBodies[i].linearLockY = MotionIfPrismaticOrRevolut[i];
                            _articulationDrives[i].stiffness = DriveStiffness[i].x;
                            _articulationDrives[i].damping = DriveDamping[i].x;
                            _articulationDrives[i].forceLimit = DriveForceLimit[i].x;
                            _articulationDrives[i].target = DriveTarget[i].x;
                            _articulationDrives[i].targetVelocity = DriveTargetVelocity[i].x;
                            break;
                        }
                        case ArticulationDriveAxis.Z:
                        {
                            _articulationDrives[i] = _articulationBodies[i].zDrive;
                            _articulationBodies[i].linearLockZ = MotionIfPrismaticOrRevolut[i];
                            _articulationDrives[i].stiffness = DriveStiffness[i].x;
                            _articulationDrives[i].damping = DriveDamping[i].x;
                            _articulationDrives[i].forceLimit = DriveForceLimit[i].x;
                            _articulationDrives[i].target = DriveTarget[i].x;
                            _articulationDrives[i].targetVelocity = DriveTargetVelocity[i].x;
                            break;
                        }
                        default:
                        {
                            Debug.LogError("Unrecognised Axis for Articulation Drive: " + ArticulationDrivesAxis[i]);
                            break;
                        }
                    }
                    _articulationDrives[i].lowerLimit = LowerAndUpperLimitsIfPrismatic[i].x;
                    _articulationDrives[i].upperLimit = LowerAndUpperLimitsIfPrismatic[i].y;
                    _articulationDrives[i].driveType = DriveTypes[i];
                    break;
                }
                case ArticulationJointType.RevoluteJoint:
                {
                    _articulationDrives[i] = _articulationBodies[i].xDrive;
                    _articulationBodies[i].linearLockX = MotionIfPrismaticOrRevolut[i];
                    _articulationDrives[i].driveType = DriveTypes[i];
                    break;
                }
                case ArticulationJointType.SphericalJoint:
                {
                    _articulationBodies[i].swingYLock = SwingYIfSpherical[i];
                    _articulationBodies[i].swingZLock = SwingZIfSpherical[i];
                    _articulationBodies[i].twistLock = TwistIfSpherical[i];
                    // _articulationBodies[i].xDrive.driveType = DriveTypes[i];
                    break;
                }
                default:
                {
                    break;
                }
            }

        }
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
