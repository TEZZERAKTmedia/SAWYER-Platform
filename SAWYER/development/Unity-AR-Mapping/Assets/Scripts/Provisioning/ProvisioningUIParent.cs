using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProvisioningUIParent : MonoBehaviour
{
    [Header("Global Controls")]
    public Button startProvisioningButton;
    public Button stopProvisioningButton;

    private string selectedDeviceUuid = null;

    void Awake(){

        if (startProvisioningButton != null)
            startProvisioningButton.onClick.AddListener(StartProvisioning);
        if (stopProvisioningButton != null)
            stopProvisioningButton.onClick.AddListener(StopProvisioning);

        
    }

    void Start()
    {
        HideAllPanels();
    }

    public void HideAllPanels()
    {
        
        if (scanPanel != null) scanPanel.SetActive(false);
        if (pinPanel != null) pinPanel.SetActive(false);
        if (wifiPanel != null) wifiPanel.SetActive(false);
        if (statusPanel != null) statusPanel.SetActive(false);
        
    }

    public void StartProvisioning()
    {
        HideAllPanels();
        ActivateScanView();
        ClearDeviceList();
        bleManager.StartScan();
        ShowStatus("Scanning for devices...");

    }

    public void StopProvisioning()
    {
        bleManager.StopScan();
        ClearDeviceList();
        ClearSSIDList();
        HideStatus();
    }

    public void ProceedToPinEntry()
    {
        HideAllPanels();
        ActivatePinView();
        ShowStatus("Please enter the PIN for your device.");
    }

    public void ProceedToWifiEntry()
    {
        HideAllPanels();
        ActivateWiFiView();
        ShowStatus("Enter Wi-Fi credentials...");

    }

    public void CompleteProvisioning()
    {
        HideAllPanels();
        ActivateStatusView();
        ShowStatus("Provisioning complete! You can now use your device.");
    }
    
    [Header("Panels")]
    public GameObject scanPanel;
    public GameObject pinPanel;
    public GameObject wifiPanel;
    public GameObject statusPanel;

    [Header("Scan Panel")]
    public Transform deviceListContent;         // Scroll View Content
    public GameObject deviceItemPrefab;
    public Button startScanButton;
    public Button stopScanButton;

    [Header("Pin Panel")]
    public TMP_InputField pinInput;
    public Button submitPinButton;

    [Header("WiFi Panel")]
    public Transform ssidListContent;           // Scroll View Content
    public GameObject ssidItemPrefab;
    public TMP_InputField wifiPasswordInput;
    public Button submitWifiButton;

    [Header("Status Panel")]
    public GameObject blockInputImage;
    public TMP_Text statusText;

    [Header("Manager References")]
    public BLEManager bleManager;

    // ====== PANEL MANAGEMENT ======

    // ====== PANEL MANAGEMENT ======

    public void ShowPanel(GameObject panelToShow)
    {
        scanPanel.SetActive(false);
        pinPanel.SetActive(false);
        wifiPanel.SetActive(false);
        statusPanel.SetActive(false);

        panelToShow.SetActive(true);
    }

    public void ActivateScanView() => ShowPanel(scanPanel);
    public void ActivatePinView() => ShowPanel(pinPanel);
    public void ActivateWiFiView() => ShowPanel(wifiPanel);
    public void ActivateStatusView() => ShowPanel(statusPanel);


    // ====== BUTTON CALLBACKS ======

    public void OnStartScanClicked()
    {
        bleManager.StartScan();
        ClearDeviceList();
        ShowStatus("Scanning for devices...");
    }

    public void OnStopScanClicked()
    {
        bleManager.StopScan();
        ShowStatus("Scan stopped.");
    }

    public void OnSubmitPinClicked()
    {
        string pin = pinInput.text.Trim();
        if (string.IsNullOrEmpty(pin))
        {
            ShowStatus("Please enter a valid PIN!");
            return;
        }
        bleManager.SendPIN(pin);
        ShowStatus("Submitting PIN...");
        ProceedToWifiEntry();
    }

    public void OnSubmitWiFiClicked()
    {
        string password = wifiPasswordInput.text.Trim();
        if (selectedSSID != null)
        {
            bleManager.SendWiFi(selectedSSID, password);
            ShowStatus("Submitting Wi-Fi credentials...");
            CompleteProvisioning();

        }
        else
        {
            ShowStatus("Please select a Wi-Fi network first!");
        }
    }

    // ====== DEVICE LIST MANAGEMENT ======

    public void ClearDeviceList()
    {
        foreach (Transform child in deviceListContent)
        {
            Destroy(child.gameObject);
        }
    }

    public void AddDeviceToList(string name, string uuid, int rssi)
    {
        GameObject item = Instantiate(deviceItemPrefab, deviceListContent);
        item.GetComponent<DeviceListItem>().Setup(name, uuid, rssi, bleManager);


    }

    public void OnDeviceSelected(string deviceUuid)
    {
        Debug.Log($"Selected device UUID: {deviceUuid}");
        bleManager.StopScan();

        selectedDeviceUuid = deviceUuid;

        ProceedToPinEntry();
    }

    // ====== WIFI SSID LIST MANAGEMENT ======

    private string selectedSSID = null;

    public void ClearSSIDList()
    {
        foreach (Transform child in ssidListContent)
        {
            Destroy(child.gameObject);
        }
    }

    public void AddSSIDToList(string ssid, int rssi)
    {
        GameObject item = Instantiate(ssidItemPrefab, ssidListContent);
        item.GetComponent<SSIDListItem>().Setup(ssid, rssi, this);
    }

    public void OnSSIDSelected(string ssid)
    {
        selectedSSID = ssid;
        Debug.Log($"Selected SSID: {ssid}");
    }

    // ====== STATUS MANAGEMENT ======

    public void ShowStatus(string message)
    {
        blockInputImage.SetActive(true);
        statusText.text = message;
        statusPanel.SetActive(true);
    }

    public void HideStatus()
    {
        blockInputImage.SetActive(false);
        statusPanel.SetActive(false);
    }
}
