#if UNITY_IOS
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;

public static class PostBuildConfigurator
{
    // ensure this runs after Unity’s own PostProcessBuild steps
    [PostProcessBuild(999)]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string pathToBuiltProject)
    {
        if (buildTarget != BuildTarget.iOS)
            return;

        // load the project
        var projPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);
        var proj     = new PBXProject();
        proj.ReadFromFile(projPath);

        // figure out both target GUIDs
        string appTarget, frameworkTarget;
        
    #if UNITY_2019_3_OR_NEWER
        appTarget        = PBXProject.GetUnityMainTargetGuid();
        frameworkTarget  = proj.GetUnityFrameworkTargetGuid();
    #else
        appTarget        = proj.TargetGuidByName("Unity-iPhone");
        frameworkTarget  = proj.TargetGuidByName("UnityFramework");
    #endif

        // relative paths inside the generated Xcode project
        const string BRIDGING_HEADER = "Libraries/Plugins/iOS/Unity-iPhone-Bridging-Header.h";
        const string HEADER_PATH     = "$(SRCROOT)/Libraries/Plugins/iOS";
        const string SWIFT_VERSION   = "5.0";

        // 1) Configure the **main app** target:
        proj.SetBuildProperty(appTarget,       "SWIFT_OBJC_BRIDGING_HEADER", BRIDGING_HEADER);
        proj.SetBuildProperty(appTarget,       "SWIFT_VERSION",              SWIFT_VERSION);
        proj.AddBuildProperty(appTarget,       "HEADER_SEARCH_PATHS",        HEADER_PATH);

        // 2) Configure the UnityFramework target:
        //    (bridging headers on frameworks are unsupported, so we only add search paths + Swift version)
        proj.AddBuildProperty(frameworkTarget, "HEADER_SEARCH_PATHS",        HEADER_PATH);
        proj.SetBuildProperty(frameworkTarget, "SWIFT_VERSION",              SWIFT_VERSION);

        // write it back out
        File.WriteAllText(projPath, proj.WriteToString());

        UnityEngine.Debug.Log(
            $"[PostBuild] Set up Swift/ObjC interop:\n" +
            $"- App target ('Unity-iPhone') bridging header: {BRIDGING_HEADER}\n" +
            $"- Added HEADER_SEARCH_PATHS to both targets\n" +
            $"- Swift version set to {SWIFT_VERSION}"
        );
    }
}
#endif
