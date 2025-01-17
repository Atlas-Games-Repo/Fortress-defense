using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using UnityEngine.Android;

public class APIManager : MonoBehaviour
{
    public static APIManager instance;
    public string BASE_URL = "https://hokm-url.herokuapp.com", assetbundle_dir = "DownloadedBundles";
    public static readonly string GAME_ID = "442";
    public bool IS_DEBUG = true;
    public string DEBUG_BASE_URL = "http://localhost:8080";
    public GameObject status;
    public float status_destroy;
    CancellationTokenSource tokenSource;
    public Color ErrorColor, WarningColor, CoolColor;
    [ReadOnly] public LifeTTR lifeTTR;

    private string pattern = @"{.*}", patternList = @"\[.*\]";

    public int lifeTTL = 30;
    public int maxLife = 5;
    public void Awake()
    {
        instance = this;
        lifeTTR = new LifeTTR(lifeTTL, maxLife);
        lifeTTR.Inintilize();
        tokenSource = new CancellationTokenSource();
        DontDestroyOnLoad(gameObject);
        UnityWebRequest.ClearCookieCache();
        Application.targetFrameRate = 144;
#if UNITY_ANDROID || UNITY_IPHONE
        bool is_read = Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead);
        bool is_write = Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite);
        if (!is_read || !is_write)
            Permission.RequestUserPermissions(new string[2]{Permission.ExternalStorageRead, Permission.ExternalStorageWrite});
#endif
    }
    public IEnumerator LoadAsynchronously(string name)
    {
        yield return new WaitForSeconds(0.01f);
        AsyncOperation operation = SceneManager.LoadSceneAsync(name);
        while (!operation.isDone)
        {
            _ = Mathf.Clamp01(operation.progress / 0.9f);
            yield return null;
        }
    }
    // Start is called before the first frame update
    public async void RunStatus(string message, Color? color = null)
    {
        Transform root = GameObject.FindGameObjectWithTag("Canves").transform;
        GameObject obj = Instantiate(status, root, false);
        obj.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = message;
        if (color != null)
            obj.GetComponent<Image>().color = (Color)color;
        await DestroyDelay(obj, status_destroy);
    }
    public async Task DestroyDelay(GameObject obj, float delay)
    {
        await Task.Delay((int)delay * 1000);
        DestroyImmediate(obj);
    }
    public IEnumerator LoadAsynchronously()
    {
        yield return new WaitForSeconds(0.01f);
        AsyncOperation operation = SceneManager.LoadSceneAsync("Logo atlas");
        while (!operation.isDone)
        {
            yield return null;
        }
    }
    public void OnDisable()
    {
        tokenSource.Cancel();
    }

    #region Public API Client
    public async Task<AssetBundleUpdateResponse> Check_for_updates(string type = null)
    {
        string param = type != null ? $"?type={type}" : "";
        return await Get<AssetBundleUpdateResponse>(route: $"/updates{param}", auth_token: User.Token);
    }
    public async Task Updates_achivement(int status = 0, string id = "0")
    {
        string param = new AchievementUpdateModel(_id: id, _status: status).ToParams;
        await Get<object>(route: "/achivements/add", auth_token: User.Token, parameters: param);
    }
    public async Task<AchievementModel[]> Get_achivements()
    {
        return await Get<AchievementModel[]>(route: "/achivements", auth_token: User.Token);
    }
    public async Task DownloadUpdate(string name, string address, IProgress<float> progress)
    {
        await GetAssetBundle(name, address, progress);
    }
    public async Task<AuthenticationResponse> Authenticate(Authentication auth)
    {
        AuthenticationResponse res = await Get<AuthenticationResponse>(route: "/user/login", parameters: auth.ToParams);
        RunStatus(res.message, CoolColor);
        return res;
    }
    public async Task<UserResponse> Check_token()
    {
        UserResponse res = await Get<UserResponse>(route: "/user/details", auth_token: User.Token);
        return res;
    }

    public async Task<UserResponse> UpdateUser(UserUpdate userdata)
    {
        return await Get<UserResponse>(
        route: "/user/update",
        parameters: userdata.ToParams,
        auth_token: User.Token);
    }

    public async Task<Sprite> Get_rofile_picture(string url)
    {
        Texture2D texture = await GetTexture(url);
        Rect rec = new Rect(0, 0, texture.width, texture.height);
        return Sprite.Create(texture, rec, new Vector2(0, 0), 1);
    }

    public async Task<GemResponseModel> Request_Gem(GemRequestModel parames = null)
    {
        return await Get<GemResponseModel>(
        route: "/user/gem",
        parameters: parames == null ? new GemRequestModel().ToParams : parames.ToParams,
        auth_token: User.Token);
    }
    #endregion

    #region Private API calls
    private async Task GetAssetBundle(string name,
                                      string adress,
                                      IProgress<float> progress,
                                      Dictionary<string, string> headers = null)
    {
        if (headers == null)
        {
            headers = new Dictionary<string, string>();
        }
        string filepath = GetFilePath(name);
        // if (File.Exists(filepath))
        // {
        //     throw new Exception(message: "file already downloaded");
        // }
        using UnityWebRequest req = UnityWebRequestAssetBundle.GetAssetBundle($"{adress}");
        foreach (KeyValuePair<string, string> item in headers)
        {
            req.SetRequestHeader(item.Key, item.Value);
        }

        var dh = new DownloadHandlerFile(filepath);
        dh.removeFileOnAbort = true;
        req.downloadHandler = dh;
        var opration = req.SendWebRequest();

        while (!opration.isDone)
        {
            float _progress = Mathf.Clamp01(opration.progress / 0.9f);
            progress.Report(_progress);
            await Task.Yield();
        }

        if (req.responseCode != 200)
        {
            throw new System.Net.WebException(message: req.error);
        }
        else
        {

            if (tokenSource.IsCancellationRequested)
            {
                throw new System.Exception(message: "Task cancelled");
            }

        }
    }
    private async Task<T> Get<T>(string route, string parameters = null, Dictionary<string, string> headers = null, string auth_token = "null")
    {
        if (headers == null)
            headers = new Dictionary<string, string>();
        if (auth_token == "")
            auth_token = "null";
        using UnityWebRequest req = UnityWebRequest.Get(Base_url + route + parameters);
        foreach (KeyValuePair<string, string> item in headers)
        {
            req.SetRequestHeader(item.Key, item.Value);
        }
        req.SetRequestHeader("Authorization", $"Bearer {auth_token}");
        // Set the request headers
        req.SetRequestHeader("Content-Type", "application/json");
        req.SetRequestHeader("Accept", "application/json");
        var opration = req.SendWebRequest();

        while (!opration.isDone)
        {
            await Task.Yield();
        }
        if (req.responseCode != 200)
        {
            CommonErrorResponse error_response = null;
            try
            {
                error_response = JsonUtility.FromJson<CommonErrorResponse>(Clean_json(req.downloadHandler.text));
            }
            catch (System.Exception)
            {
                RunStatus(req.error, ErrorColor);
                throw;
            }
            RunStatus(req.error);
            throw new System.Net.WebException(message: req.error);
        }
        else
        {
            T res;
            try
            {
                // res = JsonUtility.FromJson<T>(Clean_json(req.downloadHandler.text));
                res = JsonConvert.DeserializeObject<T>(Clean_json(req.downloadHandler.text));
            }
            catch (System.Exception e)
            {
                RunStatus(e.Message, ErrorColor);
                throw;
            }
            if (tokenSource.IsCancellationRequested)
            {
                throw new System.Exception(message: "Task cancelled");
            }
            return res;
        }
    }
    private async Task<T> Post<T>(string route, string data = null, Dictionary<string, string> headers = null, string auth_token = "null")
    {
        if (headers == null)
        {
            headers = new Dictionary<string, string>();
        }
        using UnityWebRequest req = UnityWebRequest.Post(uri: Base_url + route, postData: data);
        foreach (KeyValuePair<string, string> item in headers)
        {
            req.SetRequestHeader(item.Key, item.Value);
        }
        req.SetRequestHeader("Authorization", $"Bearer {auth_token}");
        // Set the request headers
        req.SetRequestHeader("Content-Type", "application/json");
        req.SetRequestHeader("Accept", "application/json");
        var opration = req.SendWebRequest();

        while (!opration.isDone)
        {
            await Task.Yield();
        }
        if (req.responseCode != 200)
        {
            CommonErrorResponse error_response = null;
            try
            {
                error_response = JsonUtility.FromJson<CommonErrorResponse>(Clean_json(req.downloadHandler.text));
            }
            catch (System.Exception)
            {
                RunStatus(req.error, ErrorColor);
                throw;
            }
            RunStatus(req.error);
            throw new System.Net.WebException(message: req.error);
        }
        else
        {
            T res;
            try
            {
                // res = JsonUtility.FromJson<T>(Clean_json(req.downloadHandler.text));
                res = JsonConvert.DeserializeObject<T>(Clean_json(req.downloadHandler.text));
            }
            catch (System.Exception)
            {
                throw;
            }
            if (tokenSource.IsCancellationRequested)
            {
                throw new System.Exception(message: "Task cancelled");
            }
            return res;

        }
    }
    #endregion
    public async Task<Texture2D> GetTexture(string url)
    {
        using UnityWebRequest req = UnityWebRequestTexture.GetTexture(uri: url);
        var opration = req.SendWebRequest();
        while (!opration.isDone)
        {
            await Task.Yield();

        }
        if (req.responseCode != 200)
        {
            CommonErrorResponse error_response = null;
            try
            {
                error_response = JsonUtility.FromJson<CommonErrorResponse>(Clean_json(req.downloadHandler.text));
            }
            catch (System.Exception)
            {
                RunStatus(req.error, ErrorColor);
                throw;
            }
            RunStatus(req.error);
            throw new System.Net.WebException(message: req.error);
        }
        else
        {
            Texture2D res;
            try
            {
                res = DownloadHandlerTexture.GetContent(req);
            }
            catch (System.Exception)
            {
                throw;
            }
            if (tokenSource.IsCancellationRequested)
            {
                throw new System.Exception(message: "Task cancelled");
            }
            return res;

        }
    }
    public string GetFilePath(string name)
    {
#if UNITY_EDITOR
        string SavePath = Path.Combine(Application.dataPath, assetbundle_dir);
#else
        string SavePath = Path.Combine(Application.persistentDataPath, assetbundle_dir);
#endif
        SavePath = Path.Combine(SavePath, name);

        //Create Directory if it does not exist
        if (!Directory.Exists(Path.GetDirectoryName(SavePath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(SavePath));
        }
        return SavePath;
    }
    public string Base_url => (IS_DEBUG ? DEBUG_BASE_URL : BASE_URL);
    private string Clean_json(string data)
    {
        RegexOptions options = RegexOptions.Multiline;
        string value = "{}";
        MatchCollection Listmatches = Regex.Matches(data, patternList, options);
        if (Listmatches.Count > 0)
            return data;

        foreach (Match m in Regex.Matches(data, pattern, options))
        {
            value = m.Value;
        }

        return value;
    }

    internal void Updates_achivement(string id, int new_status)
    {
        throw new NotImplementedException();
    }
}

