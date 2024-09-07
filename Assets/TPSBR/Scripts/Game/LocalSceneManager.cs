


using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LocalSceneManager : MonoBehaviour
{
    public void LoadLocalScene(string sceneName)
    {
        
        StartCoroutine(LoadLocalSceneCoroutine(sceneName));
    }

    private IEnumerator LoadLocalSceneCoroutine(string sceneName)
    {
        var loadOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        while (!loadOperation.isDone)
        {
            yield return null;
        }

        Scene loadedScene = SceneManager.GetSceneByName(sceneName);
        if (loadedScene.IsValid())
        {
            SceneManager.SetActiveScene(loadedScene);
        }
    }
}