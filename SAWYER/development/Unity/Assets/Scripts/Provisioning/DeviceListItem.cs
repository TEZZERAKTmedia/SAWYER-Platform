using UnityEngine;
using UnityEngine.UI;

public class DeviceListItem : MonoBehaviour
{
    [Header("UI References")]
    public Text nameText;
    public Text uuidText;
    public Text rssiText;

    private BLEManager bleManager;

    /// <summary>
    /// Call this immediately after Instantiate(...) to populate the row.
    /// </summary>
    public void Setup(string name, string uuid, int rssi, BLEManager manager)
    {
        nameText.text = name;
        uuidText.text = uuid;
        rssiText.text = $"RSSI: {rssi}";
        bleManager = manager;
    }
}
