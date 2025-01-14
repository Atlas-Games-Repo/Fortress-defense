using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

public class AddressableManager : MonoBehaviour
{
    public AssetBundleGeneratorV2 bundles;
    // Start is called before the first frame update
    void Start() {
        SceneManager.sceneLoaded += OnSceneLoaded;
        if (!bundles.obj) StartCoroutine(DownloadProcess(bundles));
        else StartCoroutine(HandleObject(bundles.obj));
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1) {
        if (!bundles.obj) StartCoroutine(DownloadProcess(bundles));
        else StartCoroutine(HandleObject(bundles.obj));
    }

    // public void DownloadAsset(AssetBundleGeneratorV2 assetbundle)
    // {
    //     StartCoroutine(DownloadProcess(assetbundle));
    // }
    IEnumerator DownloadProcess(AssetBundleGeneratorV2 assetbundle) {
        AsyncOperationHandle<GameObject> handler = Addressables.LoadAssetAsync<GameObject>(assetbundle.asset_bundle.AssetGUID);
        handler.Completed += OnAssetLoaded;
        while (handler.Status == AsyncOperationStatus.None) {
            yield return new WaitForEndOfFrame();
            bundles.percent = handler.GetDownloadStatus().Percent;
        }
        bundles.percent = 1;
    }
    private void OnAssetLoaded(AsyncOperationHandle<GameObject> handle)
    {

        if (handle.Status == AsyncOperationStatus.Succeeded){
            StartCoroutine(HandleObject(handle.Result));
        }
        else {
            APIManager.instance.RunStatus("Couldn't download assets! try later.",Color.red);
        }
    }
    IEnumerator HandleObject(GameObject obj) {
        bundles.obj = obj;
        if (bundles.scene_name != SceneManager.GetActiveScene().name) {
            bundles.is_complete = true;
            yield break;
        }
        yield return new WaitForEndOfFrame();
        GameObject _obj;
        if (!bundles.is_root_world) {
            GameObject root_obj = GameObject.Find(bundles.root);
            _obj = Instantiate(obj, root_obj.transform ,false);
            _obj.name = obj.name;
        } else {
            _obj = Instantiate(obj);
            _obj.name = obj.name;
        }
        if (bundles.has_depends) {
            GameObject __obj = GameObject.Find(bundles.dependence);
            if (__obj) __obj.GetComponent(bundles.dependence_type).SendMessage("assetBundleDepends", _obj);
        }
        bundles.is_complete = true;
    }

    public void CheckForUpdateOrCache()
    {
        StartCoroutine(CheckForUpdateOrCacheIEnum());
    }
    IEnumerator CheckForUpdateOrCacheIEnum() {
        AsyncOperationHandle<List<string>> update_handler = Addressables.CheckForCatalogUpdates(false);
        while (update_handler.Status == AsyncOperationStatus.None) {
            yield return new WaitForEndOfFrame();
        }
        if (update_handler.Status == AsyncOperationStatus.Failed) {
            yield break;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
