using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CrossMetaverseAvatars;

[RequireComponent(typeof(Dropdown))]
public class DropdownSetProperty : MonoBehaviour
{
    public string propertyName;
    public List<Color> colorOptions = new List<Color>();
    public AvatarCore avatarCore;

    private AvatarProperties properties;
    private Dropdown dropdown;

    // Start is called before the first frame update
    void Start()
    {
        dropdown = GetComponent<Dropdown>();
        dropdown.onValueChanged.AddListener(OnValueChanged);

        if (avatarCore)
            properties = avatarCore.GetComponent<AvatarProperties>();

        dropdown.ClearOptions();
        List<string> options = new List<string>();
        foreach(var option in colorOptions)
            options.Add("");

        dropdown.AddOptions(options);
    }

    void OnValueChanged(int selected)
    {
        if (string.IsNullOrEmpty(propertyName))
            return;

        if(properties.TryFindPropertyByAssetName(propertyName, out int index))
        {
            if (selected >= 0 && selected < colorOptions.Count)
            {
                properties.properties[index].values = colorOptions[selected];
                avatarCore.UpdateAvatar();
                Debug.Log("Items : " + avatarCore.gameObject.GetComponent<AvatarWardrobe>().Items.Count);
                CharacterData.characterData.SaveCharacterData(avatarCore);
            }
        }
    }
}
