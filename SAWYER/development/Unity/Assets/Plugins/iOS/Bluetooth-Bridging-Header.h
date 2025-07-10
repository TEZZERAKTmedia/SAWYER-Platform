#if UNITY_IOS
using System.Runtime.InteropServices;
#endif
using UnityEngine;

public class BluetoothManager : MonoBehaviour
{
    // ============================
    // Native iOS Plugin Functions
    // ============================
#if UNITY_IOS
    [DllImport("__Internal")]
    private static extern void StartBLEScan();

    [DllImport("__Internal")]
    private static extern void StopBLEScan();

    [DllImport("__Internal")]
    private static extern void ConnectToPeripheral(string uuid);

    [DllImport("__Internal")]
    private static extern void WriteToCharacteristic(string data);

    [DllImport("__Internal")]
    private static extern void RequestWiFiList();

    [DllImport("__Internal")]
    private static extern string GetCurrentSSID();
#endif

    // ============================
    // Public Unity Callers
    // ============================

    public void OnScanButtonClicked()
    {
#if UNITY_IOS
        Debug.Log("[Unity] Start BLE Scan pressed");
        StartBLEScan();
#endif
    }

    public void OnStopButtonClicked()
    {
#if UNITY_IOS
        Debug.Log("[Unity] Stop BLE Scan pressed");
        StopBLEScan();
#endif
    }

    public void OnConnectButtonClicked(string uuid)
    {
#if UNITY_IOS
        Debug.Log($"[Unity] Connect to Peripheral pressed: UUID = {uuid}");
        ConnectToPeripheral(uuid);
#endif
    }

    public void SendWiFiCredentials(string ssidPasswordJson)
    {
#if UNITY_IOS
        Debug.Log($"[Unity] Sending Wi-Fi credentials JSON: {ssidPasswordJson}");
        WriteToCharacteristic(ssidPasswordJson);
#endif
    }

    public void OnRequestWiFiList()
    {
#if UNITY_IOS
        Debug.Log("[Unity] Requesting Wi-Fi list from Pi over BLE");
        RequestWiFiList();
#endif
    }

    public string GetPhoneCurrentSSID()
    {
#if UNITY_IOS
        Debug.Log("[Unity] Getting iOS device's current connected SSID");
        return GetCurrentSSID();
#else
        return null;
#endif
    }

    // ============================
    // Callbacks from Swift
    // ============================

    // Called by Swift when a device is found during scanning
    public void OnDeviceFound(string json)
    {
        Debug.Log($"[Unity] Device Found: {json}");
        // TODO: Parse JSON, update device list UI
    }

    // Called by Swift when connection succeeds
    public void OnPeripheralConnected(string uuid)
    {
        Debug.Log($"[Unity] Connected to Peripheral UUID: {uuid}");
        // TODO: Update UI to show connection
    }

    // Called by Swift when BLE data (e.g. Wi-Fi list JSON) is received
    public void OnDataReceived(string json)
    {
        Debug.Log($"[Unity] Data Received from Pi: {json}");
        // TODO: Parse JSON, show list of available Wi-Fi networks
    }
}
