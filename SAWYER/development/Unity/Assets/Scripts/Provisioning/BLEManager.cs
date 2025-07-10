using System;
using UnityEngine;
using System.Runtime.InteropServices;

public class BLEManager : MonoBehaviour
{
    // --- EVENTS for UI to subscribe to ---
    public event Action<string,string,int> OnDeviceFound;
    public event Action<string> OnWiFiListReceived;

#if UNITY_IOS
    [DllImport("__Internal")]
    private static extern void StartBLEScan();
    [DllImport("__Internal")]
    private static extern void StopBLEScan();
    [DllImport("__Internal")]
    private static extern void ConnectToPeripheral(string uuid);
    [DllImport("__Internal")]
    private static extern void WriteToCharacteristic(string data);
#endif

    // --- UI calls these ---

    public void StartScan()
    {
#if UNITY_IOS
        StartBLEScan();
#else
        // Editor simulation
        Invoke(nameof(SimulateScan), 1f);
#endif
    }

    public void StopScan()
    {
#if UNITY_IOS
        StopBLEScan();
#endif
    }

    public void SendPIN(string pin)
    {
#if UNITY_IOS
        WriteToCharacteristic(pin);
#endif
    }

    public void SendWiFi(string ssid, string password)
    {
#if UNITY_IOS
        WriteToCharacteristic($"{ssid}|{password}");
#endif
    }

    // --- Called by your iOS plugin via UnitySendMessage ---

    // UnitySendMessage("BLEManager", "DeviceFoundCallback", name + "|" + uuid + "|" + rssi);
    public void DeviceFoundCallback(string payload)
    {
        var parts = payload.Split('|');
        if (parts.Length == 3 && int.TryParse(parts[2], out var rssi))
            OnDeviceFound?.Invoke(parts[0], parts[1], rssi);
    }

    // UnitySendMessage("BLEManager", "WiFiListReceivedCallback", jsonString);
    public void WiFiListReceivedCallback(string json)
    {
        OnWiFiListReceived?.Invoke(json);
    }

    // --- Editor-only simulation ---
    private void SimulateScan()
    {
        OnDeviceFound?.Invoke("TestDevice", Guid.NewGuid().ToString(), -55);
    }
}
