using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;
using System.IO;

public static class AutoBuildHookEditor
{
    [MenuItem("Build/Build iOS App")]
    public static void BuildiOSApp()
    {
        string buildPath = "../../Application/MainUI/Frameworks/Editor";

        if (Directory.Exists(buildPath))
        {
            Debug.Log($"[Build] Cleaning up previous build at: {buildPath}");
            Directory.Delete(buildPath, true);
        }

        Directory.CreateDirectory(buildPath);

        PlayerSettings.applicationIdentifier = "com.sawyer.editor.integrated";
        PlayerSettings.companyName = "Techtonic-Robotics";
        PlayerSettings.productName = "Editor Modular";
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
            "Assets/Scenes/Editor.unity",
        };

        BuildPlayerOptions buildOptions = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = buildPath,
            target = BuildTarget.iOS,
            options = BuildOptions.None
        };

        Debug.Log($"[Build] Starting iOS build to: {Path.GetFullPath(buildPath)}");
        BuildReport report = BuildPipeline.BuildPlayer(buildOptions);

        if (report.summary.result == BuildResult.Succeeded)
        {
            Debug.Log("iOS Editor app build, Successful!");
            Debug.Log($"Output location: {Path.GetFullPath(buildPath)}");
        }
    }
}