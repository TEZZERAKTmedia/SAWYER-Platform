using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CellRenderer : MonoBehaviour
{
    [Header("Panels")]
    public GameObject devicePanel;
    public GameObject addPanel;
    public GameObject emptyPanel;

    [Header("Device UI")]
    public Image deviceImage;
    public TextMeshProUGUI deviceNameText;

    [Header("Add Button")]
    public GameObject addButton;

    // Setup for device cell
    public void ShowDevice(string deviceName, Sprite image)
    {
        devicePanel.SetActive(true);
        addPanel.SetActive(false);
        emptyPanel.SetActive(false);

        deviceNameText.text = deviceName;
        deviceImage.sprite = image;
    }

    // Setup for Add button
    public void ShowAddButton(UnityEngine.Events.UnityAction onClick)
{
    devicePanel.SetActive(false);
    addPanel.SetActive(true);
    emptyPanel.SetActive(false);

    // Add your own click listener to the entire AddPanel
    var trigger = addPanel.GetComponent<UnityEngine.EventSystems.EventTrigger>();
    if (trigger == null)
        trigger = addPanel.AddComponent<UnityEngine.EventSystems.EventTrigger>();

    trigger.triggers.Clear();

    var entry = new UnityEngine.EventSystems.EventTrigger.Entry
    {
        eventID = UnityEngine.EventSystems.EventTriggerType.PointerClick
    };
    entry.callback.AddListener((eventData) => onClick.Invoke());

    trigger.triggers.Add(entry);
}


    // Setup for empty cell
    public void ShowEmpty()
    {
        devicePanel.SetActive(false);
        addPanel.SetActive(false);
        emptyPanel.SetActive(true);
    }
}
