using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class RollingLog : MonoBehaviour
{

    public float rollBackTime = 0.5f;
    public float logDamage = 5f;
    public float logPresenceTime = 5f;
    public float logSpeed = 3f;
    public float scalingDuration = 0.3f;
    public float scale = 0.35f;
    public AudioClip logSpawnClip;
    public AudioClip logRollingClip; 
    AudioSource _source;
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.GetComponent<SmartEnemyGrounded>())
        {
            SmartEnemyGrounded smartEnemyGrounded = col.GetComponent<SmartEnemyGrounded>();
            print("Hi");
            smartEnemyGrounded.HitLog(rollBackTime, transform);
            smartEnemyGrounded.TakeDamage(logDamage, Vector2.zero, col.transform.position,
                gameObject, BODYPART.NONE, null);
        }
    }
    
    void Start()
    {
        _source = GetComponent<AudioSource>();
        _source.playOnAwake = false;
        StartCoroutine(DestroyLog());
        StartCoroutine(ScaleLog(transform, new Vector3(0, 0, 0), new Vector3(scale,scale,scale), scalingDuration));
    }

    IEnumerator DestroyLog()
    {
        yield return new WaitForSeconds(logPresenceTime);
        Destroy(gameObject);
    }
    void Update()
    {
        MoveLog();
    }

    void MoveLog()
    {
        transform.Translate(new Vector3(-logSpeed * Time.deltaTime,0,0));
        if (!_source.isPlaying)
        {
            _source.clip = logRollingClip;
            _source.loop = true;
            _source.Play();
        }
    }
    IEnumerator ScaleLog(Transform target, Vector3 initialScale, Vector3 finalScale, float time)
    {
        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            target.localScale = Vector3.Lerp(initialScale, finalScale, elapsedTime / time);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _source.clip = logSpawnClip;
        _source.Play();
        target.localScale = finalScale; // Ensure the final scale is set
    }
}
