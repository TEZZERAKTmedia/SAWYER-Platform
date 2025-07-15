using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class BLEUIController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject deviceListPanel;
    [SerializeField] private GameObject pinInputPanel;
    [SerializeField] private Transform deviceListParent;
    [SerializeField] private GameObject deviceItemPrefab;
    [SerializeField] private Button scanButton;
    [SerializeField] private Button stopScanButton;
    [SerializeField] private Text statusText;
    
    [Header("PIN Authentication")]
    [SerializeField] private InputField pinInputField;
    [SerializeField] private Button submitPinButton;
    [SerializeField] private Button cancelPinButton;
    [SerializeField] private Text pinStatusText;
    
    [Header("Connection Status")]
    [SerializeField] private Text connectionStatusText;
    [SerializeField] private Button disconnectButton;
    
    private UnityBLEManager bleManager;
    private List<UnityBLEManager.DeviceInfo> discoveredDevices = new List<UnityBLEManager.DeviceInfo>();
    private Dictionary<string, GameObject> deviceUIItems = new Dictionary<string, GameObject>();
    private UnityBLEManager.DeviceInfo selectedDevice;
    private bool isAuthenticating = false;
    
    // Security states
    private enum AuthState
    {
        None,
        WaitingForPin,
        PinSent,
        Authenticated,
        AuthFailed
    }
    private AuthState currentAuthState = AuthState.None;

    void Start()
    {
        InitializeUI();
        SetupBLEManager();
    }

    void InitializeUI()
    {
        // Setup button listeners
        scanButton.onClick.AddListener(StartScanning);
        stopScanButton.onClick.AddListener(StopScanning);
        submitPinButton.onClick.AddListener(SubmitPin);
        cancelPinButton.onClick.AddListener(CancelPinInput);
        disconnectButton.onClick.AddListener(Disconnect);
        
        // Initial UI state
        deviceListPanel.SetActive(true);
        pinInputPanel.SetActive(false);
        stopScanButton.interactable = false;
        disconnectButton.interactable = false;
        
        UpdateStatusText("Ready to scan for devices");
    }

    void SetupBLEManager()
    {
        bleManager = FindObjectOfType<UnityBLEManager>();
        if (bleManager == null)
        {
            Debug.LogError("UnityBLEManager not found in scene!");
            return;
        }

        // Subscribe to BLE events
        bleManager.OnDeviceDiscovered += HandleDeviceDiscovered;
        bleManager.OnDeviceConnected += HandleDeviceConnected;
        bleManager.OnDataReceived += HandleDataReceived;
    }

    #region UI Event Handlers

    void StartScanning()
    {
        ClearDeviceList();
        bleManager.StartScanning();
        scanButton.interactable = false;
        stopScanButton.interactable = true;
        UpdateStatusText("Scanning for devices...");
    }

    void StopScanning()
    {
        bleManager.StopScanning();
        scanButton.interactable = true;
        stopScanButton.interactable = false;
        UpdateStatusText($"Scan stopped. Found {discoveredDevices.Count} devices.");
    }

    void SubmitPin()
    {
        string pin = pinInputField.text.Trim();
        if (string.IsNullOrEmpty(pin))
        {
            UpdatePinStatusText("Please enter a PIN");
            return;
        }

        if (pin.Length < 4)
        {
            UpdatePinStatusText("PIN must be at least 4 characters");
            return;
        }

        SendPinToDevice(pin);
    }

    void CancelPinInput()
    {
        HidePinPanel();
        currentAuthState = AuthState.None;
        UpdateStatusText("PIN authentication cancelled");
    }

    void Disconnect()
    {
        // Note: You might want to add a disconnect method to UnityBLEManager
        bleManager.SendData("DISCONNECT");
        currentAuthState = AuthState.None;
        disconnectButton.interactable = false;
        UpdateConnectionStatusText("Disconnected");
    }

    #endregion

    #region BLE Event Handlers

    void HandleDeviceDiscovered(string deviceJson)
    {
        try
        {
            var deviceInfo = JsonUtility.FromJson<UnityBLEManager.DeviceInfo>(deviceJson);
            
            // Check if device already exists
            if (discoveredDevices.Any(d => d.uuid == deviceInfo.uuid))
                return;

            discoveredDevices.Add(deviceInfo);
            CreateDeviceListItem(deviceInfo);
            
            UpdateStatusText($"Found {discoveredDevices.Count} devices");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error parsing device info: {e.Message}");
        }
    }

    void HandleDeviceConnected(string uuid)
    {
        selectedDevice = discoveredDevices.FirstOrDefault(d => d.uuid == uuid);
        if (selectedDevice != null)
        {
            UpdateConnectionStatusText($"Connected to {selectedDevice.name}");
            disconnectButton.interactable = true;
            
            // Start authentication process
            StartPinAuthentication();
        }
    }

    void HandleDataReceived(string data)
    {
        Debug.Log($"Received data: {data}");
        
        // Handle authentication responses
        if (currentAuthState == AuthState.PinSent)
        {
            HandleAuthenticationResponse(data);
        }
        else if (currentAuthState == AuthState.Authenticated)
        {
            HandleAuthenticatedData(data);
        }
        else
        {
            // Handle other data types
            ProcessReceivedData(data);
        }
    }

    #endregion

    #region Device List Management

    void CreateDeviceListItem(UnityBLEManager.DeviceInfo deviceInfo)
    {
        GameObject deviceItem = Instantiate(deviceItemPrefab, deviceListParent);
        
        // Setup device item UI (assuming your prefab has these components)
        Text nameText = deviceItem.transform.Find("NameText").GetComponent<Text>();
        Text uuidText = deviceItem.transform.Find("UUIDText").GetComponent<Text>();
        Text rssiText = deviceItem.transform.Find("RSSIText").GetComponent<Text>();
        Button connectButton = deviceItem.transform.Find("ConnectButton").GetComponent<Button>();
        
        nameText.text = deviceInfo.name;
        uuidText.text = deviceInfo.uuid;
        rssiText.text = $"RSSI: {deviceInfo.rssi}";
        
        connectButton.onClick.AddListener(() => ConnectToDevice(deviceInfo));
        
        deviceUIItems[deviceInfo.uuid] = deviceItem;
    }

    void ClearDeviceList()
    {
        foreach (var item in deviceUIItems.Values)
        {
            Destroy(item);
        }
        deviceUIItems.Clear();
        discoveredDevices.Clear();
    }

    void ConnectToDevice(UnityBLEManager.DeviceInfo deviceInfo)
    {
        selectedDevice = deviceInfo;
        bleManager.ConnectToDevice(deviceInfo.uuid);
        UpdateStatusText($"Connecting to {deviceInfo.name}...");
        
        // Disable all connect buttons during connection
        foreach (var item in deviceUIItems.Values)
        {
            item.transform.Find("ConnectButton").GetComponent<Button>().interactable = false;
        }
    }

    #endregion

    #region PIN Authentication

    void StartPinAuthentication()
    {
        currentAuthState = AuthState.WaitingForPin;
        ShowPinPanel();
        UpdatePinStatusText("Enter PIN to authenticate with device");
    }

    void SendPinToDevice(string pin)
    {
        if (currentAuthState != AuthState.WaitingForPin)
            return;

        currentAuthState = AuthState.PinSent;
        
        // Send PIN in a structured format
        string pinMessage = $"AUTH_PIN:{pin}";
        bleManager.SendData(pinMessage);
        
        UpdatePinStatusText("Authenticating...");
        submitPinButton.interactable = false;
        
        // Set timeout for authentication
        StartCoroutine(AuthenticationTimeout());
    }

    System.Collections.IEnumerator AuthenticationTimeout()
    {
        yield return new WaitForSeconds(10f); // 10 second timeout
        
        if (currentAuthState == AuthState.PinSent)
        {
            currentAuthState = AuthState.AuthFailed;
            UpdatePinStatusText("Authentication timeout");
            submitPinButton.interactable = true;
        }
    }

    void HandleAuthenticationResponse(string data)
    {
        if (data.Contains("AUTH_SUCCESS"))
        {
            currentAuthState = AuthState.Authenticated;
            HidePinPanel();
            UpdateStatusText("Authentication successful! Ready to receive WiFi networks.");
            
            // Now we can safely request WiFi list
            bleManager.RequestWiFiNetworks();
        }
        else if (data.Contains("AUTH_FAILED"))
        {
            currentAuthState = AuthState.AuthFailed;
            UpdatePinStatusText("Invalid PIN. Please try again.");
            submitPinButton.interactable = true;
            pinInputField.text = "";
        }
    }

    void HandleAuthenticatedData(string data)
    {
        // Handle WiFi network data and other authenticated communications
        if (data.StartsWith("WIFI_NETWORKS:"))
        {
            HandleWiFiNetworksData(data);
        }
        else if (data.StartsWith("WIFI_CONNECTED:"))
        {
            HandleWiFiConnectionResult(data);
        }
        // Add other authenticated data handlers here
    }

    #endregion

    #region Data Processing

    void ProcessReceivedData(string data)
    {
        // Handle non-authenticated data
        Debug.Log($"Processing data: {data}");
    }

    void HandleWiFiNetworksData(string data)
    {
        // Parse and display WiFi networks
        string networksJson = data.Substring("WIFI_NETWORKS:".Length);
        // Process WiFi networks list here
        Debug.Log($"Received WiFi networks: {networksJson}");
    }

    void HandleWiFiConnectionResult(string data)
    {
        // Handle WiFi connection result
        Debug.Log($"WiFi connection result: {data}");
    }

    #endregion

    #region UI Helpers

    void ShowPinPanel()
    {
        deviceListPanel.SetActive(false);
        pinInputPanel.SetActive(true);
        pinInputField.text = "";
        pinInputField.Select();
        submitPinButton.interactable = true;
    }

    void HidePinPanel()
    {
        pinInputPanel.SetActive(false);
        deviceListPanel.SetActive(true);
    }

    void UpdateStatusText(string message)
    {
        if (statusText != null)
            statusText.text = message;
    }

    void UpdatePinStatusText(string message)
    {
        if (pinStatusText != null)
            pinStatusText.text = message;
    }

    void UpdateConnectionStatusText(string message)
    {
        if (connectionStatusText != null)
            connectionStatusText.text = message;
    }

    #endregion

    void OnDestroy()
    {
        // Unsubscribe from events
        if (bleManager != null)
        {
            bleManager.OnDeviceDiscovered -= HandleDeviceDiscovered;
            bleManager.OnDeviceConnected -= HandleDeviceConnected;
            bleManager.OnDataReceived -= HandleDataReceived;
        }
    }
}