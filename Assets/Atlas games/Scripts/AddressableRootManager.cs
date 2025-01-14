using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddressableRootManager : MonoBehaviour
{
    public static AddressableRootManager self;
    public AddressableManager[] addressables;
    public Slider loading;
    public TMPro.TextMeshProUGUI loading_text;
    public bool is_download_complete;
    // Start is called before the first frame update
    private void Awake() {
        if (self == null){
            self = this;
            DontDestroyOnLoad(this);
        } else {
            Destroy(this);
        }
    }
    void Start() {
        if (loading_text) loading_text.text = "Downloading...";
    }

    // Update is called once per frame
    void Update() {
        if (is_download_complete) return;
        float percent = 0;
        int comp_count = 0;
        foreach (AddressableManager item in addressables) {
            percent += item.bundles.percent;
            if (item.bundles.is_complete) comp_count++;
        }
        if (comp_count == addressables.Length) {
            //load next scene
            // StartCoroutine(APIManager.instance.LoadAsynchronously("Menu atlas"));
            is_download_complete = true;
            StartCoroutine(APIManager.instance.LoadAsynchronously());
        }
        if (loading) loading.value = percent / (float)addressables.Length;
    }
}
