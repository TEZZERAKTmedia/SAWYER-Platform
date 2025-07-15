using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class ProvisioningUIController : MonoBehaviour
{
    public enum ProvisioningState
    {
        BluetoothScan,
        BluetoothPINEntry,
        WifiList,
        WifiCredentials,
        Complete,
        Error
    }

    [Header("Panels (One Active at a Time)")]
    [SerializeField] private GameObject bluetoothScanPanel;
    [SerializeField] private GameObject bluetoothPINPanel;
    [SerializeField] private GameObject wifiListPanel;
    [SerializeField] private GameObject wifiCredentialsPanel;
    [SerializeField] private GameObject completePanel;
    [SerializeField] private GameObject errorPanel;

    [Header("UI Elements to Hide During Provisioning")]
    [Tooltip("These GameObjects will be hidden when provisioning starts, and reactivated when provisioning closes.")]
    [SerializeField] private List<GameObject> objectsToDeactivateDuringProvisioning;

    [Header("Buttons That Start This Flow")]
    [SerializeField] private List<Button> startFlowButtons;

    [Header("Buttons That Close This Flow")]
    [SerializeField] private List<Button> closeFlowButtons;

    [Header("Simulation / Development Mode")]
    [SerializeField] private bool simulationMode = true;
    [SerializeField] private List<string> simulatedBluetoothDevices;
    [SerializeField] private List<string> simulatedWifiNetworks;

    private ProvisioningState currentState;

    private void Awake()
    {
        HideAllPanels();

        // Hook up start buttons
        if (startFlowButtons != null)
        {
            foreach (var button in startFlowButtons)
            {
                if (button != null)
                {
                    button.onClick.AddListener(StartProvisioningFlow);
                }
            }
        }

        // Hook up close buttons
        if (closeFlowButtons != null)
        {
            foreach (var button in closeFlowButtons)
            {
                if (button != null)
                {
                    button.onClick.AddListener(CloseProvisioningFlow);
                }
            }
        }
    }

    private void Start()
    {
        Debug.Log($"[ProvisioningUI] Simulation Mode: {simulationMode}");
        gameObject.SetActive(false);
    }

    public void StartProvisioningFlow()
    {
        Debug.Log("[ProvisioningUI] Starting provisioning flow...");

        // Hide other UI sections
        if (objectsToDeactivateDuringProvisioning != null)
        {
            foreach (var obj in objectsToDeactivateDuringProvisioning)
            {
                if (obj != null)
                {
                    obj.SetActive(false);
                }
            }
        }

        // Show this provisioning UI
        gameObject.SetActive(true);

        // Always start at BluetoothScan
        SetState(ProvisioningState.BluetoothScan);
    }

    public void CloseProvisioningFlow()
    {
        Debug.Log("[ProvisioningUI] Closing provisioning flow...");

        // Hide this provisioning UI
        HideAllPanels();
        gameObject.SetActive(false);

        // Reactivate other UI sections
        if (objectsToDeactivateDuringProvisioning != null)
        {
            foreach (var obj in objectsToDeactivateDuringProvisioning)
            {
                if (obj != null)
                {
                    obj.SetActive(true);
                }
            }
        }
    }

    public void SetState(ProvisioningState newState)
    {
        currentState = newState;
        HideAllPanels();

        switch (newState)
        {
            case ProvisioningState.BluetoothScan:
                bluetoothScanPanel?.SetActive(true);
                if (simulationMode)
                {
                    Debug.Log("[SIMULATION] Populating dummy BLE devices...");
                    bluetoothScanPanel.GetComponent<BluetoothScanPanel>()?.Populate(simulatedBluetoothDevices);
                }
                break;

            case ProvisioningState.BluetoothPINEntry:
                bluetoothPINPanel?.SetActive(true);
                if (simulationMode)
                {
                    Debug.Log("[SIMULATION] Auto-success PIN entry after delay...");
                    Invoke(nameof(SimulatePinSuccess), 1.0f);
                }
                break;

            case ProvisioningState.WifiList:
                wifiListPanel?.SetActive(true);
                if (simulationMode)
                {
                    Debug.Log("[SIMULATION] Populating dummy Wi-Fi networks...");
                    wifiListPanel.GetComponent<WifiListPanel>()?.Populate(simulatedWifiNetworks);
                }
                break;

            case ProvisioningState.WifiCredentials:
                wifiCredentialsPanel?.SetActive(true);
                if (simulationMode)
                {
                    Debug.Log("[SIMULATION] Auto-success Wi-Fi after delay...");
                    Invoke(nameof(SimulateWifiSubmitSuccess), 1.0f);
                }
                break;

            case ProvisioningState.Complete:
                completePanel?.SetActive(true);
                break;

            case ProvisioningState.Error:
                errorPanel?.SetActive(true);
                break;
        }
    }

    private void HideAllPanels()
    {
        bluetoothScanPanel?.SetActive(false);
        bluetoothPINPanel?.SetActive(false);
        wifiListPanel?.SetActive(false);
        wifiCredentialsPanel?.SetActive(false);
        completePanel?.SetActive(false);
        errorPanel?.SetActive(false);
    }

    // Developer simulation helpers
    private void SimulatePinSuccess()
    {
        Debug.Log("[SIMULATION] Auto-success PIN step.");
        OnPinSubmitted(true);
    }

    private void SimulateWifiSubmitSuccess()
    {
        Debug.Log("[SIMULATION] Auto-success Wi-Fi credentials step.");
        OnWifiCredentialsSubmitted(true);
    }

    // === UI Callbacks from Panels ===

    public void OnDeviceSelected()
    {
        Debug.Log("[ProvisioningUI] Device selected by user, moving to PIN entry...");
        SetState(ProvisioningState.BluetoothPINEntry);
    }

    public void OnPinSubmitted(bool success)
    {
        if (success)
        {
            Debug.Log("[ProvisioningUI] PIN success, fetching Wi-Fi list...");
            SetState(ProvisioningState.WifiList);
        }
        else
        {
            Debug.LogError("[ProvisioningUI] PIN failed");
            SetState(ProvisioningState.Error);
        }
    }

    public void OnWifiNetworkSelected()
    {
        Debug.Log("[ProvisioningUI] Wi-Fi network chosen, requesting password...");
        SetState(ProvisioningState.WifiCredentials);
    }

    public void OnWifiCredentialsSubmitted(bool success)
    {
        if (success)
        {
            Debug.Log("[ProvisioningUI] Provisioning complete!");
            SetState(ProvisioningState.Complete);
        }
        else
        {
            Debug.LogError("[ProvisioningUI] Wi-Fi setup failed");
            SetState(ProvisioningState.Error);
        }
    }

    public void OnErrorAcknowledged()
    {
        Debug.Log("[ProvisioningUI] User acknowledged error, returning to Scan...");
        SetState(ProvisioningState.BluetoothScan);
    }
}
