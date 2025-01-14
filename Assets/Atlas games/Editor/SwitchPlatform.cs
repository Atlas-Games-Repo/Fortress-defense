using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Compilation;
using UnityEngine;
public delegate void Callback();
public class SwitchPlatform
{
    static bool isListening = false;
    
    static void BuildMac() {
        // Mac
        bool switched_1 = EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone , BuildTarget.StandaloneOSX);
        if (switched_1) {
            EditorUserBuildSettings.selectedStandaloneTarget = BuildTarget.StandaloneOSX;
            BuildAddressables();
        }
    }
    static void BuildWindow86() {
        // windows 86
        bool switched_2 = EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone , BuildTarget.StandaloneWindows);
        if (switched_2) {
            EditorUserBuildSettings.selectedStandaloneTarget = BuildTarget.StandaloneWindows;
            BuildAddressables();
        }
    }
    static void BuildAndroid() {
        // android
        bool switched_4 = EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android , BuildTarget.Android);
        if (switched_4) {
            BuildAddressables();
        }
    }
    static void BuildIOS() {
        // ios
        bool switched_6 = EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);
        if (switched_6) {
            BuildAddressables();
        }
    }
    static void BuildWebGL() {
        // webgl
        bool switched_5 = EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WebGL, BuildTarget.WebGL);
        if (switched_5) {
            BuildAddressables();
        }
    }
    static void BuildWindow64() {
        // windows 64
        bool switched_3 = EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone , BuildTarget.StandaloneWindows64);
        if (switched_3) {
            EditorUserBuildSettings.selectedStandaloneTarget = BuildTarget.StandaloneWindows64;
            BuildAddressables();
        }
    }

    public static void BuildAddressables(object o = null)
    {
        if (EditorApplication.isCompiling)
        {
            Debug.Log("Delaying until compilation is finished...");
     
            if(!isListening)
                CompilationPipeline.compilationFinished += BuildAddressables;
            isListening = true;
            return;
        }

        if(isListening)
            CompilationPipeline.compilationFinished -= BuildAddressables;

        //AddressableAssetSettingsDefaultObject.Settings = AddressableAssetSettings.Create();
        Debug.Log("Building Addressables!!! START PLATFORM: platform: " + Application.platform + " target: " + EditorUserBuildSettings.activeBuildTarget);
     
        AddressableAssetSettings.CleanPlayerContent();
        AddressableAssetSettings.BuildPlayerContent();
     
        Debug.Log("Building Addressables!!! DONE");
    }

    [MenuItem("Assets/Addressables/Build All")]
    static void BuildAll() {
        // Mac
        bool switched_1 = EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone , BuildTarget.StandaloneOSX);
        if (switched_1) {
            EditorUserBuildSettings.selectedStandaloneTarget = BuildTarget.StandaloneOSX;
            BuildAddressables();
        }
        // windows 86
        bool switched_2 = EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone , BuildTarget.StandaloneWindows);
        if (switched_2) {
            EditorUserBuildSettings.selectedStandaloneTarget = BuildTarget.StandaloneWindows;
            BuildAddressables();
        }
        // android
        bool switched_4 = EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android , BuildTarget.Android);
        if (switched_4) {
            BuildAddressables();
        }
        // ios
        bool switched_6 = EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);
        if (switched_6) {
            BuildAddressables();
        }
        // webgl
        bool switched_5 = EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WebGL, BuildTarget.WebGL);
        if (switched_5) {
            BuildAddressables();
        }
        // windows 64
        bool switched_3 = EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone , BuildTarget.StandaloneWindows64);
        if (switched_3) {
            EditorUserBuildSettings.selectedStandaloneTarget = BuildTarget.StandaloneWindows64;
            BuildAddressables();
        }
    }
}
