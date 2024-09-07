using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CrossMetaverseAvatars;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[RequireComponent(typeof(Dropdown))]
public class LoadWardrobe : MonoBehaviour
{
    [Tooltip("")]
    public List<string> wardrobeNodes = new List<string>();
    public string addressableKey;
    public AvatarCore avatarCore;

    AvatarRig avatarRig;
    Dropdown dropdown;
    List<AvatarItem> items = new List<AvatarItem>();
    private string maleRigAddress = "MaleRig";
    private string femaleRigAddress = "FemaleRig";

    // Start is called before the first frame update
    void Start()
    {
        dropdown = GetComponent<Dropdown>();
        dropdown.onValueChanged.AddListener(OnValueChanged);

        if(avatarCore)
            avatarRig = avatarCore.GetComponent<AvatarRig>();
    }

    private void OnEnable()
    {
        UpdateDropdownValues();
    }

    public void LoadAvatarItems()
    {
        LoadAvatarItems(new List<string> { addressableKey });
    }

    public void LoadAvatarItems(List<string> keys)
    {
        items.Clear();
        Addressables.LoadAssetsAsync<AvatarItem>(keys, AvatarItemLoadedCallback, Addressables.MergeMode.Union, true).Completed += OnAvatarItemsCompleted;
    }

    private void AvatarItemLoadedCallback(AvatarItem avatarItem)
    {
        items.Add(avatarItem);
    }

    private void OnAvatarItemsCompleted(AsyncOperationHandle<IList<AvatarItem>> handle)
    {
        UpdateDropdownValues();
    }

    public void UpdateDropdownValues()
    {
        if (dropdown)
        {
            dropdown.ClearOptions();

            List<string> options = new List<string>();
            options.Add("Nothing");

            if (avatarRig)
            {
                if (avatarRig.ContainsRigGroupAddress(maleRigAddress))
                {
                    foreach (var item in items)
                    {
                        if (item.maleWardrobeData != null && item.maleWardrobeData.Length > 0)
                            options.Add(item.itemName);
                    }
                }
                else
                {
                    if (avatarRig.ContainsRigGroupAddress(femaleRigAddress))
                    {
                        foreach (var item in items)
                        {
                            if (item.femaleWardrobeData != null && item.femaleWardrobeData.Length > 0)
                                options.Add(item.itemName);
                        }
                    }
                }
            }

            dropdown.AddOptions(options);
        }
    }

    void OnValueChanged(int selected)
    {
        if (avatarCore == null || avatarRig == null)
            return;

        var itemName = dropdown.options[selected].text;

        if (avatarCore.TryGetComponent(out AvatarWardrobe wardrobe))
        {
            if (selected == 0)
            {
                foreach (string node in wardrobeNodes)
                    wardrobe.RemoveWardrobe(node);
            }
            else
            {
                foreach (var item in items)
                {
                    if (item.itemName == itemName)
                    {
                        if(avatarRig.ContainsRigGroupAddress(maleRigAddress))
                        {
                            foreach (var data in item.maleWardrobeData)
                                wardrobe.AddWardrobe(data);
                            break;
                        }
                        else
                        {
                            if (avatarRig.ContainsRigGroupAddress(femaleRigAddress))
                            {
                                foreach (var data in item.femaleWardrobeData)
                                    wardrobe.AddWardrobe(data);
                                break;
                            }
                        }
                    }
                }

            }
        }
        avatarCore.UpdateAvatar();
        StartCoroutine(SaveData());
    }

    IEnumerator SaveData()
    {
        yield return new WaitForSeconds(2f);
        Debug.Log("Items : " + avatarCore.gameObject.GetComponent<AvatarWardrobe>().Items.Count);
        CharacterData.characterData.SaveCharacterData(avatarCore);
    }
}
