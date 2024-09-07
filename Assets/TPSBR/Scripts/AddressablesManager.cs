using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using System;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressablesManager : MonoBehaviour
{
    [SerializeField]
    private AssetReferenceGameObject[] _environments;

    [SerializeField]
    private Transform _parent;

    
    public bool _active = true;

    private int _totalEnvironments;
    private int _loadedEnvironments;
    
    public void StartLoading()
    {
        _totalEnvironments = _environments.Length;
        _loadedEnvironments = 0;


        

        if(_totalEnvironments>0 && _active){
            StartCoroutine(LoadEnvironments());
        }

        //AsyncOperationHandle<IList<GameObject>> loadOperation = Addressables.LoadAssetsAsync<GameObject>("Material", null);

        //loadOperation.Completed += OnLoadCompleted;
    }

    private void OnLoadCompleted(AsyncOperationHandle<IList<GameObject>> operation)
    {
        if (operation.Status == AsyncOperationStatus.Succeeded)
        {
            // Access and use the loaded assets.
            foreach (var loadedAsset in operation.Result)
            {
                Instantiate(loadedAsset); // For example, instantiate a GameObject.
            }
        }
        else
        {
            // Handle the error if the operation fails.
            Debug.LogError("Failed to load assets by label: " + operation.OperationException);
        }

        // Unload the operation when you're done to free up resources.
        Addressables.Release(operation);
    }

    public static Action<int> LoadingAssets;
    public static Action LoadingComplete;

    private IEnumerator LoadEnvironments()
    {
        Debug.Log("Loading addressables" );
        for (int i = 0; i < _totalEnvironments; i++)
        {
            AsyncOperationHandle<GameObject> handle = _environments[i].LoadAssetAsync<GameObject>();

            handle.Completed += LoadCompletedCallback;

            while (!handle.IsDone)
            {
                // Calculate the loading progress percentage
                float progress = (float)_loadedEnvironments / _totalEnvironments + handle.PercentComplete / _totalEnvironments;
                Debug.Log($"Loading progress: {progress * 100}%");
                LoadingAssets?.Invoke((int)(progress * 100));
                yield return null;
            }


            
            
            yield return null;
        }


    }

    private void LoadCompletedCallback(AsyncOperationHandle<GameObject> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject environment = handle.Result;
            GameObject environmentInstance = Instantiate(environment, _parent);
            environmentInstance.transform.localPosition = Vector3.zero;
            _loadedEnvironments++;

            if(_loadedEnvironments == _totalEnvironments){
                LoadingComplete?.Invoke();
                LoadingAssets?.Invoke(100);
            }
            Debug.Log($"Environment {_loadedEnvironments}/{_totalEnvironments} loaded");
        }
        else
        {
            Debug.LogError($"Failed to load environment: {handle.DebugName}");
        }
    }

    
    private void OnDestroy()
    {
        for (int i = 0; i < _totalEnvironments; i++)
        {
            _environments[i].ReleaseAsset();
        }
        
    }
}



