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
    }
}
