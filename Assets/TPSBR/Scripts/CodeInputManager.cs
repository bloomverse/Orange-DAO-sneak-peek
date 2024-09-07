using UnityEngine;
using TMPro;

public class CodeInputManager : MonoBehaviour
{
    public TMP_InputField[] inputFields;

    private void Start()
    {
        foreach (var field in inputFields)
        {
            field.onValueChanged.AddListener(delegate { ValueChangeCheck(field); });
        }
    }

    public void ValueChangeCheck(TMP_InputField inputField)
    {
        int index = System.Array.IndexOf(inputFields, inputField);
        
       
        if (inputField.text.Length == 1 && index < inputFields.Length - 1)
        {
            inputFields[index + 1].Select();
        }
        
        else if (string.IsNullOrEmpty(inputField.text) && index > 0)
        {
            inputFields[index - 1].Select();
        }
    }
}