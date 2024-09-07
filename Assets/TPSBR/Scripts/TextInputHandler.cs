using System.Runtime.InteropServices;
using UnityEngine;
using TMPro; // Make sure you have the TextMesh Pro namespace included
using UnityEngine.EventSystems; // Required for event handling

public class TextInputHandler : MonoBehaviour, IPointerDownHandler
{
    public TMP_InputField inputField;

    private void Awake()
    {
        // Ensure you have an input field assigned or find it dynamically if needed
        if (inputField == null)
            inputField = GetComponent<TMP_InputField>();
    }

    // Implement the OnPointerDown method of IPointerDownHandler
    public void OnPointerDown(PointerEventData eventData)
    {
        // When the input field is clicked, call the JavaScript function
        GetTextFromJavaScript();
    }

    // Declare the external JavaScript function
    [DllImport("__Internal")]
    private static extern void GetTextFromJavaScript();

    // This function will be called from JavaScript to return the input
    public void ReceiveTextFromJavaScript(string text)
    {
        inputField.text = text;
    }
}
