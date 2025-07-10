using UnityEngine;
using UnityEngine.UI;
using TMPro;

[ExecuteAlways]
public class TMPWithBackground : MonoBehaviour
{
    [Header("Background Settings")]
    public Color backgroundColor = Color.black;
    public Sprite backgroundSprite;

    private RectTransform backgroundRect;
    private Image backgroundImage;
    private TextMeshProUGUI tmp;

    void OnEnable()
    {
        SetupStructure();
        UpdateBackground();
    }

    void OnValidate()
    {
        SetupStructure();
        UpdateBackground();
    }

    void SetupStructure()
    {
        // Assume THIS is the CellContainer (parent)
        if (transform.childCount < 2)
        {
            // Create background
            GameObject bgObj = new GameObject("Background", typeof(RectTransform), typeof(Image));
            bgObj.transform.SetParent(transform, false);
            bgObj.transform.SetSiblingIndex(0);

            RectTransform bgRect = bgObj.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;

            // Create Text
            GameObject txtObj = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
            txtObj.transform.SetParent(transform, false);
            txtObj.transform.SetSiblingIndex(1);

            RectTransform txtRect = txtObj.GetComponent<RectTransform>();
            txtRect.anchorMin = Vector2.zero;
            txtRect.anchorMax = Vector2.one;
            txtRect.offsetMin = Vector2.zero;
            txtRect.offsetMax = Vector2.zero;
        }

        backgroundImage = transform.GetChild(0).GetComponent<Image>();
        tmp = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }

    void UpdateBackground()
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = backgroundColor;
            backgroundImage.sprite = backgroundSprite;
            backgroundImage.type = backgroundSprite ? Image.Type.Sliced : Image.Type.Simple;
        }
    }
}
