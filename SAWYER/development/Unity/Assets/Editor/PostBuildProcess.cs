using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;

public class BLEPostProcessBuild
{
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string pathToBuiltProject)
    {
        if (buildTarget == BuildTarget.iOS)
        {
            string plistPath = pathToBuiltProject + "/Info.plist";
            PlistDocument plist = new PlistDocument();
            plist.ReadFromString(File.ReadAllText(plistPath));
            
            PlistElementDict rootDict = plist.root;
            
            // Add Bluetooth permissions
            rootDict.SetString("NSBluetoothAlwaysUsageDescription", 
                "This app uses Bluetooth to connect to external devices");
            rootDict.SetString("NSBluetoothPeripheralUsageDescription", 
                "This app uses Bluetooth to connect to external devices");
            
            // Add Location permissions (required for WiFi SSID access on iOS 14+)
            rootDict.SetString("NSLocationWhenInUseUsageDescription", 
                "This app needs location access to get WiFi network information");
            rootDict.SetString("NSLocationAlwaysAndWhenInUseUsageDescription", 
                "This app needs location access to get WiFi network information");
            
            // Add background modes for Bluetooth
            PlistElementArray backgroundModes = rootDict.CreateArray("UIBackgroundModes");
            backgroundModes.AddString("bluetooth-central");
            
            // Write the modified plist back
            File.WriteAllText(plistPath, plist.WriteToString());
            
            Debug.Log("BLE: Added Info.plist entries for Bluetooth and Location permissions");
        }
    }
}