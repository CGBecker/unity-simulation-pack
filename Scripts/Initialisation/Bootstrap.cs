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

    private Dictionary<string, List<string>> _labeledKeys;

    void Start()
    {
        LoadAndListLabels();

        foreach (string label in _labeledKeys.Keys)
        {
            ListAddressablesByLabel(label);
        }
    }

    private void ListAddressablesByLabel(string label)
    {
        Addressables.LoadResourceLocationsAsync(label).Completed += OnResourceLocationsLoaded;
    }

    private void OnResourceLocationsLoaded(AsyncOperationHandle<IList<IResourceLocation>> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            IList<IResourceLocation> locations = handle.Result;
            Debug.Log($"Found {locations.Count} addressables per label");

            foreach (IResourceLocation location in locations)
            {
                Debug.Log($"Addressable: {location.PrimaryKey}");
            }

            foreach (string label in _labeledKeys.Keys)
            {
                _labeledKeys[label] = locations.Select(location => location.PrimaryKey).ToList();
            }
        }
        else
        {
            Debug.LogError("Failed to load resource locations.");
        }
        Addressables.Release(handle);
    }

    private void LoadAndListLabels()
    {
        Addressables.LoadAssetAsync<LabelsList>("Assets/Configs/LabelsList.asset").Completed += OnLabelsLoaded;

        // continue loading from here?
    }

    private void OnLabelsLoaded(AsyncOperationHandle<LabelsList> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            LabelsList labelData = handle.Result;

            Debug.Log("Addressable Labels:");
            foreach (string label in labelData.labels)
            {
                _labeledKeys.Add(label, null);
            }
        }
        else
        {
            Debug.LogError("Failed to load label data.");
        }
        Addressables.Release(handle);
    }
}