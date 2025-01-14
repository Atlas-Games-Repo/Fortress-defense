using UnityEditor;
using UnityEngine;
using System.IO;

public class CreateAssetBundles
{
    [MenuItem("Assets/Build Assetbundles")]
    static void BuildAllAssetBundles()
    {
        BuildTarget[] targets = new BuildTarget[6] { BuildTarget.iOS, BuildTarget.StandaloneOSX, BuildTarget.WebGL, BuildTarget.StandaloneWindows, BuildTarget.StandaloneWindows64, BuildTarget.Android };
        foreach (BuildTarget target in targets)
        {
            string assetBundleDirectory = $"Assets/StreamingAssets/{target}";

            if (!Directory.Exists(assetBundleDirectory))
            {
                Directory.CreateDirectory(assetBundleDirectory);
            }
            BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, target);
            AssetDatabase.Refresh();
        }
    }
}