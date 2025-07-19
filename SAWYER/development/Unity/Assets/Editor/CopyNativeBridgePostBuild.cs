using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using System.IO;

#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

public class CopyNativeBridgePostBuild
{
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string pathToBuiltProject)
    {
        if (buildTarget != BuildTarget.iOS)
            return;

#if UNITY_IOS
        Debug.Log("⚙️ Post-build script running...");

        string projPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);
        PBXProject proj = new PBXProject();
        proj.ReadFromFile(projPath);

        string frameworkTarget = proj.GetUnityFrameworkTargetGuid();

        proj.SetBuildProperty(frameworkTarget, "SWIFT_VERSION", "5.0");
        proj.SetBuildProperty(frameworkTarget, "ENABLE_BITCODE", "NO");
        proj.SetBuildProperty(frameworkTarget, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");
        proj.SetBuildProperty(frameworkTarget, "DEFINES_MODULE", "YES");

        // ⚠️ Your actual Swift source file
        string nativeBridgeDir = Path.Combine(Directory.GetCurrentDirectory(), "../SAWYER-iOS/DCFLUX/NativeBridge");
        string[] swiftFiles = Directory.GetFiles(nativeBridgeDir, "*.swift");

        foreach (var file in swiftFiles)
        {
            string fileName = Path.GetFileName(file);
            string fullPath = Path.GetFullPath(file);
            string relativePath = Path.Combine("..", "..", "SAWYER-iOS", "DCFLUX", "NativeBridge", fileName);

            Debug.Log($"📎 Linking Swift file: {relativePath}");

            proj.AddFileToBuild(frameworkTarget, proj.AddFile(relativePath, fileName, PBXSourceTree.Source));
        }

        proj.WriteToFile(projPath);

        Debug.Log("✅ NativeBridge files linked successfully.");
#endif
    }
}
