using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimDisableAuto : MonoBehaviour {
	Animator anim;
	public string clipName = "clipTrigger";
	public Vector2 minScale = new Vector2(0.5f,0.5f);
	public Vector2 maxScale = new Vector2(1f,1f);
	public Sprite[] RandomSprite;
	public SpriteRenderer SpriteRandom;

	Vector3 oriSize;

	void Awake(){
		oriSize = transform.localScale;
	}
	// Use this for initialization
	void OnEnable () {
		if (anim == null)
			anim = GetComponent<Animator> ();
		CancelInvoke ();
		Invoke ("Disable", AnimationHelper.getAnimationLength (anim, clipName));
		transform.localScale = new Vector3 (Random.Range (minScale.x, maxScale.x), Random.Range (minScale.y, maxScale.y), oriSize.z);

		if (SpriteRandom != null && RandomSprite.Length > 0)
			SpriteRandom.sprite = RandomSprite [Random.Range (0, RandomSprite.Length )];
	}
	
	// Update is called once per frame
	void Disable () {
		if (gameObject.activeInHierarchy)
			gameObject.transform.position = new Vector3(0, 100, 0);
		gameObject.SetActive (false);
	}
}
