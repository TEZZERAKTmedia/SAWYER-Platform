using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UITMPPageController : MonoBehaviour
{
    [Header("TMP Labels to act as Tabs")]
    [Tooltip("Assign your TextMeshProUGUI labels here. They will act as buttons.")]
    public List<TextMeshProUGUI> tabLabels = new List<TextMeshProUGUI>();

    [Header("Pages to Switch Between")]
    [Tooltip("Each Page GameObject will be enabled/disabled as tabs are clicked.")]
    public List<GameObject> pages = new List<GameObject>();

    [Header("Optional")]
    [Tooltip("Which page to show by default at Start.")]
    public int defaultPageIndex = 0;

    private int currentPageIndex = -1;

    void Start()
    {
        // Attach triggers to TMP labels
        for (int i = 0; i < tabLabels.Count; i++)
        {
            int index = i;
            if (tabLabels[index] != null)
            {
                AddClickHandler(tabLabels[index], index);
            }
        }

        // Open default
        OpenPage(defaultPageIndex);
    }

    void AddClickHandler(TextMeshProUGUI label, int index)
    {
        if (label == null) return;

        EventTrigger trigger = label.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = label.gameObject.AddComponent<EventTrigger>();

        var entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerClick
        };
        entry.callback.AddListener((eventData) => OnTabSelected(index));
        trigger.triggers.Add(entry);
    }

    public void OpenPage(int index)
    {
        if (pages == null || pages.Count == 0)
        {
            Debug.LogWarning("No pages assigned!");
            return;
        }

        if (index < 0 || index >= pages.Count)
        {
            Debug.LogWarning($"Invalid page index: {index}");
            return;
        }

        for (int i = 0; i < pages.Count; i++)
        {
            if (pages[i] != null)
                pages[i].SetActive(i == index);
        }

        currentPageIndex = index;
    }

    private void OnTabSelected(int index)
    {
        Debug.Log($"Tab {index} selected");
        OpenPage(index);
    }
}
