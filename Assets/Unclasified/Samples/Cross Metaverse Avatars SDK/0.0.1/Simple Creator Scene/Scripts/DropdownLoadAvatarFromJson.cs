using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CrossMetaverseAvatars;
using UnityEngine.Events;

[RequireComponent(typeof(Dropdown))]
public class DropdownLoadAvatarFromJson : MonoBehaviour
{
    public TextAsset maleJSON;
    public TextAsset femaleJSON;
    public AvatarCore avatarCore;
    public UnityEvent avatarLoaded;

    private Dropdown dropdown;
    private AvatarRig avatarRig;

    // Start is called before the first frame update
    void Start()
    {
        dropdown = GetComponent<Dropdown>();
        dropdown.onValueChanged.AddListener(OnValueChanged);

        if (avatarCore)
        {
            avatarRig = avatarCore.GetComponent<AvatarRig>();
        }
    }

    void OnValueChanged(int selected)
    {
        if (avatarCore == null || avatarRig == null)
            return;

        if(selected == 0 && !avatarRig.ContainsRigGroupAddress("MaleRig"))
        {
            AvatarJson.FromJson(avatarCore.gameObject, maleJSON.text);
            avatarCore.AvatarUpdateCompleted += AvatarCore_AvatarUpdateCompleted;
            avatarCore.UpdateAvatar();
        }

        if (selected == 1 && !avatarRig.ContainsRigGroupAddress("FemaleRig"))
        {
            AvatarJson.FromJson(avatarCore.gameObject, femaleJSON.text);
            avatarCore.AvatarUpdateCompleted += AvatarCore_AvatarUpdateCompleted;
            avatarCore.UpdateAvatar();
        }
    }

    private void AvatarCore_AvatarUpdateCompleted()
    {
        avatarLoaded.Invoke();
        avatarCore.AvatarUpdateCompleted -= AvatarCore_AvatarUpdateCompleted;
    }
}
