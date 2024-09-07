using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class AutoFitHeight : MonoBehaviour
{
    private Image image;
    private RectTransform rectTransform;

    void Start()
    {
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        AdjustSize();
    }


     public void AdjustSize()
    {
        if (image==null || image.sprite == null) return;

        float imageAspect = image.sprite.bounds.size.x / image.sprite.bounds.size.y;

        // Adjust the width of the RectTransform based on the height and aspect ratio of the image
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.y * imageAspect, rectTransform.sizeDelta.y);
    }
}
