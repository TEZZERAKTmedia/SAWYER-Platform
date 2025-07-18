using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.Linq;

public class BuildConfigRN_Unity : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        Debug.Log("[BuildConfigurator] Applying iOS player settings...");

        if (report.summary.platform == BuildTarget.iOS)
        {
            // ✅ Build as UnityFramework
            EditorUserBuildSettings.iOSBuildType = iOSBuildType.Framework;

            // ✅ Bundle Identifier
            PlayerSettings.applicationIdentifier = "com.yourcompany.dcflux";

            // ✅ iOS-specific settings
            PlayerSettings.iOS.appleEnableAutomaticSigning = true;
            PlayerSettings.iOS.targetOSVersionString = "13.0";
            PlayerSettings.iOS.scriptCallOptimization = ScriptCallOptimizationLevel.FastButNoExceptions;

            // ✅ Lock orientation to Landscape only
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
            PlayerSettings.allowedAutorotateToLandscapeLeft = true;
            PlayerSettings.allowedAutorotateToLandscapeRight = true;
            PlayerSettings.allowedAutorotateToPortrait = false;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;

            // ✅ Scripting backend and architecture
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.iOS, ScriptingImplementation.IL2CPP);
            PlayerSettings.SetArchitecture(BuildTargetGroup.iOS, 1); // 1 = ARM64

            // ✅ IL2CPP and API settings
            PlayerSettings.SetAdditionalIl2CppArgs("--emit-null-checks");
            PlayerSettings.apiCompatibilityLevel = ApiCompatibilityLevel.NET_Standard_2_0;

            // ✅ Stripping and engine settings
            PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.iOS, ManagedStrippingLevel.Minimal);
            PlayerSettings.stripEngineCode = false;
        }
    }

    // ✅ Manual build method if you want to trigger via script or terminal
    [MenuItem("Build/iOS Build to DCFLUX")]
    public static void BuildiOSProject()
    {
        var scenes = EditorBuildSettings.scenes
            .Where(s => s.enabled)
            .Select(s => s.path)
            .ToArray();

        if (scenes.Length == 0)
        {
            Debug.LogError("[BuildConfigurator] ❌ No scenes are marked as enabled in Build Settings.");
            return;
        }

        string buildPath = "../SAWYER-iOS/DCFLUX/ios";

        Debug.Log("[BuildConfigurator] 🚀 Starting iOS framework build...");
        Debug.Log("[BuildConfigurator] 📦 Output Path: " + buildPath);

        BuildReport report = BuildPipeline.BuildPlayer(
            scenes,
            buildPath,
            BuildTarget.iOS,
            BuildOptions.None
        );

        if (report.summary.result == BuildResult.Succeeded)
            Debug.Log("[BuildConfigurator] ✅ Build succeeded!");
        else
            Debug.LogError("[BuildConfigurator] ❌ Build failed.");
    }
}
