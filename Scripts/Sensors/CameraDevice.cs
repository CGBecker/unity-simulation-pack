using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class CameraDevice : BaseSensor
{
    // Create camera component
    public Transform[] CamerasTransforms;
    private Camera[] cameras;
    private HDAdditionalCameraData[] camerasDatas;

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
