using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using Unity.Mathematics;
using Unity.Burst;

public class CameraDevice : BaseSensor
{
    /// <summary>
    /// Gameobjects for where the cameras are to be either added or have already been added
    /// OBS.: Gameobjects MUST be assigned for the device to work but the Camera components need not be pre-added, but can
    /// </summary>
    public GameObject[] CamerasGameobjects;  // Gameobjects of either the camera or objects to add camera component
    private Camera[] cameras;  // Cameras to be assigned or created
    private HDAdditionalCameraData[] camerasDatas;  // cameras data components for controlling extra settings
    private DeviceData<RenderTexture>[] deviceDatas;  // Data per camera

#region Camera Settings
    public int2 CameraResolution;
    public int CameraRenderBitDepth;
    public uint FPS;  // TODO: To be improved alongside TimeManagement work
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
    // public CameraSettings.Culling Culling;  // TODO: find better way of setting layer mask for culling
    public GameObject ExposureTarget;
    public CameraClearFlags BackgroundType;
#endregion

#region Volume Settings
    private Volume[] volumes;
    private VolumeProfile[] volumeProfiles;
    private Exposure[] exposures;
    public ExposureModeParameter ExposureModeSettings;
    public Bloom BloomSettings;
    public ColorAdjustments ColorAdjustmentsSettings;
    public FilmGrain FilmGrainSettings;
    public MotionBlur MotionBlurSettings;
    public Tonemapping TonemappingSettings;
    public Vignette VignetteSettings;
#endregion

    public override void ConfigureDevice()
    {
        if (CamerasGameobjects == null)
        {
            Debug.LogError("ERROR: No camera Transform assigned to Camera Device!");
            return;
        }

        ValidateInput();

        AssignOrCreateCameras();

        SetGlobalSettings();

        SetCameraSettings();

        CreateAndSetVolumes();
    }
    public override void InitialiseSensor()
    {
        // enable gameobject/component
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

    private RenderTexture GetRender(uint index)
    {
        return new RenderTexture(CameraResolution.x, CameraResolution.y, CameraRenderBitDepth);
    }

    private void ValidateInput()
    {
        if (CameraResolution.x <= 0 || CameraResolution.y <= 0)
        {
            Debug.LogWarning("Broken value detected for Camera Device Resolution: " + CameraResolution + ". Setting default (512,512)");
            CameraResolution = new int2(512,512);
        }
        if (CameraRenderBitDepth != 0 && CameraRenderBitDepth != 16 && CameraRenderBitDepth != 24 && CameraRenderBitDepth != 32)
        {
            Debug.LogWarning("Broken value detected for Camera Device BitDepth: " + CameraRenderBitDepth + ". Setting default (24)");
            CameraRenderBitDepth = 24;
        }
        if (FPS <= 0)
        {
            Debug.LogWarning("Broken value detected for Camera Device FPS: " + FPS + ". Setting default (60)");
            FPS = 60;
        }
        if (FieldOfView < 1 && FieldOfView > 179)
        {
            Debug.LogWarning("Broken value detected for Camera Device FieldOfView: " + FieldOfView + ". Setting default (100)");
            FieldOfView = 100;
        }
        if (SensorSize.x < 0.1f || SensorSize.y < 0.1f)
        {
            Debug.LogWarning("Broken value detected for Camera Device SensorSize: " + SensorSize + ". Setting default (70,50)");
            SensorSize = new float2(70,50);
        }
        if (ISO < 1)
        {
            Debug.LogWarning("Broken value detected for Camera Device ISO: " + ISO + ". Setting default (200)");
            ISO = 200;
        }
        if (ShutterSpeed <= 0f)
        {
            Debug.LogWarning("Broken value detected for Camera Device ShutterSpeed: " + ShutterSpeed + ". Setting default (0.005)");
            ShutterSpeed = 0.005f;
        }
        if (FocalLength < 0.2296482f)
        {
            Debug.LogWarning("Broken value detected for Camera Device FocalLength: " + FocalLength + ". Setting default (20)");
        }
        if (Aperture < 0.7f || Aperture > 32f)
        {
            Debug.LogWarning("Broken value detected for Camera Device Aperture: " + Aperture + ". Setting default (16)");
            Aperture = 16f;
        }
        if (FocusDistance < 0.1f)
        {
            Debug.LogWarning("Broken value detected for Camera Device FocusDistance: " + FocusDistance + ". Setting default (10)");
            FocusDistance = 10f;
        }
        if (ApertureBladeCount < 3 || ApertureBladeCount > 11)
        {
            Debug.LogWarning("Broken value detected for Camera Device ApertureBladeCount: " + ApertureBladeCount + ". Setting default (5)");
            ApertureBladeCount = 5;
        }
        if (ApertureCurvature.x < 0.7f || ApertureCurvature.x > 32 || ApertureCurvature.y < 0.7f || ApertureCurvature.y > 32f)
        {
            Debug.LogWarning("Broken value detected for Camera Device ApertureCurvature: " + ApertureCurvature + ". Setting default (2,11)");
            ApertureCurvature = new float2(2,11);
        }
        if (ApertureBarrelClipping < 0f || ApertureBarrelClipping > 1f)
        {
            Debug.LogWarning("Broken value detected for Camera Device ApertureBarrelClipping: " + ApertureBarrelClipping + ". Setting default (0.25)");
            ApertureBarrelClipping = 0.25f;
        }
        if (ApertureAnamorphism < -1f || ApertureAnamorphism > 1f)
        {
            Debug.LogWarning("Broken value detected for Camera Device ApertureAnamorphism: " + ApertureAnamorphism + ". Setting default (0)");
            ApertureAnamorphism = 0f;
        }
    }

    private void AssignOrCreateCameras()
    {
        cameras = new Camera[CamerasGameobjects.Length];
        camerasDatas = new HDAdditionalCameraData[CamerasGameobjects.Length];
        for (int i = 0; i < CamerasGameobjects.Length; i++)
        {
            if (CamerasGameobjects[i].TryGetComponent<Camera>(out cameras[i]))
            {
                camerasDatas[i] = CamerasGameobjects[i].GetComponent<HDAdditionalCameraData>();
            }
            else
            {
                cameras[i] = CamerasGameobjects[i].AddComponent<Camera>();
                camerasDatas[i] = CamerasGameobjects[i].AddComponent<HDAdditionalCameraData>();
            }
            CamerasGameobjects[i].SetActive(false);
        }
    }

    private void SetGlobalSettings()
    {
        Application.targetFrameRate = (int)FPS;
        QualitySettings.vSyncCount = 0;  // Removing VSync so that the FPS is more guaranteed
    }

    private void SetCameraSettings()
    {
        for (int i = 0; i < CamerasGameobjects.Length; i++)
        {
            cameras[i].fieldOfView = FieldOfView;
            cameras[i].nearClipPlane = ClippingPlane.x;
            cameras[i].farClipPlane = ClippingPlane.y;
            cameras[i].sensorSize = SensorSize;
            cameras[i].iso = (int)ISO;
            cameras[i].shutterSpeed = ShutterSpeed;
            cameras[i].gateFit = GateFitMode;
            cameras[i].focalLength = FocalLength;
            cameras[i].lensShift = Shift;
            cameras[i].aperture = Aperture;
            cameras[i].focusDistance = FocusDistance;
            cameras[i].bladeCount = (int)ApertureBladeCount;
            cameras[i].curvature = ApertureCurvature;
            cameras[i].barrelClipping = ApertureBarrelClipping;
            cameras[i].anamorphism = ApertureAnamorphism;
            camerasDatas[i].antialiasing = PostAntiAliasing;
            camerasDatas[i].stopNaNs = StopNaNs;
            camerasDatas[i].dithering = Dithering;
            // cameras[i].cullingMask = 1 << 9;  // TODO: Find better way to do this
            if (ExposureTarget != null)
            {
                camerasDatas[i].exposureTarget = ExposureTarget;
            }
            cameras[i].clearFlags = BackgroundType;
        }
    }

    private void CreateAndSetVolumes()
    {
        volumes = new Volume[CamerasGameobjects.Length];
        volumeProfiles = new VolumeProfile[CamerasGameobjects.Length];
        if (ExposureModeSettings == ExposureMode.Fixed)
            {
                exposures = new Exposure[CamerasGameobjects.Length];
            }
        for (int i = 0; i < CamerasGameobjects.Length; i++)
        {
            GameObject tempVolumeGameObject = new GameObject("LocalVolume");
            tempVolumeGameObject.transform.SetParent(CamerasGameobjects[i].transform);
            tempVolumeGameObject.transform.localPosition = new Vector3(0, 0, 0);
            volumes[i] = tempVolumeGameObject.AddComponent<Volume>();
            volumeProfiles[i] = new VolumeProfile();
            volumes[i].sharedProfile = volumeProfiles[i];
            BoxCollider tempCollider = tempVolumeGameObject.AddComponent<BoxCollider>();
            tempCollider.size = new Vector3(0.005f,0.005f,0.005f);
            tempCollider.isTrigger = true;
            volumes[i].isGlobal = false;

            if (ExposureModeSettings == ExposureMode.Fixed)
            {
                exposures[i] = volumeProfiles[i].Add<Exposure>();
                exposures[i].mode = ExposureModeSettings;
            }
            else
            {
                Exposure tempExposure = volumeProfiles[i].Add<Exposure>();
                tempExposure.mode = ExposureModeSettings;
            }

            Bloom tempBloom = volumeProfiles[i].Add<Bloom>();
            tempBloom.active = BloomSettings.active;
            tempBloom.intensity = BloomSettings.intensity;
            tempBloom.threshold = BloomSettings.threshold;
            tempBloom.quality = BloomSettings.quality;
            tempBloom.scatter = BloomSettings.scatter;
            tempBloom.tint = BloomSettings.tint;
            if (BloomSettings.dirtTexture != null)
            {
                tempBloom.dirtTexture = BloomSettings.dirtTexture;
                tempBloom.dirtIntensity = BloomSettings.dirtIntensity;
            }

            ColorAdjustments tempColorAdjustments = volumeProfiles[i].Add<ColorAdjustments>();

            FilmGrain tmepFilmGrain = volumeProfiles[i].Add<FilmGrain>();

            MotionBlur tmepMotionBlur = volumeProfiles[i].Add<MotionBlur>();

            Tonemapping tempTonemapping = volumeProfiles[i].Add<Tonemapping>();

            Vignette tempVignette = volumeProfiles[i].Add<Vignette>();



        }
    }
}
