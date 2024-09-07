using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropdownUpdateItemColor : MonoBehaviour
{
    public DropdownSetProperty dropdownSetProperty;

    [SerializeField]
    Image image;

    private void OnEnable()
    {
        UpdateImageColor();
    }

    private void OnTransformParentChanged()
    {
        UpdateImageColor();
    }

    void UpdateImageColor()
    {
        if (image != null && dropdownSetProperty != null)
        {
            int index = transform.GetSiblingIndex() - 1;
            if (index >= 0 && index < dropdownSetProperty.colorOptions.Count)
            {
                image.color = dropdownSetProperty.colorOptions[index];
            }
        }
    }
}
