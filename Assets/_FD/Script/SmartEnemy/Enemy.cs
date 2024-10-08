using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
public enum ATTACKTYPE
{
    RANGE,
    MELEE,
    THROW,
    //CALLMINION,
    WIZARD,
    NONE
}
public static class ANIMATION_STATE
{
    public static readonly string DEAD = "Dead";
    public static readonly string ATTACK = "Attack";
    public static readonly string IDLE = "Idle";
    public static readonly string WALK = "Walk";
}
public enum ENEMYSTATE
{
    SPAWNING,
    IDLE,
    ATTACK,
    WALK,
    HIT,
    DEATH
}

public enum ENEMYEFFECT
{
    NONE,
    BURNING,
    FREEZE,
    SHOKING,
    POISON,
    EXPLOSION
}

public enum STARTBEHAVIOR
{
    NONE,
    BURROWUP,
    PARACHUTE,
    WALK_RIGHT,
    WALK_LEFT
}

public enum DIEBEHAVIOR
{
    NORMAL,
    DESTROY,
    BLOWUP
}

public enum HITBEHAVIOR
{
    NONE,
    CANKNOCKBACK
}

public enum ENEMYTYPE
{
    ONGROUND,
    INWATER,
    FLY
}


//public enum ENEMYKIND { NORMAL, BOSS }

[RequireComponent(typeof(CheckTargetHelper))]

public class Enemy : MonoBehaviour, ICanTakeDamage, IListener
{

    //public ENEMYKIND enemyKind;
    public UpgradedCharacterParameter upgradedCharacterID;
    [HideInInspector] public ENEMYTYPE enemyType;
    public bool is_boss = false;
    [Header("Setup")]
    public bool useGravity = false;
    [ReadOnly] public float gravity = 35f;
    public float walkSpeed = 3;
    [Header("Behavier")]
    public ATTACKTYPE attackType;
    public STARTBEHAVIOR startBehavior = STARTBEHAVIOR.WALK_LEFT;
    [HideInInspector] public DIEBEHAVIOR dieBehavior = DIEBEHAVIOR.NORMAL;
    [HideInInspector] public HITBEHAVIOR hitBehavior = HITBEHAVIOR.NONE;
    public float spawnDelay = 1;

    [ReadOnly] public float multipleSpeed = 1;

    [Header("HEALTH")]
    [Range(0, 5000)]
    public int health = 100;
    [HideInInspector] public GameObject dieFX, hitFX;
    public GameObject disableFX;
    public Transform spawnDisableFX;
    [HideInInspector] public GameObject bloodPuddleFX;
    [HideInInspector] public GameObject[] explosionFX;
    [HideInInspector] public Vector2 randomHitPoint = new Vector2(0.2f, 0.2f);
    [HideInInspector] public Vector2 randomBloodPuddlePoint = new Vector2(0.5f, 0.25f);
    public Vector2 healthBarOffset = new Vector2(0, 1.5f);
    public float AutoHealthBarOffset = 0.2f;
    public bool IsAutoHealthBar = true;

    [ReadOnly] public ENEMYSTATE enemyState = ENEMYSTATE.IDLE;
    protected ENEMYEFFECT enemyEffect;
    [Space]

    [Header("WEAKNESS AND STRENGTH")]
    [Range(1, 10)]
    public float weaknessMultiplier = 1f;
    [Range(0, 1)]
    public float strengthMultiplier = 1f;
    public WEAPON_EFFECT weakness = WEAPON_EFFECT.NONE;
    public WEAPON_EFFECT strength = WEAPON_EFFECT.NONE;

    [Header("Freeze Option")]
    [HideInInspector] public bool canBeFreeze = true;
    //public float timeFreeze = 5;
    [HideInInspector] public GameObject dieFrozenFX;

    [Header("Burning Option")]
    [HideInInspector] public bool canBeBurn = true;
    //public float timeBurn = 2;
    [HideInInspector] public GameObject dieBurnFX;
    float damageBurningPerFrame;

    [Header("Poison Option")]
    [HideInInspector] public bool canBePoison = true;
    [Tooltip("% reduce poison damage")]
    [Range(0, 90)]
    [HideInInspector] public float resistPoisonPercent = 10;
    [Range(0, 1)]
    [HideInInspector] public float poisonSlowSpeed = 0.3f;
    //public int timePoison = 5;
    [ReadOnly] public float damagePoisonPerSecond;

    [Header("Shocking Option")]
    [HideInInspector] public bool canBeShock = true;
    [HideInInspector] public float timeShocking = 2f;
    float damageShockingPerFrame;

    [Header("Sound")]
    [Range(0, 1)]
    public float soundHitVol = 0.5f;
    public AudioClip[] soundHit;
    [Range(0, 1)]
    public float soundDieVol = 0.5f;
    public AudioClip[] soundDie;
    [Range(0, 1)]
    public float soundDieBlowVol = 0.5f;
    public AudioClip[] soundDieBlow;
    public AudioClip BossMusic;
    [Range(0, 1)]
    [HideInInspector] public float soundSpawnVol = 0.5f;
    [HideInInspector] public AudioClip[] soundSpawn;


    [ReadOnly] public int currentHealth;
    Vector2 hitPos;
    Vector2 knockBackForce;
    public bool isPlaying { get; set; }
    public bool isStopping { get; set; }
    [ReadOnly] public bool isStunning = false;

    protected HealthBarEnemyNew healthBar;
    protected Animator anim;

    protected float moveSpeed;
    [ReadOnlyAttribute] public CheckTargetHelper checkTarget;
    [ReadOnly] public bool isPlayerDetected;
    [HideInInspector] public float delayChasePlayerWhenDetect = 1;

    [Header("Spine options")]
    public bool is_spine;
    public SkeletonAnimation skeletonAnimation;

    [Space(3)]
    [Header("Night Mode")] public float customNightMultiplier = 2f;
    public bool useCustomNightMultiplier = false;
    public bool isFacingRight()
    {
        return transform.rotation.eulerAngles.y == 180 ? true : false;
    }

    protected virtual void OnEnable()
    {
        if (GameManager.Instance)
            GameManager.Instance.AddListener(this);

        isPlaying = true;
    }


    public virtual void Start()
    {
        int initialHealth = health;
        if (GameLevelSetup.Instance && GameLevelSetup.Instance && GameLevelSetup.Instance.NightMode())
        {
            if (useCustomNightMultiplier)
            {
                health = Mathf.RoundToInt(initialHealth * customNightMultiplier);
            }
            else
            {
                health = Mathf.RoundToInt(GameLevelSetup.Instance.NightModeXpMultiplier());
            }
        }
        
        if (!useGravity)
            gravity = 0;
        currentHealth = health;
        moveSpeed = walkSpeed;
        if (IsAutoHealthBar ){
            if (transform.GetComponent<BoxCollider2D>())
            {
                BoxCollider2D box = transform.GetComponent<BoxCollider2D>();
                healthBarOffset.y = Mathf.Abs(box.bounds.size.y + AutoHealthBarOffset);
            }else if (transform.GetComponent<PolygonCollider2D>())
            {
                PolygonCollider2D poly = transform.GetComponent<PolygonCollider2D>();
                healthBarOffset.y = Mathf.Abs(poly.bounds.size.y + AutoHealthBarOffset);
            }
         
            
        }
        var healthBarObj = (HealthBarEnemyNew)Resources.Load("HealthBar", typeof(HealthBarEnemyNew));
        healthBar = (HealthBarEnemyNew)Instantiate(healthBarObj, healthBarOffset, Quaternion.identity);

        healthBar.Init(transform, (Vector3)healthBarOffset);

        anim = GetComponent<Animator>();
        if (is_spine) {
            foreach (Transform item in transform) {
                item.TryGetComponent(out skeletonAnimation);
                if (skeletonAnimation) break;
            }
        }
        checkTarget = GetComponent<CheckTargetHelper>();

        switch (startBehavior)
        {
            case STARTBEHAVIOR.BURROWUP:
                SoundManager.PlaySfx(soundSpawn, soundSpawnVol);

                SetEnemyState(ENEMYSTATE.SPAWNING);
                AnimSetTrigger("spawn");

                Invoke("FinishSpawning", spawnDelay);
                break;
            case STARTBEHAVIOR.NONE:
                SetEnemyState(ENEMYSTATE.WALK);
                break;
            case STARTBEHAVIOR.WALK_LEFT:
                SetEnemyState(ENEMYSTATE.WALK);
                break;
            case STARTBEHAVIOR.WALK_RIGHT:
                SetEnemyState(ENEMYSTATE.WALK);
                break;

        }
        SetSkeletonAnimation(ANIMATION_STATE.WALK, true);

    }

    void FinishSpawning()
    {
        if (enemyState == ENEMYSTATE.SPAWNING && isPlaying)
            SetEnemyState(ENEMYSTATE.WALK);
    }

    public void SetSkeletonAnimation(string name, bool looped = false, int trackIndex = 0)
    {
        if (is_spine && skeletonAnimation != null)
            skeletonAnimation.AnimationState.SetAnimation(trackIndex, name, looped);
    }
    public void AnimSetTrigger(string name)
    {
        anim.SetTrigger(name);
    }

    public void AnimSetBool(string name, bool value)
    {
        anim.SetBool(name, value);
    }

    public void AnimSetFloat(string name, float value)
    {
        anim.SetFloat(name, value);
    }

    public void SetEnemyState(ENEMYSTATE state)
    {
        enemyState = state;
    }

    public void SetEnemyEffect(ENEMYEFFECT effect)
    {
        enemyEffect = effect;
    }

    public bool IEnabled() {
        return this.enabled;
    }
    public virtual void Update()
    {

        //Debug.LogError(enemyEffect);
        if (enemyEffect == ENEMYEFFECT.BURNING)
            CheckDamagePerFrame(damageBurningPerFrame);

        if (enemyEffect == ENEMYEFFECT.SHOKING)
            CheckDamagePerFrame(damageShockingPerFrame);

        healthBar.transform.localScale = new Vector2(transform.localScale.x > 0 ? Mathf.Abs(healthBar.transform.localScale.x) : -Mathf.Abs(healthBar.transform.localScale.x), healthBar.transform.localScale.y);
    }


    //can call by Alarm action of other Enemy
    public virtual void DetectPlayer(float delayChase = 0)
    {
        if (isPlayerDetected)
            return;

        StartCoroutine(DelayBeforeChasePlayer(delayChase));
    }


    protected IEnumerator DelayBeforeChasePlayer(float delay)
    {
        yield return null;
        while (isStopping || isStunning) { yield return null; }
        isPlayerDetected = true;

        if (delay > 0)
        {
            SetEnemyState(ENEMYSTATE.IDLE);
            yield return new WaitForSeconds(delay);
        }

        if (enemyState == ENEMYSTATE.ATTACK)
        {
            yield break;
        }

        SetEnemyState(ENEMYSTATE.WALK);
    }

    public virtual void FixedUpdate()
    {

    }

    public virtual void Hit(Vector2 force = default(Vector2), bool pushBack = false, bool knockDownRagdoll = false, bool shock = false)
    {
        SoundManager.PlaySfx(soundHit, soundHitVol);
        switch (hitBehavior)
        {
            case HITBEHAVIOR.CANKNOCKBACK:
                KnockBack(knockBackForce);
                break;
            case HITBEHAVIOR.NONE:

                break;
            default:
                break;
        }
    }

    public virtual void KnockBack(Vector2 force, float stunningTime = 0)
    {

    }

    public virtual void Die()
    {
        isPlaying = false;
        GameManager.Instance.RemoveListener(this);
        isPlayerDetected = false;
        SetEnemyState(ENEMYSTATE.DEATH);

        if (GetComponent<GiveCoinWhenDie>())
        {
            GetComponent<GiveCoinWhenDie>().GiveCoin();
        }

        if (GetComponent<GiveExpWhenDie>())
        {
            GetComponent<GiveExpWhenDie>().GiveExp();
        }

        if (dieFX)
            Instantiate(dieFX, transform.position, dieFX.transform.rotation);

        if (enemyEffect == ENEMYEFFECT.FREEZE && dieFrozenFX)
            SpawnSystemHelper.GetNextObject(dieFrozenFX, true).transform.position = hitPos;

        if (enemyEffect == ENEMYEFFECT.SHOKING)
            UnShock();

        if (enemyEffect == ENEMYEFFECT.EXPLOSION)
        {
            if (bloodPuddleFX)
            {
                for (int i = 0; i < Random.Range(2, 5); i++)
                {
                    SpawnSystemHelper.GetNextObject(bloodPuddleFX, true).transform.position =
                        (Vector2)transform.position + new Vector2(Random.Range(-(randomBloodPuddlePoint.x * 2), randomBloodPuddlePoint.x * 2), Random.Range(-(2 * randomBloodPuddlePoint.y), 2 * randomBloodPuddlePoint.y));
                }
            }
            if (explosionFX.Length > 0)
            {
                for (int i = 0; i < Random.Range(1, 3); i++)
                {
                    var obj = SpawnSystemHelper.GetNextObject(explosionFX[Random.Range(0, explosionFX.Length)], false);
                    obj.transform.position = transform.position;
                    obj.SetActive(true);
                }
            }

            SoundManager.PlaySfx(soundDieBlow, soundDieBlowVol);
        }
        else
            SoundManager.PlaySfx(soundDie, soundDieVol);

        if (is_boss)
            SoundManager.PlayMusic(SoundManager.Instance.musicsGame);

        GlobalValue.KillCount += 1;

    }

    private void CheckDamagePerFrame(float _damage)
    {
        
        if (enemyState == ENEMYSTATE.DEATH)
            return;
        currentHealth -= (int)_damage;
        if (healthBar)
            healthBar.UpdateValue(currentHealth / (float)health);

        if (currentHealth <= 0)
            Die();
    }

    #region IListener implementation

    public virtual void IPlay()
    {
    }

    public virtual void ISuccess()
    {
    }

    public virtual void IPause()
    {
    }

    public virtual void IUnPause()
    {
    }

    public virtual void IGameOver()
    {
        if (!isPlaying)
            return;

        isPlaying = false;
        SetEnemyState(ENEMYSTATE.IDLE);
    }

    public virtual void IOnRespawn()
    {
    }

    public virtual void IOnStopMovingOn()
    {
    }

    public virtual void IOnStopMovingOff()
    {
    }

    #endregion

    #region ICanFreeze implementation
    //	_2dxFX_Frozen[] iceFX;
    public virtual void Freeze(float time, GameObject instigator)
    {
        if (enemyEffect == ENEMYEFFECT.FREEZE)
            return;

        if (enemyEffect == ENEMYEFFECT.BURNING)
            BurnOut();

        if (enemyEffect == ENEMYEFFECT.SHOKING)
        {
            UnShock();
        }

        if (canBeFreeze)
        {
            enemyEffect = ENEMYEFFECT.FREEZE;
            StartCoroutine(UnFreezeCo(time));
        }
    }

    IEnumerator UnFreezeCo(float time)
    {
        AnimSetBool("isFreezing", true);
        SetSkeletonAnimation(ANIMATION_STATE.IDLE, true);
        if (enemyEffect != ENEMYEFFECT.FREEZE)
            yield break;
        float _time = weakness == WEAPON_EFFECT.FREEZE ? time * weaknessMultiplier : strength == WEAPON_EFFECT.FREEZE ? time * strengthMultiplier : time;
        yield return new WaitForSeconds(_time);
        UnFreeze();
    }

    void UnFreeze()
    {
        if (enemyEffect != ENEMYEFFECT.FREEZE)
            return;

        enemyEffect = ENEMYEFFECT.NONE;
        AnimSetBool("isFreezing", false);
        SetSkeletonAnimation(ANIMATION_STATE.WALK, true);
    }

    #endregion

    #region LIGHTING
    //	_2dxFX_Frozen[] iceFX;
    public virtual void Lighting(float time, GameObject instigator)
    {
        if (enemyEffect == ENEMYEFFECT.FREEZE)
            return;

        if (enemyEffect == ENEMYEFFECT.BURNING)
            BurnOut();

        if (enemyEffect == ENEMYEFFECT.SHOKING)
        {
            UnShock();
        }

        enemyEffect = ENEMYEFFECT.FREEZE;
        StartCoroutine(UnLightingCo(time));
    }

    IEnumerator UnLightingCo(float time)
    {
        AnimSetBool("isLighting", true);
        SetSkeletonAnimation(ANIMATION_STATE.IDLE, true);

        if (enemyEffect != ENEMYEFFECT.FREEZE)
            yield break;

        yield return new WaitForSeconds(time);
        UnLighting();
    }

    void UnLighting()
    {
        if (enemyEffect != ENEMYEFFECT.FREEZE)
            return;

        enemyEffect = ENEMYEFFECT.NONE;
        AnimSetBool("isLighting", false);
        SetSkeletonAnimation(ANIMATION_STATE.WALK, true);
    }

    #endregion

    #region ICanBurn implementation
    public virtual void Burning(float damage, GameObject instigator)
    {
        if (enemyEffect == ENEMYEFFECT.BURNING)
            return;

        if (enemyEffect == ENEMYEFFECT.FREEZE)
        {
            UnFreeze();
        }

        if (enemyEffect == ENEMYEFFECT.SHOKING)
        {
            UnShock();
        }

        if (canBeBurn)
        {
            damageBurningPerFrame = damage;
            enemyEffect = ENEMYEFFECT.BURNING;

            StartCoroutine(BurnOutCo(1));
        }
    }

    IEnumerator BurnOutCo(float time)
    {
        if (enemyEffect != ENEMYEFFECT.BURNING)
            yield break;

        //float wait = timeBurn - 1;
        yield return new WaitForSeconds(time);


        if (enemyState == ENEMYSTATE.DEATH)
        {
            BurnOut();
            gameObject.SetActive(false);
        }

        BurnOut();
    }

    void BurnOut()
    {
        if (enemyEffect != ENEMYEFFECT.BURNING)
            return;

        enemyEffect = ENEMYEFFECT.NONE;
    }
    #endregion

    #region ICanPoison implementation
    public virtual void Poison(float damage, float time, GameObject instigator)
    {
        if (enemyEffect == ENEMYEFFECT.BURNING)
            return;

        if (enemyEffect == ENEMYEFFECT.POISON)
            return;

        if (enemyEffect == ENEMYEFFECT.FREEZE)
        {
            UnFreeze();
        }

        if (enemyEffect == ENEMYEFFECT.SHOKING)
        {
            UnShock();
        }

        if (canBePoison)
        {
            damagePoisonPerSecond = damage;
            enemyEffect = ENEMYEFFECT.POISON;
        
            StartCoroutine(PoisonCo(time));
        }
    }

    IEnumerator PoisonCo(float time)
    {
        AnimSetBool("isPoisoning", true);
        multipleSpeed = poisonSlowSpeed;
        if (enemyEffect != ENEMYEFFECT.POISON)
            yield break;

        int wait = (int)time;

        while (wait > 0)
        {
            yield return new WaitForSeconds(1);
            //Debug.LogError(damagePoisonPerSecond);
            float _damage = damagePoisonPerSecond * Random.Range(90 - resistPoisonPercent, 100f - resistPoisonPercent) * 0.01f;
            // POISON is active
            if (weakness == WEAPON_EFFECT.POISON)
                _damage *= weaknessMultiplier;
            if (strength == WEAPON_EFFECT.POISON)
                _damage *= strengthMultiplier;

            currentHealth -= (int)_damage;
            if (healthBar)
                healthBar.UpdateValue(currentHealth / (float)health);

            FloatingTextManager.Instance.ShowText("" + (int)_damage, healthBarOffset, Color.red, transform.position);

            if (currentHealth <= 0)
            {
                PoisonEnd();
                Die();
                yield break;
            }

            wait -= 1;
        }


        if (enemyState == ENEMYSTATE.DEATH)
        {
            BurnOut();
            gameObject.SetActive(false);
        }

        PoisonEnd();
    }

    void PoisonEnd()
    {
        if (enemyEffect != ENEMYEFFECT.POISON)
            return;

        AnimSetBool("isPoisoning", false);
        multipleSpeed = 1;

        enemyEffect = ENEMYEFFECT.NONE;
    }
    #endregion

    #region ICanShock implementation

    //	protected _2dxFX_Lightning[] shockFX;
    public virtual void Shoking(float damage, GameObject instigator)
    {
        if (enemyEffect == ENEMYEFFECT.SHOKING)
            return;

        if (enemyEffect == ENEMYEFFECT.FREEZE)
        {
            UnFreeze();
        }

        if (enemyEffect == ENEMYEFFECT.BURNING)
            BurnOut();

        if (canBeShock)
        {
            damageShockingPerFrame = damage;
            enemyEffect = ENEMYEFFECT.SHOKING;
            StartCoroutine(UnShockCo());
        }
    }

    IEnumerator UnShockCo()
    {
        if (enemyEffect != ENEMYEFFECT.SHOKING)
            yield break;

        yield return new WaitForSeconds(timeShocking);

        UnShock();
    }

    void UnShock()
    {
        if (enemyEffect != ENEMYEFFECT.SHOKING)
            return;

        enemyEffect = ENEMYEFFECT.NONE;
    }
    #endregion

    #region Stun
    public virtual void Stun(float time = 2)
    {

    }

    public virtual void StunManuallyOn()
    {

    }

    public virtual void StunManuallyOff()
    {

    }
    #endregion

    #region TakeDamage implementation
    protected BODYPART _bodyPart;
    protected Vector2 _bodyPartForce;
    protected float _damage;
    public void TakeDamage(float damage, Vector2 force, Vector2 hitPoint, GameObject instigator, BODYPART bodyPart = BODYPART.NONE, WeaponEffect weaponEffect = null, WEAPON_EFFECT forceEffect = WEAPON_EFFECT.NONE)
    {
        if (enemyState == ENEMYSTATE.DEATH)
            return;

        Player_Archer owner = instigator.GetComponent<Player_Archer>();
        if (isStopping)
            return;
        //store parameters
        _bodyPart = bodyPart;
        _bodyPartForce = force;
        _damage = damage * GlobalValue.AttackDamageRate;
        hitPos = hitPoint;
        bool isExplosion = false;
        WEAPON_EFFECT effect = weaponEffect != null ? forceEffect != WEAPON_EFFECT.NONE ? forceEffect : weaponEffect.effectType : WEAPON_EFFECT.NONE;
        if (effect == weakness)
            _damage *= weaknessMultiplier;
        if (effect == strength)
            _damage *= strengthMultiplier;
        currentHealth -= (int)_damage;
        FloatingTextManager.Instance.ShowText("" + (int)_damage, healthBarOffset, Color.red, transform.position);

        //if (_bodyPart == BODYPART.HEAD)
        //    FloatingTextManager.Instance.ShowText("HEAD SHOT", healthBarOffset, Color.black, transform.position, 24);

        knockBackForce = force;
        if (hitFX)
            SpawnSystemHelper.GetNextObject(hitFX, true).transform.position =
                hitPos + new Vector2(Random.Range(-randomHitPoint.x, randomHitPoint.x), Random.Range(-randomHitPoint.y, randomHitPoint.y));
        if (bloodPuddleFX)
            SpawnSystemHelper.GetNextObject(bloodPuddleFX, true).transform.position =
                (Vector2)transform.position + new Vector2(Random.Range(-randomBloodPuddlePoint.x, randomBloodPuddlePoint.x), Random.Range(-randomBloodPuddlePoint.y, randomBloodPuddlePoint.y));


        if (healthBar)
            healthBar.UpdateValue(currentHealth / (float)health);
        //		Debug.LogError (isExplosion + "BLOW" + (dieBehavior == DIEBEHAVIOR.BLOWUP));
        if (currentHealth <= 0)
        {
            if (isExplosion || dieBehavior == DIEBEHAVIOR.BLOWUP)
            {
                SetEnemyEffect(ENEMYEFFECT.EXPLOSION);
            }
            if (owner)
            {
                if (owner.criticalRateTemp + 0.2f <= 1f && owner.is_manual_enable)
                {
                    owner.criticalRateTemp += 0.2f;
                }
                if (owner.criticalRateTemp >= 0.8f)
                    owner.criticalRateTemp = owner.criticalRate;
            }

            Die();
        }
        else
        {
            if (weaponEffect != null)
            {
                switch (forceEffect != WEAPON_EFFECT.NONE ? forceEffect : weaponEffect.effectType)
                {
                    case WEAPON_EFFECT.POISON:
                        if (Random.Range(0f, 1f) <= weaponEffect.poisonChance)
                            Poison(weaponEffect.poisonDamagePerSec, weaponEffect.poisonTime, instigator);
                        return;
                    case WEAPON_EFFECT.FREEZE:
                        if (Random.Range(0f, 1f) <= weaponEffect.freezeChance)
                            Freeze(weaponEffect.freezeTime, instigator);
                        return;
                    case WEAPON_EFFECT.LIGHTING:
                        Lighting(0.5f, instigator);
                        //AnimSetTrigger("isLighting");
                        break;
                    default:
                        break;
                }
            }

            Hit(force);
        }
    }
    #endregion

}
