using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class UnityBLEManager : MonoBehaviour
{
    [Header("Debug")]
    public bool enableDebugLogs = true;
    
    [Header("Status")]
    public bool isScanning = false;
    public bool isConnected = false;
    public string connectedDeviceUUID = "";
    
    // Events
    public System.Action<string> OnDeviceDiscovered;
    public System.Action<string> OnDeviceConnected;
    public System.Action<string> OnDataReceived;

#if UNITY_IOS && !UNITY_EDITOR
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

    private void Awake()
    {
        // Make sure this GameObject is named "BluetoothGameObject" 
        // to match the UnitySendMessage calls
        gameObject.name = "BluetoothGameObject";
    }

    #region Public Methods

    public void StartScanning()
    {
#if UNITY_IOS && !UNITY_EDITOR
        try
        {
            StartBLEScan();
            isScanning = true;
            Log("Started BLE scanning");
        }
        catch (System.Exception e)
        {
            LogError($"Error starting scan: {e.Message}");
        }
#else
        Log("BLE scanning only available on iOS device");
#endif
    }

    public void StopScanning()
    {
#if UNITY_IOS && !UNITY_EDITOR
        try
        {
            StopBLEScan();
            isScanning = false;
            Log("Stopped BLE scanning");
        }
        catch (System.Exception e)
        {
            LogError($"Error stopping scan: {e.Message}");
        }
#else
        Log("BLE scanning only available on iOS device");
#endif
    }

    public void ConnectToDevice(string uuid)
    {
#if UNITY_IOS && !UNITY_EDITOR
        try
        {
            ConnectToPeripheral(uuid);
            Log($"Attempting to connect to device: {uuid}");
        }
        catch (System.Exception e)
        {
            LogError($"Error connecting to device: {e.Message}");
        }
#else
        Log("BLE connection only available on iOS device");
#endif
    }

    public void SendData(string data)
    {
#if UNITY_IOS && !UNITY_EDITOR
        if (!isConnected)
        {
            LogError("Cannot send data - not connected to any device");
            return;
        }

        try
        {
            WriteToCharacteristic(data);
            Log($"Sent data: {data}");
        }
        catch (System.Exception e)
        {
            LogError($"Error sending data: {e.Message}");
        }
#else
        Log("BLE communication only available on iOS device");
#endif
    }

    public void RequestWiFiNetworks()
    {
#if UNITY_IOS && !UNITY_EDITOR
        try
        {
            RequestWiFiList();
            Log("Requested WiFi list");
        }
        catch (System.Exception e)
        {
            LogError($"Error requesting WiFi list: {e.Message}");
        }
#else
        Log("WiFi list request only available on iOS device");
#endif
    }

    public string GetCurrentWiFiSSID()
    {
#if UNITY_IOS && !UNITY_EDITOR
        try
        {
            string ssid = GetCurrentSSID();
            Log($"Current WiFi SSID: {ssid ?? "None"}");
            return ssid;
        }
        catch (System.Exception e)
        {
            LogError($"Error getting current SSID: {e.Message}");
            return null;
        }
#else
        Log("WiFi SSID retrieval only available on iOS device");
        return null;
#endif
    }

    #endregion

    #region Callbacks from iOS (called via UnitySendMessage)

    // Called when a BLE device is discovered
    public void OnDeviceFound(string deviceJson)
    {
        try
        {
            var deviceInfo = JsonUtility.FromJson<DeviceInfo>(deviceJson);
            Log($"Found device: {deviceInfo.name} ({deviceInfo.uuid}) RSSI: {deviceInfo.rssi}");
            OnDeviceDiscovered?.Invoke(deviceJson);
        }
        catch (System.Exception e)
        {
            LogError($"Error parsing device info: {e.Message}");
        }
    }

    // Called when successfully connected to a peripheral
    public void OnPeripheralConnected(string uuid)
    {
        isConnected = true;
        connectedDeviceUUID = uuid;
        Log($"Connected to device: {uuid}");
        OnDeviceConnected?.Invoke(uuid);
    }

    // Called when data is received from the connected device (UnitySendMessage callback)
    public void OnBLEDataReceived(string data)
    {
        Log($"Received data: {data}");
        OnDataReceived?.Invoke(data);
    }

    #endregion

    #region Helper Methods

    private void Log(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"[BLE] {message}");
        }
    }

    private void LogError(string message)
    {
        Debug.LogError($"[BLE] {message}");
    }

    #endregion

    #region Data Classes

    [System.Serializable]
    public class DeviceInfo
    {
        public string name;
        public string uuid;
        public int rssi;
    }

    #endregion
}