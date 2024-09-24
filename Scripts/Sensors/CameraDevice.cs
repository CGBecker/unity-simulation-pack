using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using Unity.Mathematics;
using Unity.Burst;

public class CameraDevice : BaseSensor
{
    // Create camera component
    public Transform[] CamerasTransforms;
    private Camera[] cameras;
    private HDAdditionalCameraData[] camerasDatas;

#region Camera Settings
    public float2 CameraResolution;
    public uint FPS;
    public float FieldOfView;
    public float2 ClippingPlane;
    public float2 SensorSize;
    public uint ISO;
    public float ShutterSpeed;
    public Camera.GateFitMode GateFitMode;
    public float FocalLength;
    public float2 Shift;
    public float Aperture;
    public float FocusDistance;
    public uint ApertureBladeCount;
    public float2 ApertureCurvature;
    public float ApertureBarrelClipping;
    public float ApertureAnamorphism;
    public HDAdditionalCameraData.AntialiasingMode PostAntiAliasing;
    public bool StopNaNs;
    public bool Dithering;
    public CameraSettings.Culling Culling;
    public Transform ExposureTargetTransform;
    public CameraClearFlags BackgroundType;
#endregion

#region Volume Settings
    private Volume volume;
    private VolumeProfile volumeProfile;
    public ExposureMode ExposureModeSettings;
    public Bloom BloomSettings;
    public ColorAdjustments ColorAdjustmentsSettings;
    public FilmGrain FilmGrainSettings;
    public MotionBlur MotionBlurSettings;
    public Tonemapping TonemappingSettings;
    public Vignette VignetteSettings;
#endregion

    // initialise and configure camera component from given settings
    // - Camera resolution
    // - FPS
    // - Field of View
    // - Clipping Planes (near and far)
    // - Sensor size
    // - ISO
    // - Shutter Speed
    // - Gate Fit
    // - Focal Length
    // - Shift
    // - Aperture
    // - Focus Distance
    // - Aperture shape
    //  |_ Blade Count
    //  |_ Curvature
    //  |_ Barrel Clipping
    //  |_ Anamorphism
    // - Post Anti-Alias
    // - Stop NaNs
    // - Dithering
    // - Culling Mask
    // - Exposure Target Object
    // - Background type (Sky, colour,...)
    // - Exposure (Auto, Manual,...)
    // - Bloom
    // - Color Adjustments
    // - Film Grain
    // - Motion Blur
    // - Tonemapping
    // - Vignette

    public override void ConfigureDevice()
    {
        throw new System.NotImplementedException();
    }
    public override void InitialiseSensor()
    {
        throw new System.NotImplementedException();
    }

    // Command for custom render if needed
    public override T TakeReading<T>()
    {
        throw new System.NotImplementedException();
    }
    public override T TakeReading<T>(uint index)
    {
        throw new System.NotImplementedException();
    }
}
