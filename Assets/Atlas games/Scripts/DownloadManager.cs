using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;
using System;
using System.IO;

public class DownloadManager : MonoBehaviour
{
    public string CONNECTING, CHECKING_FOR_DOWNLOAD, DOWNLOADING;
    public Slider downloadSlider;
    public TextMeshProUGUI downloading;
    public string pattern = @"/(.[a-zA-Z0-9]+.assetbundle)";
    private int itter = 0;

    public async void Start()
    {
        downloading.text = CONNECTING;
        itter = 0;
        await Task.Delay(100);
        AssetBundleUpdateResponse assets = null;
        try
        {
            assets = await APIManager.instance.Check_for_updates();
        }
        catch (System.Exception)
        {
            APIManager.instance.RunStatus(NetworkStatusError.UNKNOWN_ERROR, APIManager.instance.ErrorColor);
            User.Token = "";
            await Task.Delay(3 * 1000);
            APIManager.instance.LoadAsynchronously("Login");
        }
        if (assets.list.Length <= 0)
            Application.Quit();
        var progress = new Progress<float>(value =>
        {
            downloadSlider.value = (value + (float)itter) / (float)assets.list.Length;
            downloading.text = DOWNLOADING;
        });
        downloading.text = CHECKING_FOR_DOWNLOAD;
        RegexOptions options = RegexOptions.Multiline;
        Add_target_platform(ref pattern);
        foreach (var item in assets.list)
        {
            itter++;
            string file_path = "";
            foreach (Match m in Regex.Matches(item, pattern, options))
            {
                file_path = m.Groups[1].Value;
            }
            if (file_path == "") continue;
            if (!File.Exists(APIManager.instance.GetFilePath(file_path)))
            {
                await APIManager.instance.DownloadUpdate(file_path, item, progress);
            }
        }
        StartCoroutine(APIManager.instance.LoadAsynchronously());
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
#if UNITY_STANDALONE && UNITY_64
        pattern = pattern.Insert(index, "StandaloneWindows64");
#endif
    }

}
