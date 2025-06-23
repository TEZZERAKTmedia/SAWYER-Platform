using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.Build.Reporting;


public static class AutoBuildHookUI
{
    [MenuItem("Build/Build Main UI")]
    public static void BuildiOSApp()
    {
        string buildPath = "../Builds/MainUI";

        if (Directory.Exists(buildPath))
        {
            Debug.Log($"[Build] Cleaning up previous build at: {buildPath}");
            Directory.Delete(buildPath, true);
        }

        Directory.CreateDirectory(buildPath);

        PlayerSettings.applicationIdentifier = "com.techtonic.sawyer.mainui";
        PlayerSettings.companyName = "Techtonic-Robotics";
        PlayerSettings.productName = "Main UI";
        PlayerSettings.bundleVersion = "1.0.0";
        PlayerSettings.iOS.buildNumber = "1";
        PlayerSettings.iOS.sdkVersion = iOSSdkVersion.DeviceSDK;
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.iOS, ScriptingImplementation.IL2CPP);
        PlayerSettings.SetArchitecture(BuildTargetGroup.iOS, 1);

        // Prevent portrait mode
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
        PlayerSettings.allowedAutorotateToPortrait = false;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
        PlayerSettings.allowedAutorotateToLandscapeRight = true;
        PlayerSettings.allowedAutorotateToLandscapeLeft = true;
        PlayerSettings.iOS.requiresFullScreen = true;

        
        
        string[] scenes = new string[]
        {
            "Assets/Scenes/MainUI.unity",
        };

        BuildPlayerOptions buildOptions = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = buildPath,
            target = BuildTarget.iOS,
            options = BuildOptions.None
        };

        Debug.Log($"[Build] Starting Main UI build to: {Path.GetFullPath(buildPath)}");
        BuildReport report = BuildPipeline.BuildPlayer(buildOptions);

        if (report.summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Main UI app build, Successful!");
            Debug.Log($"Output location: {Path.GetFullPath(buildPath)}");
        }
    }
}