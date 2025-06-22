using UnityEditor;
using UnityEditor.Build.Reporting;
using System.IO;
using UnityEngine;


public static class BuildHandleEditoriOS 
{
    [MenuItem("Build/Build iOS App")]
    public static void BuildIOSApp()
    {
        string buildPath = "../../Builds/Editor";

        if (Directory.Exists(buildPath))
        {
            Debug.Log($"[Build] Cleaning up previous build at: {buildPath}");
            Directory.Delete(buildPath, true);
        }

        Directory.CreateDirectory(buildPath);

        PlayerSettings.applicationIdentifier = "com.sawyer.editor.modular";
        PlayerSettings.companyName = "Techtonic-Robotics";
        PlayerSettings.productName = "Editor Modular";
        PlayerSettings.bundleVersion = "1.0.0";
        PlayerSettings.iOS.buildNumber = "1";
        PlayerSettings.iOS.sdkVersion = iOSSdkVersion.DeviceSDK;
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.iOS, ScriptingImplementation.IL2CPP);
        PlayerSettings.SetArchitecture(BuildTargetGroup.iOS, 1);
        //Prevent portrait mode
        // Set the default orientation to landscape left
        PlayerSettings.DefaultInterfaceOrientation = UIOrientation.LandscapeLeft;
        PlayerSettings.allowedAutorotateToPortrait = false;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
        PlayerSettings.allowedAutorotateToLandscapeRight = true; // If you want to support both
        PlayerSettings.allowedAutorotateToLandscapeLeft = true;


        string[] scenes = new string[]
        {
            "Assets/Scenes/Editor/Editor_Unity.unity",
        };

        BuildPlayerOptions buildOptions = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = buildPath,
            target = BuildTarget.iOS,
            options = BuildOptions.None
        }

        Debug.Log($"[Build] Starting iOS build to: {Path.GetFullPath(buildPath)}");
        BuildReport report = BuildPipeline.BuildPlayer(buildOptions);

        if (report.summary.result == BuildResult.Succeeded)
        {
            Debug.Log("iOS Editor app build, Successful!");
            Debug.Log($"Output location: {Path.GetFullPath(buildPath)}");

        }
        else 
        {
            Debug.LogError(" iOS Editor app build, Failed!");
            Debug.LogError("Result: " + report.summary.result):
        }
        {

        }
    }
}