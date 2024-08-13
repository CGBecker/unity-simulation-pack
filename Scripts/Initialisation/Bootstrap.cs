using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using System.Linq;
using System.Threading.Tasks;

public class Bootstrap : MonoBehaviour
{
    // initialise based on target environment and target vehicle

    // environment screen selection (Menu)

    // use addressables for loading

    // read urdf or XML description for building vehicle

    /// <summary>
    /// Dictionary to store the addressables' address/keys separated per label
    /// </summary>
    private Dictionary<string, List<string>> _labeledKeys = new Dictionary<string, List<string>>();

    /// <summary>
    /// Starting bootstrap behaviour
    /// </summary>
    void Start()
    {
        uint maxDelta = 40;  // Default maxDelta
        uint physicsStep = 4;
        string[] args = System.Environment.GetCommandLineArgs();

        for (int i = 0; i < args.Length; i+=2)
        {
           string argument = args[i];

            if (argument == "--MaxDelta" || argument == "-md")
            {
                bool success = uint.TryParse(args[i+1], out maxDelta);
                if (!success)
                {
                    Debug.LogError("Invalid MaxDelta value from argument!");
                }
            }

            if (argument == "--PhysicsStep" || argument == "-ps")
            {
                bool success = uint.TryParse(args[i+1], out physicsStep);
                if (!success)
                {
                    Debug.LogError("Invalid PhysicsStep value from argument!");
                }
            }
        }

         if (SimTimeManager.Instance == null)
        {
            GameObject singletonObject = new GameObject("GameManager");
            singletonObject.AddComponent<SimTimeManager>();
        }
        SimTimeManager.Instance.InitializeSettings(maxDelta, physicsStep);

        InitialiseAddressablesLoading();
    }

    /// <summary>
    /// Start per-label addressables loading/listing
    /// </summary>
    /// <param name="label"></param>
    private void ListAddressablesByLabel(string label)
    {
        Addressables.LoadResourceLocationsAsync(label).Completed += OnResourceLocationsLoaded;
    }

    /// <summary>
    /// Actual listing of addressables' addresses per label
    /// </summary>
    /// <param name="handle"></param>
    private void OnResourceLocationsLoaded(AsyncOperationHandle<IList<IResourceLocation>> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            IList<IResourceLocation> locations = handle.Result;
            LoggingSingleton.Logging.Log($"Found {locations.Count} addressables per label");

            foreach (IResourceLocation location in locations)
            {
                LoggingSingleton.Logging.Log($"Addressable: {location.PrimaryKey}");
            }

            List<string> keys = _labeledKeys.Keys.ToList();
            foreach (string label in keys)
            {
                _labeledKeys[label] = locations.Select(location => location.PrimaryKey).ToList();
            }
        }
        else
        {
            LoggingSingleton.Logging.LogError("Failed to load resource locations.");
        }
        Addressables.Release(handle);
    }

    /// <summary>
    /// Start addressables loading, beginning with LabelsList.asset Scriptable Object
    /// then proceeding to load labels listed in said Scriptable Object
    /// </summary>
    private void InitialiseAddressablesLoading()
    {
        Addressables.LoadAssetAsync<LabelsList>("Assets/Configs/LabelsList.asset").Completed += OnLabelsLoaded;
    }

    /// <summary>
    /// Loading labels list from Scriptable Object in order to start loading of addressables
    /// separated per label
    /// </summary>
    /// <param name="handle"></param>
    private void OnLabelsLoaded(AsyncOperationHandle<LabelsList> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            LabelsList labelData = handle.Result;

            LoggingSingleton.Logging.Log("Addressable Labels:");
            foreach (string label in labelData.labels)
            {
                _labeledKeys.Add(label, null);
            }
            foreach (string label in _labeledKeys.Keys)
            {
                ListAddressablesByLabel(label);
            }
        }
        else
        {
            LoggingSingleton.Logging.LogError("Failed to load label data.");
        }
        Addressables.Release(handle);
    }
}