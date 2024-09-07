using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using System;
using UnityEngine.ResourceManagement.AsyncOperations;
using TMPro;


[System.Serializable]
public class AddresabbleGroup
{
    public string name;
    public AssetReferenceGameObject[] references;
}

[System.Serializable]
public class Addresabbles
{
    public AddresabbleGroup[] group;
}

namespace TPSBR.UI
{

public class AddressablesMain : MonoBehaviour
{
    [SerializeField]
    private Addresabbles _stages;
    private int _totalEnvironments;
    private int _loadedEnvironments;
    private int _currentStage = 0;

    public static AddressablesMain instance;
    
    [SerializeField] private GameObject loadingObj;
    [SerializeField] private TextMeshProUGUI loadingText;

    public void Awake(){
        instance = this;
       
    }

    public void StartLoading()
    {
        Debug.Log("Loading Addressables .." + _stages.group[_currentStage].name);
       

        if(_totalEnvironments>0 && _currentStage < _stages.group.Length){
            _totalEnvironments = _stages.group[_currentStage].references.Length;  
            _loadedEnvironments = 0;
            loadingObj.SetActive(true);
            loadingText.text = "Loading..." + _stages.group[_currentStage].name;
            StartCoroutine(LoadEnvironments(_stages.group[_currentStage].references));
        }     
    }

    public static Action<int> LoadingAssets;

    private IEnumerator LoadEnvironments(AssetReferenceGameObject[] _assets)
    {
        Debug.Log("Loading addressables" );
        for (int i = 0; i < _assets.Length; i++)
        {
            AsyncOperationHandle<GameObject> handle = _assets[i].LoadAssetAsync<GameObject>();

            handle.Completed += LoadCompletedCallback;
            _assets[i].ReleaseAsset();

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
            //GameObject environmentInstance = Instantiate(environment, _parent);
           // environmentInstance.transform.localPosition = Vector3.zero;
            _loadedEnvironments++;

            if(_loadedEnvironments == _totalEnvironments){
                loadingObj.SetActive(false);
                _currentStage++;
                StartLoading();

                //handle.Result.ReleaseAsset();
                //LoadingAssets?.Invoke(100);
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
        //_environments.ReleaseAsset();
    }
}


}