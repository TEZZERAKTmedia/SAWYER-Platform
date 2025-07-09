using System;
using UnityEngine;

[Serializable]
public class WiFiNetwork
{
    public string ssid;
    public int signal;
}

[Serializable]
public class WiFiNetworkList
{
    public WiFiNetwork[] networks;
}

[Serializable]
public class WifiCredentials
{
    public string ssid;
    public string password;
}

[Serializable]
public class BluetoothDevice
{
    public string name;
    public string uuid;
    public int rssi;
}
