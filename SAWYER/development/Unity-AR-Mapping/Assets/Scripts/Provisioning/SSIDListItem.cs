using UnityEngine;
using UnityEngine.UI;

public class SSIDListItem : MonoBehaviour
{
    public Text ssidText;
    public Text rssiText;
    public Button selectButton;

    private string ssid;
    private ProvisioningUIParent uiManager;

    public void Setup(string ssidValue, int rssi, ProvisioningUIParent manager)
    {
        ssid = ssidValue;
        ssidText.text = ssid;
        rssiText.text = $"Signal: {rssi}";
        uiManager = manager;

        selectButton.onClick.AddListener(OnSelectPressed);
    }

    private void OnSelectPressed()
    {
        Debug.Log($"[SSIDListItem] Selected SSID: {ssid}");
        uiManager.OnSSIDSelected(ssid);
    }
}
