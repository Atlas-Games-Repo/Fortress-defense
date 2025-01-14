using UnityEngine;
using System.Collections;

public class SimpleProjectile : Projectile, ICanTakeDamage, IListener
{
    public int Damage = 30;
	public GameObject DestroyEffect;
    public int pointToGivePlayer = 100;
    public float timeToLive = 3;
	public Sprite newBulletImage;
	public AudioClip fire_sound;
	[Range(0,1)]
	public float fire_sound_volume = 0.5f;
	public AudioClip soundHitEnemy;
	[Range(0,1)]
	public float soundHitEnemyVolume = 0.5f;
	public AudioClip soundHitNothing;
	[Range(0,1)]
	public float soundHitNothingVolume = 0.5f;

	public GameObject ExplosionObj;
	private SpriteRenderer rend;

	public GameObject NormalFX;
	public GameObject DartFX;
	public GameObject destroyParent;
    float timeToLiveCounter = 0;
    void OnEnable()
    {
        timeToLiveCounter = timeToLive ;
    }
	void Start(){
		if (fire_sound)
			SoundManager.PlaySfx(fire_sound, fire_sound_volume);
		if (Explosion) {
			rend = GetComponent<SpriteRenderer> ();
//			rend.sprite = newBulletImage;
		}
		if(NormalFX)
		NormalFX.SetActive (!Explosion);
		if(DartFX)
		DartFX.SetActive (Explosion);
		GameManager.Instance.listeners.Add (this);
	}
	// Update is called once per frame

	bool comeBackToPlayer = false;
	void Update ()
	{
		if (isStop)
			return;

		if (destroyParent == null)
			destroyParent = gameObject;
		
		if ((timeToLiveCounter -= Time.deltaTime) <= 0) {
            
			if (Explosion && CanGoBackOwner)
				comeBackToPlayer = true;
			else
				DestroyProjectile ();
			
//			return;
		}

        if (comeBackToPlayer)
        {
            Vector3 comebackto = Owner.transform.position;
            destroyParent.transform.position = Vector2.MoveTowards(destroyParent.transform.position, comebackto, Speed * Time.deltaTime);
            if (Vector2.Distance(transform.position, comebackto) < 0.26f)
                (destroyParent != null ? destroyParent : gameObject).SetActive(false);
        }
        else
        {
            //Debug.LogError((Direction + new Vector2(InitialVelocity.x, 0)) * Speed * Time.deltaTime + "?" + Speed);
            transform.Translate((Direction + new Vector2(InitialVelocity.x, 0)) * Speed * Time.deltaTime, Space.World);
        }
	}

	void DestroyProjectile(){
        if (DestroyEffect != null)
            SpawnSystemHelper.GetNextObject(DestroyEffect, true).transform.position = transform.position;

            //Instantiate (DestroyEffect, transform.position, Quaternion.identity);

		if (Explosion) {
			// var bullet = Instantiate (ExplosionObj, transform.position, Quaternion.identity) as GameObject;
			var bullet = SpawnSystemHelper.GetNextObject(ExplosionObj, false);
			bullet.transform.position = transform.position;
			bullet.SetActive(true);
		}

        var obj = destroyParent != null ? destroyParent : gameObject;
		gameObject.transform.position = new Vector3(0, 100, gameObject.transform.position.z);
		gameObject.SetActive(false);
	}


	public void TakeDamage(float damage, Vector2 force, Vector2 hitPoint, GameObject instigator, BODYPART bodyPart = BODYPART.NONE, WeaponEffect weaponEffect = null, WEAPON_EFFECT forceEffect = WEAPON_EFFECT.NONE)
    {
		SoundManager.PlaySfx (soundHitNothing, soundHitNothingVolume);
		DestroyProjectile ();
	}

	protected override void OnCollideOther (Collider2D other)
	{
//		other.gameObject.SendMessageUpwards ("TakeDamage", SendMessageOptions.DontRequireReceiver);
		SoundManager.PlaySfx (soundHitNothing, soundHitNothingVolume);
		DestroyProjectile ();
	}

	protected override void OnCollideTakeDamage (Collider2D other, ICanTakeDamage takedamage)
	{
		takedamage.TakeDamage ((NewDamage == 0 ? Damage : NewDamage), Vector2.zero, transform.position, Owner, BODYPART.NONE,weaponEffect);
		SoundManager.PlaySfx (soundHitEnemy, soundHitEnemyVolume);
		DestroyProjectile ();
	}

	bool isStop = false;
	#region IListener implementation

	public void IPlay ()
	{
		//		throw new System.NotImplementedException ();
	}

	public void ISuccess ()
	{
		//		throw new System.NotImplementedException ();
	}

    public bool IEnabled() {
        return this.enabled;
    }
	public void IPause ()
	{
		//		throw new System.NotImplementedException ();
	}

	public void IUnPause ()
	{
		//		throw new System.NotImplementedException ();
	}

	public void IGameOver ()
	{
		//		throw new System.NotImplementedException ();
	}

	public void IOnRespawn ()
	{
		//		throw new System.NotImplementedException ();
	}

	public void IOnStopMovingOn ()
	{
//		Debug.Log ("IOnStopMovingOn");
//		anim.enabled = false;
		isStop = true;
		//		GetComponent<Rigidbody2D> ().isKinematic = true;
	}

	public void IOnStopMovingOff ()
	{
//		anim.enabled = true;
		isStop = false;
		//		GetComponent<Rigidbody2D> ().isKinematic = false;
	}

	#endregion
}

