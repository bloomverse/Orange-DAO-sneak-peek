using CrossMetaverseAvatars;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.IO;

public class LoadAvatarFromJSON : MonoBehaviour
{
    public bool loadAvatarOnStart = false;
    public AvatarCore avatar;
    public TextAsset jsonFile;

    public UnityEvent avatarLoaded;

    public void Start()
    {
        if (loadAvatarOnStart)
            LoadJSON();
        else
        {
            if (avatar != null)
            {
                string filePath = Path.Combine(Application.persistentDataPath, "jsonString.json");

                // Check if the file exists
                if (File.Exists(filePath))
                {
                    // Read the content of the file
                    string fileContent = File.ReadAllText(filePath);
                    jsonFile = new TextAsset(fileContent);
                    Debug.Log("File Content: " + fileContent);
                }
                else
                {
                    Debug.Log("File does not exist.");
                }
                AvatarJson.FromJson(avatar.gameObject, jsonFile.text);
                avatar.AvatarUpdateCompleted += Avatar_AvatarUpdateCompleted;
                avatar.UpdateAvatar();
            }
            else
                Debug.LogWarning("Avatar is null!", gameObject);
        }
    }
    public void LoadJSON()
    {
        if (jsonFile != null)
        {
            if (avatar != null)
            {
                AvatarJson.FromJson(avatar.gameObject, jsonFile.text);
                avatar.AvatarUpdateCompleted += Avatar_AvatarUpdateCompleted;
                avatar.UpdateAvatar();
            }
            else
                Debug.LogWarning("Avatar is null!", gameObject);
        }
        else
            Debug.LogWarning("JSON is null!", gameObject);
    }

    private void Avatar_AvatarUpdateCompleted()
    {
        avatarLoaded.Invoke();
        avatar.AvatarUpdateCompleted -= Avatar_AvatarUpdateCompleted;
    }
}
