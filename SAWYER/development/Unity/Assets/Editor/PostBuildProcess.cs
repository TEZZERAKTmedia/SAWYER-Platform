using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;
using System.IO;

public class PostBuildConfigurator
{
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string pathToBuiltProject)
    {
        if (buildTarget != BuildTarget.iOS)
            return;

        Debug.Log("🛠️ Starting Unity + RN iOS post-build configuration");

        string projPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);
        PBXProject proj = new PBXProject();
        proj.ReadFromFile(projPath);

#if UNITY_2019_3_OR_NEWER
        string mainTarget = proj.GetUnityMainTargetGuid();
        string unityFrameworkTarget = proj.GetUnityFrameworkTargetGuid();
#else
        string mainTarget = proj.TargetGuidByName("Unity-iPhone");
        string unityFrameworkTarget = proj.TargetGuidByName("UnityFramework");
#endif

        // Embed UnityFramework
        string frameworkPath = "Frameworks/UnityFramework.framework";
        string fileGuid = proj.FindFileGuidByProjectPath(frameworkPath);
        if (!string.IsNullOrEmpty(fileGuid))
        {
            proj.AddFileToEmbedFrameworks(mainTarget, fileGuid);
            Debug.Log("✅ Embedded UnityFramework");
        }

        // Add UnityFramework as a target dependency
        proj.AddTargetDependency(mainTarget, unityFrameworkTarget);
        Debug.Log("✅ Added UnityFramework target dependency");

        // Disable Bitcode
        proj.SetBuildProperty(mainTarget, "ENABLE_BITCODE", "NO");
        proj.SetBuildProperty(unityFrameworkTarget, "ENABLE_BITCODE", "NO");
        Debug.Log("✅ Disabled Bitcode");

        // Add Swift support
        proj.SetBuildProperty(mainTarget, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");
        proj.SetBuildProperty(unityFrameworkTarget, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");

        // Set Bridging Header (adjust the path if needed)
        proj.SetBuildProperty(mainTarget, "SWIFT_OBJC_BRIDGING_HEADER", "DCFLUX/BridgingHeader/DCFLUX-Bridging-Header.h");
        proj.SetBuildProperty(mainTarget, "DEFINES_MODULE", "YES");

        proj.WriteToFile(projPath);

        // Modify Info.plist for Bluetooth and Location
        string plistPath = Path.Combine(pathToBuiltProject, "Info.plist");
        PlistDocument plist = new PlistDocument();
        plist.ReadFromFile(plistPath);
        PlistElementDict rootDict = plist.root;

        rootDict.SetString("NSBluetoothAlwaysUsageDescription", "This app uses Bluetooth to connect to external devices.");
        rootDict.SetString("NSBluetoothPeripheralUsageDescription", "This app uses Bluetooth to connect to external devices.");
        rootDict.SetString("NSLocationWhenInUseUsageDescription", "This app needs location access to get WiFi network information.");
        rootDict.SetString("NSLocationAlwaysAndWhenInUseUsageDescription", "This app needs location access to get WiFi network information.");

        // Add background modes if not already there
        PlistElementArray bgModes;
        if (rootDict.values.ContainsKey("UIBackgroundModes"))
        {
            bgModes = rootDict["UIBackgroundModes"].AsArray();
        }
        else
        {
            bgModes = rootDict.CreateArray("UIBackgroundModes");
        }

        if (!bgModes.values.Exists(v => v.AsString() == "bluetooth-central"))
            bgModes.AddString("bluetooth-central");

        plist.WriteToFile(plistPath);
        Debug.Log("✅ Info.plist configured for BLE and location");

        Debug.Log("🎉 Post-build configuration complete");
    }
}
