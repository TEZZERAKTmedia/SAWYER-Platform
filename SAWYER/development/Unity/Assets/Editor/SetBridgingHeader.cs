#if UNITY_IOS
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;

public class SetBridgingHeader
{
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (target != BuildTarget.iOS)
            return;

        var pbxProjectPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);
        var pbxProject = new PBXProject();
        pbxProject.ReadFromFile(pbxProjectPath);

#if UNITY_2019_3_OR_NEWER
        var targetGuid = pbxProject.GetUnityMainTargetGuid();
#else
        var targetGuid = pbxProject.TargetGuidByName(PBXProject.GetUnityTargetName());
#endif

        // Path relative to Xcode project root *after export*
        string bridgingHeaderPath = "Libraries/Plugins/iOS/Unity-iPhone-Bridging-Header.h";
        pbxProject.SetBuildProperty(targetGuid, "SWIFT_OBJC_BRIDGING_HEADER", bridgingHeaderPath);

        pbxProject.WriteToFile(pbxProjectPath);
    }
}
#endif
