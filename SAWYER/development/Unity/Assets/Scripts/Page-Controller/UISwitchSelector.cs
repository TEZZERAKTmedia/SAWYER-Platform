using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISwitchSelector : MonoBehaviour
{
    [System.Serializable]
    public class LabelConfig
    {
        [Header("Clickable Image Container")]
        public Image buttonImage;  // Parent image to click

        [Header("Text Label Inside")]
        public TextMeshProUGUI label;  // TMP label INSIDE the image

        [Header("Text Color Settings")]
        public Color selectedTextColor = Color.white;
        public Color unselectedTextColor = Color.gray;

        [Header("Background Image Settings")]
        public Image backgroundImage;
        public Color selectedBackgroundColor = Color.white;
        public Color unselectedBackgroundColor = Color.clear;

        [Header("Linked Panel")]
        public GameObject linkedPanel;  // The panel to show when this tab is selected
    }

    [Header("Tabs")]
    public List<LabelConfig> labels = new List<LabelConfig>();

    [Header("Animation")]
    public float colorLerpDuration = 0.2f;

    private LabelConfig selectedConfig;
    private Dictionary<TextMeshProUGUI, Coroutine> activeTextCoroutines = new Dictionary<TextMeshProUGUI, Coroutine>();
    private Dictionary<Image, Coroutine> activeBgCoroutines = new Dictionary<Image, Coroutine>();

    void Start()
    {
        // Add click listeners
        foreach (var config in labels)
        {
            if (config.buttonImage != null)
                AddClickHandler(config.buttonImage.gameObject, config);
        }

        // Select first by default
        if (labels.Count > 0 && labels[0].buttonImage != null)
        {
            OnLabelClicked(labels[0]);
        }
    }

    void AddClickHandler(GameObject clickable, LabelConfig config)
    {
        if (clickable == null) return;

        EventTrigger trigger = clickable.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = clickable.AddComponent<EventTrigger>();

        var entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerClick
        };
        entry.callback.AddListener((eventData) => OnLabelClicked(config));
        trigger.triggers.Add(entry);
    }

    public void OnLabelClicked(LabelConfig selected)
    {
        Debug.Log($"Selected: {(selected.label != null ? selected.label.text : selected.buttonImage.name)}");

        foreach (var config in labels)
        {
            bool isSelected = (config == selected);

            // Animate Text Color
            if (config.label != null)
            {
                Color targetColor = isSelected ? config.selectedTextColor : config.unselectedTextColor;
                if (activeTextCoroutines.ContainsKey(config.label) && activeTextCoroutines[config.label] != null)
                    StopCoroutine(activeTextCoroutines[config.label]);

                activeTextCoroutines[config.label] = StartCoroutine(LerpTextColor(config.label, targetColor));
            }

            // Animate Background Color
            if (config.backgroundImage != null)
            {
                Color targetBGColor = isSelected ? config.selectedBackgroundColor : config.unselectedBackgroundColor;
                if (activeBgCoroutines.ContainsKey(config.backgroundImage) && activeBgCoroutines[config.backgroundImage] != null)
                    StopCoroutine(activeBgCoroutines[config.backgroundImage]);

                activeBgCoroutines[config.backgroundImage] = StartCoroutine(LerpImageColor(config.backgroundImage, targetBGColor));
            }

            // Enable/Disable Linked Panels
            if (config.linkedPanel != null)
            {
                config.linkedPanel.SetActive(isSelected);
            }

            if (isSelected)
                selectedConfig = config;
        }

        OnSwitchSelected(selected);
    }

    private IEnumerator LerpTextColor(TextMeshProUGUI label, Color targetColor)
    {
        Color startColor = label.color;
        float elapsed = 0f;

        while (elapsed < colorLerpDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / colorLerpDuration);
            label.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }

        label.color = targetColor;
    }

    private IEnumerator LerpImageColor(Image image, Color targetColor)
    {
        Color startColor = image.color;
        float elapsed = 0f;

        while (elapsed < colorLerpDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / colorLerpDuration);
            image.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }

        image.color = targetColor;
    }

    protected virtual void OnSwitchSelected(LabelConfig selected)
    {
        // You can override this for callbacks
    }
}
