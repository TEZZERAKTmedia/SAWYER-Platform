using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CanvasPanelManager : MonoBehaviour
{
    [System.Serializable]
    public class PanelControlEntry
    {
        [Header("Panel to control")]
        public GameObject panel;

        [Header("Triggers")]
        public GameObject openTriggerObject;
        public GameObject closeTriggerObject;

        [Header("Delays")]
        public float openDelay = 0f;
        public float closeDelay = 0f;
    }

    [Header("Panel Configurations")]
    [Tooltip("List of panels and their open/close triggers")]
    public List<PanelControlEntry> panelEntries = new List<PanelControlEntry>();

    private void Awake()
    {
        foreach (var entry in panelEntries)
        {
            // Ensure panel starts inactive
            if (entry.panel != null)
                entry.panel.SetActive(false);

            // Setup open trigger
            if (entry.openTriggerObject != null)
                AddClickEvent(entry.openTriggerObject, () => StartCoroutine(OpenPanel(entry)));

            // Setup close trigger
            if (entry.closeTriggerObject != null)
                AddClickEvent(entry.closeTriggerObject, () => StartCoroutine(ClosePanel(entry)));
        }
    }

    private void AddClickEvent(GameObject target, System.Action action)
    {
        if (target.GetComponent<Collider>() == null)
        {
            Debug.LogWarning($"'{target.name}' needs a Collider to receive clicks!");
            return;
        }

        EventTrigger trigger = target.GetComponent<EventTrigger>() ?? target.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerClick
        };
        entry.callback.AddListener((eventData) => action());
        trigger.triggers.Add(entry);

        Debug.Log($"CanvasPanelManager: Added click listener to '{target.name}'.");
    }

    private IEnumerator OpenPanel(PanelControlEntry entry)
    {
        yield return new WaitForSeconds(entry.openDelay);
        if (entry.panel != null)
            entry.panel.SetActive(true);
    }

    private IEnumerator ClosePanel(PanelControlEntry entry)
    {
        yield return new WaitForSeconds(entry.closeDelay);
        if (entry.panel != null)
            entry.panel.SetActive(false);
    }
}
