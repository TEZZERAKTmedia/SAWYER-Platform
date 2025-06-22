using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;
using System.IO;

public static class AutoBuildHookARMapping
{
    [MenuItem("Build/Build iOS App")]
    public static void BuildIOSApp()
    {
        string buildPath = "../../Builds/AR-Mapping";

        if (Directory.Exists(buildPath))  
            Directory.Delete(buildPath, true);
        Directory.CreateDirectory(buildPath);

        PlayerSettings.applicationIdentifier = "com.sawyer.ar.mapping";
        PlayerSettings.companyName = "Sawyer";
        PlayerSettings.productName = "AR Mapping";
        PlayerSettings.bundleVersion = "1.0.0";
        PlayerSettings.iOS.buildNumber = "1";
        PlayerSettings.iOS.sdkVersion = iOSSdkVersion.DeviceSDK;
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.iOS, ScriptingImplementation.IL2CPP);
        PlayerSettings.SetArchitecture(BuildTargetGroup.iOS, 1);
        
        BuildPlayerOptions buildOptions = new BuildPlayerOptions
        {
            scenes = new[] { "Assets/Scenes/mapping-ar-save.unity"},
            locationPathName = buildPath,
            target = BuildTarget.iOS,
            options = BuildOptions.none,
        };
        BuildReport report = BuildPipeline.BuildPlayer(buildOptions);
        Debug.Log("✅ Build completed with result: " + report.summary.result);
    }
}