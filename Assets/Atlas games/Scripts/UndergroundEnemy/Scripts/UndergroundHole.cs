using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndergroundHole : MonoBehaviour
{
    public Transform spriteMaskTransform;
    public GameObject sprite2DMask;
    private Animator _anim;
    public GameObject meshMask;
    public float fadeTime = 3.5f;
    public AudioClip diggingSound;
    public float diggingVolume = 0.5f;
    public GameObject topHoleSection;
    Transform _mainCamera;
    public void Init(float climbingTime, float yScale, float pileScale, float holeAnimationTime,bool isSpine)
    {
        _mainCamera = GameObject.FindWithTag("MainCamera").transform;
        topHoleSection.transform.position = new Vector3(topHoleSection.transform.position.x,
            topHoleSection.transform.position.y, topHoleSection.transform.position.z + 1f);
        SoundManager.PlaySfx(diggingSound, diggingVolume);
        _anim = GetComponent<Animator>();
        _anim.SetTrigger("Open");
        transform.localScale *= pileScale;
        spriteMaskTransform.localScale = new Vector3(1, yScale, 1);
        sprite2DMask.SetActive(!isSpine);
            meshMask.gameObject.SetActive(isSpine);
            meshMask.transform.position = new Vector3(meshMask.transform.position.x, meshMask.transform.position.y, 3);
        StartCoroutine(AnimateFade());
    }

     public void DisableMask()
    {
        sprite2DMask.SetActive(false);
        meshMask.gameObject.SetActive(false);
    }

    IEnumerator AnimateFade()
    {
        yield return new WaitForSeconds(fadeTime);
        _anim.SetTrigger("Fade");
    }

    public void DisableHole()
    {
        Invoke("DeactivateHole", 0.01f);
    }

    void DeactivateHole()
    {
        gameObject.SetActive(false);
    }
}
