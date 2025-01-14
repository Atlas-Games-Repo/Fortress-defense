using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

public class AssetBundleManager : MonoBehaviour
{
    public AssetBundleGenerator[] bundles;
    //public ScriptableObject asd;

    async void Awake()
    {
        await LoadDownloadedAssets();
    }
    async Task LoadDownloadedAssets()
    {

        await Task.Delay(1);
        List<Task> tasks = new List<Task>(bundles.Length);
        for (int i = 0; i < bundles.Length; i++)
        {
            tasks.Add(LoadFromFile(i));
        }
        await Task.WhenAll(tasks);
    }
    public async Task LoadFromFile(int i)
    {
        string bundle_name = bundles[i].name;
        Add_target_platform(ref bundle_name, 0);
        var updates = await APIManager.instance.Check_for_updates(type: bundle_name);
        string file_name = Address_to_name(updates.list[0]);
        if (file_name == "") return;
        if (File.Exists(APIManager.instance.GetFilePath(file_name)))
        {
            AssetBundleCreateRequest bundle = AssetBundle.LoadFromFileAsync(APIManager.instance.GetFilePath(file_name));

            while (!bundle.isDone)
            {
                await Task.Yield();
            }
            foreach (var item in bundle.assetBundle.LoadAllAssets<GameObject>())
            {
                GameObject obj;
                if (bundles[i].is_root_world)
                {
                    obj = Instantiate(item);
                    obj.name = item.name;
                }
                else
                {
                    obj = Instantiate(item, bundles[i].root, false);
                    obj.name = item.name;
                }

                if (bundles[i].has_depends)
                {
                    bundles[i].depends_type.SendMessage("assetBundleDepends", obj);
                }
            }
            bundle.assetBundle.Unload(false);
        }
        else
        {
            Application.Quit(); // TODO: Show an proper error and than exit the game
        }
    }
    public string Address_to_name(string address)
    {
        string file_name = "";
        RegexOptions options = RegexOptions.Multiline;
        string pattern = @"/(.[a-zA-Z0-9]+.assetbundle)";
        Add_target_platform(ref pattern);
        foreach (Match m in Regex.Matches(address, pattern, options))
        {
            file_name = m.Groups[1].Value;
        }
        return file_name;
    }
    public void Add_target_platform(ref string pattern, int index = 2)
    {
#if UNITY_ANDROID
        pattern = pattern.Insert(index, "Android");
#endif
#if UNITY_WEBGL
        pattern = pattern.Insert(index, "WebGL");
#endif
#if UNITY_STANDALONE_OSX
        pattern = pattern.Insert(index, "StandaloneOSX");
#endif
#if UNITY_IOS
        pattern = pattern.Insert(index, "iOS");
#endif
#if UNITY_STANDALONE_WIN && !UNITY_64 
        pattern = pattern.Insert(index, "StandaloneWindows");
#endif
#if UNITY_STANDALONE_WIN && UNITY_64
        pattern = pattern.Insert(index, "StandaloneWindows64");
#endif
    }

}
