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
        // Only apply when building via CLI (run-build.sh or Unity -batchmode)
        if (!System.Environment.GetCommandLineArgs().Contains("-batchmode"))
        {
            Debug.Log("[BuildConfigurator] Skipping PlayerSettings override for UI build.");
            return;
        }

        Debug.Log("[BuildConfigurator] Applying iOS player settings...");

        if (report.summary.platform == BuildTarget.iOS)
        {
            PlayerSettings.applicationIdentifier = "com.dcflux.sawyer";
            PlayerSettings.iOS.appleEnableAutomaticSigning = true;
            PlayerSettings.iOS.targetOSVersionString = "13.0";
            PlayerSettings.iOS.scriptCallOptimization = ScriptCallOptimizationLevel.FastButNoExceptions;

            PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
            PlayerSettings.allowedAutorotateToLandscapeLeft = true;
            PlayerSettings.allowedAutorotateToLandscapeRight = true;
            PlayerSettings.allowedAutorotateToPortrait = false;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;

            PlayerSettings.SetScriptingBackend(BuildTargetGroup.iOS, ScriptingImplementation.IL2CPP);
            PlayerSettings.SetArchitecture(BuildTargetGroup.iOS, 1); // ARM64
            PlayerSettings.SetAdditionalIl2CppArgs("--emit-null-checks");
            PlayerSettings.apiCompatibilityLevel = ApiCompatibilityLevel.NET_Standard_2_0;
            PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.iOS, ManagedStrippingLevel.Minimal);
            PlayerSettings.stripEngineCode = false;

            Debug.Log("[BuildConfigurator] ✅ iOS PlayerSettings configured for IL2CPP");
        }
    }

    [MenuItem("Build/iOS Build to DCFLUX")]
    public static void BuildiOSProject()
    {
        var scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();

        if (scenes.Length == 0)
        {
            Debug.LogError("[BuildConfigurator] ❌ No scenes marked as enabled.");
            return;
        }

        string buildPath = "../SAWYER-iOS/DCFLUX/ios/Unity-iOS";

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
