#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using UnityEngine;

public class CopyNativeBridgePostBuild
{
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string pathToBuiltProject)
    {
        if (buildTarget != BuildTarget.iOS)
            return;

        string unityBridgePath = Path.Combine(Application.dataPath, "Plugins/iOS/NativeBridge");
        string targetBridgePath = Path.Combine(pathToBuiltProject, "NativeBridge");

        if (!Directory.Exists(unityBridgePath))
        {
            Debug.LogWarning("[NativeBridge] No NativeBridge folder found in Unity project.");
            return;
        }

        Directory.CreateDirectory(targetBridgePath);
        foreach (var file in Directory.GetFiles(unityBridgePath))
        {
            var fileName = Path.GetFileName(file);
            File.Copy(file, Path.Combine(targetBridgePath, fileName), overwrite: true);
        }

        Debug.Log("[NativeBridge] ✅ Copied Swift bridge files to Xcode iOS project.");

        string projPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);
        PBXProject proj = new PBXProject();
        proj.ReadFromFile(projPath);

#if UNITY_2019_3_OR_NEWER
        string targetGuid = proj.GetUnityMainTargetGuid();
        string frameworkGuid = proj.GetUnityFrameworkTargetGuid();
#else 
        string targetGuid = proj.TargetGuidByName("Unity-iPhone");
        string frameworkGuid = targetGuid;
#endif 
        foreach (var file in Directory.GetFiles(targetBridgePath, "*.swift"))
        {
            var relativePath = "NativeBridge/" + Path.GetFileName(file);
            var fileGuid = proj.AddFile(file, relativePath, PBXSourceTree.Source);
            proj.AddFileToBuild(frameworkGuid, fileGuid);
        }

        proj.SetBuildProperty(frameworkGuid, "SWIFT_VERSION", "5.0");
        proj.SetBuildProperty(frameworkGuid, "ENABLE_BITCODE", "NO");
        proj.SetBuildProperty(frameworkGuid, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");
        proj.SetBuildProperty(frameworkGuid, "DEFINES_MODULE", "YES");


        proj.WriteToFile(projPath);

        Debug.Log("[NativeBridge] Linked Swift bridge files into Unity-iPhone.xcodeproj.");

        
    }
}
