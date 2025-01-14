using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.Concurrent;
using TMPro;
using System.Threading.Tasks;
public class AchievementStatus : MonoBehaviour
{

    public static AchievementStatus self;
    public int destroy_after_seconds = 2;
    public Queue<AchievementModel> models = new Queue<AchievementModel>{};
    private GameObject model;
    public GameObject prefab;
    public Transform parent;
    public AudioClip audioClip;
    public float volume;
    public string child_address = "2";
    private Coroutine coroutine = null;
    void Awake() {
        if (self == null) {
            self = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
        coroutine = StartCoroutine(Show_status());
    }
    ~AchievementStatus() {
        if (coroutine != null) {
            StopCoroutine(coroutine);
        }
    }
    void OnApplicationQuit() {
        if (coroutine != null) {
            StopCoroutine(coroutine);
        }
    }
    IEnumerator Show_status() {
        while (true) {
            yield return new WaitForEndOfFrame();
            if (model != null) continue;
            if (models.Count <= 0) continue;
            if (prefab == null || parent == null) continue;
            AchievementModel the_model = models.Dequeue();
            GameObject obj = Instantiate(prefab, parent, false);
            TextMeshProUGUI txt = ChildInParent.GetChild(obj.transform, child_address).GetComponent<TextMeshProUGUI>();
            string description = the_model.description.Replace("zNoNz", the_model.checkpoint.ToString());
            txt.text = $"{description}. reward: {the_model.reward}c";
            model = obj;
            if (audioClip != null)
                SoundManager.PlaySfx(audioClip, volume);
            yield return new WaitForSeconds(destroy_after_seconds);
            if (models == null) continue;
            Destroy(model);
            model = null;
        }
    }
}
