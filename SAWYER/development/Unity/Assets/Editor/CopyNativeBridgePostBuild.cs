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

#if UNITY_IOS
        string projPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);
        PBXProject proj = new PBXProject();
        proj.ReadFromFile(projPath);

        string mainTarget = proj.GetUnityMainTargetGuid();           // Unity-iPhone
        string unityFrameworkTarget = proj.GetUnityFrameworkTargetGuid(); // UnityFramework

        // ✅ Set Swift & build flags
        string[] targets = { mainTarget, unityFrameworkTarget };
        foreach (var target in targets)
        {
            proj.SetBuildProperty(target, "SWIFT_VERSION", "5.0");
            proj.SetBuildProperty(target, "ENABLE_BITCODE", "NO");
            proj.SetBuildProperty(target, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");
            proj.SetBuildProperty(target, "DEFINES_MODULE", "YES");
        }

        // ✅ Link Swift source files without copying
        string nativeBridgeDir = Path.GetFullPath(Path.Combine(Application.dataPath, "../../SAWYER-iOS/DCFLUX/NativeBridge"));
        if (!Directory.Exists(nativeBridgeDir))
        {
            Debug.LogWarning($"[NativeBridge] Directory not found: {nativeBridgeDir}");
        }
        else
        {
            foreach (string filePath in Directory.GetFiles(nativeBridgeDir, "*.swift"))
            {
                string fileName = Path.GetFileName(filePath);
                string relativePath = Path.Combine("NativeBridge", fileName);

                string fileGuid = proj.AddFile(filePath, relativePath, PBXSourceTree.Source);
                proj.AddFileToBuild(mainTarget, fileGuid);
            }

            Debug.Log("[NativeBridge] ✅ Linked NativeBridge Swift files into Unity-iPhone target.");
        }

        proj.WriteToFile(projPath);
#endif
    }
}
