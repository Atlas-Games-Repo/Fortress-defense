using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(CheckTargetHelper))]
[RequireComponent(typeof(Controller2D))]

public class Player_Archer : Enemy, ICanTakeDamage, IListener
{
    public CheckTargetHelper checkTargetHelper;
    [Header("MANUAL ARROW SHOOT")]
    [ReadOnly]
    public bool is_manual_enable = false;
    [ReadOnly]
    public GameObject manual_targeted_enemy;
    [Header("ARROW SETUP")]
    public NumberArrow numberArrow = NumberArrow.Single;
    public float shootRate = 1;
    public float force = 20;
    public Vector2 aimOffset = new Vector2(0.1f, 0.1f);
    public GameObject iconIce, iconPoison, iconDoubleArrow;
    public WeaponEffect weaponEffect;
    [ReadOnly] public int damageMin, damageMax;
    [Tooltip("aim target postion = target position + animOffset")]
    //[ReadOnly]
    [Range(0.01f, 0.1f)]
    [ReadOnly] public float stepCheck = 0.1f;
    [ReadOnly] public float stepAngle = 1;
    [ReadOnly] public float gravityScale = 3.5f;
    [ReadOnly] public bool onlyShootTargetInFront = true;
    public ArrowProjectile arrow;
    [Range(0, 1)]
    public float criticalRate = 0.1f;
    public float criticalRateTemp = 0.1f;
    [ReadOnly]
    public float criticalMultiplier = 1.2f;
    public Transform firePostion;
    [Space]
    [Header("Sound")]
    public float soundShootVolume = 0.5f;
    public AudioClip[] soundShoot;

    private float x1, y1;
    bool isTargetRight = false;
    float lastShoot;

    [ReadOnly] public bool isAvailable = true;

    public bool isSocking { get; set; }
    public bool isDead { get; set; }

    [HideInInspector]
    public Vector3 velocity;
    private Vector2 _direction;
    [HideInInspector]
    public Controller2D controller;

    float velocityXSmoothing = 0;
    Vector2 pushForce;
    private float _directionFace;

    [Header("New")]
    bool allowCheckAttack = true;
    bool isLoading = false;
    bool _is_attacking = false;
    public bool is_attacking
    {
        get { return _is_attacking; }
    }
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        controller = GetComponent<Controller2D>();
        _direction = isFacingRight() ? Vector2.right : Vector2.left;

        if ((_direction == Vector2.right && startBehavior == STARTBEHAVIOR.WALK_LEFT) || (_direction == Vector2.left && startBehavior == STARTBEHAVIOR.WALK_RIGHT))
        {
            Flip();
        }
        isPlaying = true;
        isSocking = false;

        controller.collisions.faceDir = 1;

        iconIce.SetActive(weaponEffect.effectType == WEAPON_EFFECT.FREEZE);
        iconPoison.SetActive(weaponEffect.effectType == WEAPON_EFFECT.POISON);
        iconDoubleArrow.SetActive(numberArrow == NumberArrow.Double);
        StartCoroutine(AutoCheckAndShoot());

        //Do Get Upgrade
        if (upgradedCharacterID != null)
        {
            weaponEffect = upgradedCharacterID.weaponEffect;
            damageMin = weaponEffect.normalDamageMin + upgradedCharacterID.UpgradeRangeDamage;
            damageMax = weaponEffect.normalDamageMax + upgradedCharacterID.UpgradeRangeDamage;

            criticalRate = upgradedCharacterID.UpgradeCriticalDamage / 100f;
        }
        //arrowDamage = upgradedCharacterID.UpgradeRangeDamage;
    }

    public override void Update()
    {
        base.Update();
        HandleAnimation();

        if (enemyState != ENEMYSTATE.WALK)
        {
            velocity.x = 0;
            return;
        }

        if (checkTarget.CheckTarget(isFacingRight() ? 1 : -1))
            DetectPlayer(delayChasePlayerWhenDetect);
    }

    public virtual void LateUpdate()
    {
        if (GameManager.Instance.State != GameManager.GameState.Playing)
            return;
        else if (!isPlaying || isSocking || enemyEffect == ENEMYEFFECT.SHOKING || isLoading || checkTargetHelper.CheckTarget((isFacingRight() ? 1 : -1)))
        {
            velocity = Vector2.zero;
            return;
        }

        float targetVelocityX = _direction.x * moveSpeed;
        if (isSocking || enemyEffect == ENEMYEFFECT.SHOKING)
        {
            targetVelocityX = 0;
        }

        if (enemyState != ENEMYSTATE.WALK || enemyEffect == ENEMYEFFECT.FREEZE)
            targetVelocityX = 0;

        if (isStopping || isStunning)
            targetVelocityX = 0;

        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? 0.1f : 0.2f);

        velocity.y += -gravity * Time.deltaTime;

        if ((_direction.x > 0 && controller.collisions.right) || (_direction.x < 0 && controller.collisions.left))
            velocity.x = 0;

        controller.Move(velocity * Time.deltaTime * multipleSpeed, false, isFacingRight());

        if (controller.collisions.above || controller.collisions.below)
            velocity.y = 0;
    }

    void Flip()
    {
        _direction = -_direction;
        transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, isFacingRight() ? 0 : 180, transform.rotation.z));
    }

    public override void Stun(float time = 2)
    {
        base.Stun(time);
        StartCoroutine(StunCo(time));
    }

    IEnumerator StunCo(float time)
    {
        AnimSetTrigger("stun");
        isStunning = true;
        yield return new WaitForSeconds(time);
        isStunning = false;
    }

    public override void StunManuallyOn()
    {
        AnimSetTrigger("stun");
        isStunning = true;
    }

    public override void StunManuallyOff()
    {
        isStunning = false;
    }

    public override void DetectPlayer(float delayChase = 0)
    {
        base.DetectPlayer(delayChase);
    }

    void HandleAnimation()
    {
        //AnimSetFloat("speed", Mathf.Abs(velocity.x));
        //AnimSetBool("isRunning", Mathf.Abs(velocity.x) > walkSpeed);
        //AnimSetBool("isStunning", isStunning);
    }

    public void SetForce(float x, float y)
    {
        velocity = new Vector3(x, y, 0);
    }

    public override void Die()
    {
        if (isDead)
            return;

        base.Die();

        isDead = true;

        CancelInvoke();

        var cols = GetComponents<BoxCollider2D>();
        foreach (var col in cols)
            col.enabled = false;

        AnimSetBool("isDead", true);
        if (Random.Range(0, 2) == 1)
            AnimSetTrigger("die2");

        if (enemyEffect == ENEMYEFFECT.BURNING)
            return;

        if (enemyEffect == ENEMYEFFECT.EXPLOSION || dieBehavior == DIEBEHAVIOR.DESTROY)
        {
            gameObject.SetActive(false);
            return;
        }

        StopAllCoroutines();
        StartCoroutine(DisableEnemy(AnimationHelper.getAnimationLength(anim, "Die") + 2f));
    }

    public override void Hit(Vector2 force, bool pushBack = false, bool knockDownRagdoll = false, bool shock = false)
    {
        if (!isPlaying || isStunning)
            return;

        base.Hit(force, pushBack, knockDownRagdoll, shock);
        if (isDead)
            return;

        AnimSetTrigger("hit");

        if (knockDownRagdoll)
            {}
        else if (pushBack)
            StartCoroutine(PushBack(force));
        else if (shock)
            StartCoroutine(Shock());
        else
            {}
    }

    public override void KnockBack(Vector2 force, float stunningTime = 0)
    {
        base.KnockBack(force);

        SetForce(force.x, force.y);
    }

    public IEnumerator PushBack(Vector2 force)
    {
        SetForce(force.x, force.y);

        if (isDead)
        {
            Die();
            yield break;
        }
    }

    public IEnumerator Shock()
    {
        if (isDead)
        {
            Die();
            yield break;
        }
    }

    IEnumerator DisableEnemy(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (disableFX)
            SpawnSystemHelper.GetNextObject(disableFX, true).transform.position = spawnDisableFX != null ? spawnDisableFX.position : transform.position;
        gameObject.SetActive(false);
    }

    Vector2 autoShootPoint;
    float delayAutoShoot = 3;
    float delayAutoShootCounter = 0;
    Transform target;

    IEnumerator AutoCheckAndShoot()
    {
        while (true)
        {
            target = null;
            yield return null;

            while (!checkTargetHelper.CheckTarget((isFacingRight() ? 1 : -1))) { yield return null; };

            RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, 100, Vector2.zero, 0, GameManager.Instance.layerEnemy);
            if (hits.Length > 0)
            {
                float closestDistance = 99999;
                foreach (var obj in hits)
                {
                    var checkEnemy = (ICanTakeDamage)obj.collider.gameObject.GetComponent(typeof(ICanTakeDamage));
                    if (checkEnemy != null)
                    {

                        if (is_manual_enable)
                        {
                            //Debug.LogError("MANUAL ARCHER ATTACK");
                            // GlobalValue.Debug("Manual ARCHER ATTACK");
                            //Debug.LogError(obj.transform.GetComponent<Enemy>().attackType);
                            if (manual_targeted_enemy && GameObject.ReferenceEquals(manual_targeted_enemy, obj.transform.gameObject))
                            {
                                target = obj.transform;
                                // GlobalValue.Debug($"{target.name} is selected - Manual");
                                obj.transform.GetComponent<SmartEnemyGrounded>().is_targeted = true;
                                var hit = Physics2D.Raycast(transform.position, (obj.point - (Vector2)transform.position), 100, GameManager.Instance.layerEnemy);
                                Debug.DrawRay(transform.position, (obj.point - (Vector2)transform.position) * 100, Color.red);

                                autoShootPoint = /*target.position + Vector3.up * Random.Range(0.2f, 0.6f);*/ hit.collider.gameObject.transform.position;
                                //autoShootPoint.y = Mathf.Max(autoShootPoint.y, firePostion.position.y - 0.1f);
                            }
                            else
                            {
                                obj.transform.GetComponent<SmartEnemyGrounded>().is_targeted = false;
                            }
                        }
                        else
                        {
                            // GlobalValue.Debug("Auto ARCHER ATTACK");
                            if (Mathf.Abs(obj.transform.position.x - transform.position.x) < closestDistance)
                            {
                                closestDistance = Mathf.Abs(obj.transform.position.x - transform.position.x);

                                target = obj.transform;
                                // GlobalValue.Debug($"{target.name} is selected - Automatic");

                                var hit = Physics2D.Raycast(transform.position, (obj.point - (Vector2)transform.position), 100, GameManager.Instance.layerEnemy);
                                Debug.DrawRay(transform.position, (obj.point - (Vector2)transform.position) * 100, Color.red);

                                autoShootPoint = /*target.position + Vector3.up * Random.Range(0.2f, 0.6f);*/ hit.collider.gameObject.transform.position;
                                //autoShootPoint.y = Mathf.Max(autoShootPoint.y, firePostion.position.y - 0.1f);
                            }

                        }
                    }
                }

                if (target)
                {
                    Shoot();
                    _is_attacking = true;
                    yield return new WaitForSeconds(0.2f);
                }
                else
                {
                    _is_attacking = false;
                }
            }
        }
    }

    public void Victory()
    {
        anim.SetBool("victory", true);
    }

    public void Shoot()
    {
        if (!isAvailable || target == null || GameManager.Instance.State != GameManager.GameState.Playing)
            return;

        isTargetRight = autoShootPoint.x > transform.position.x;

        if (onlyShootTargetInFront && ((isTargetRight && !isFacingRight()) || (isFacingRight() && !isTargetRight)))
            return;

        StartCoroutine(CheckTarget());
    }


    IEnumerator CheckTarget()
    {
        Vector3 mouseTempLook = autoShootPoint;
        mouseTempLook -= transform.position;
        mouseTempLook.x *= (isFacingRight() ? -1 : 1);
        yield return null;



        Vector2 fromPosition = firePostion.position;
        Vector2 target = autoShootPoint + aimOffset;


        float beginAngle = Vector2ToAngle(target - fromPosition);
        //Debug.LogError("beginAngle" + beginAngle);
        Vector2 ballPos = fromPosition;


        float cloestAngleDistance = int.MaxValue;

        bool checkingPerAngle = true;

        while (checkingPerAngle)
        {
            int k = 0;
            Vector2 lastPos = fromPosition;
            bool isCheckingAngle = true;
            float clostestDistance = int.MaxValue;

            while (isCheckingAngle)
            {
                Vector2 shotForce = force * AngleToVector2(beginAngle);
                x1 = ballPos.x + shotForce.x * Time.fixedDeltaTime * (stepCheck * k);    //X position for each point is found
                y1 = ballPos.y + shotForce.y * Time.fixedDeltaTime * (stepCheck * k) - (-(Physics2D.gravity.y * gravityScale) / 2f * Time.fixedDeltaTime * Time.fixedDeltaTime * (stepCheck * k) * (stepCheck * k));    //Y position for each point is found

                float _distance = Vector2.Distance(target, new Vector2(x1, y1));
                if (_distance < clostestDistance)
                    clostestDistance = _distance;

                if ((y1 < lastPos.y) && (y1 < target.y))
                    isCheckingAngle = false;
                else
                    k++;

                lastPos = new Vector2(x1, y1);
            }

            if (clostestDistance >= cloestAngleDistance)
                checkingPerAngle = false;
            else
            {
                cloestAngleDistance = clostestDistance;

                if (isTargetRight)
                    beginAngle += stepAngle;
                else
                    beginAngle -= stepAngle;
            }
        }

        //set ik spine
        var lookAt = AngleToVector2(beginAngle) * 10;
        lookAt.x *= (isFacingRight() ? -1 : 1);

        yield return null;
        anim.SetTrigger("shoot");

        bool isCritical = false;
        if (Random.Range(0f, 1f) < criticalRateTemp)
        {
            isCritical = true;
            criticalMultiplier += 0.2f;

        }
        else
        {
            criticalMultiplier = 1.2f;
        }
        //Fire number arrow
        ArrowProjectile _tempArrow;
        WeaponEffect _finalWeaponEffect = weaponEffect;

        //_tempArrow = Instantiate(arrow, fromPosition, Quaternion.identity);
        NumberArrow _finalNumberArrow = BoostItemUI.Instance && BoostItemUI.Instance.currentNumberOfArrows == NumberArrow.Double ? NumberArrow.Double : numberArrow;
        switch (_finalNumberArrow)
        {
            case NumberArrow.Double:
                _tempArrow = Instantiate(arrow, fromPosition, Quaternion.identity);
                _tempArrow.Init(damageMin, damageMax, force * AngleToVector2(beginAngle + 1.5f), gravityScale, isCritical, weaponEffect, BoostItemUI.Instance.currentEffect, target.y, criticalMultiplier, this.gameObject);
                //shot second arrow
                _tempArrow = Instantiate(arrow, fromPosition, Quaternion.identity);
                _tempArrow.Init(damageMin, damageMax, force * AngleToVector2(beginAngle - 1.5f), gravityScale, isCritical, weaponEffect, BoostItemUI.Instance.currentEffect, target.y, criticalMultiplier, this.gameObject);
                break;
            default:
                _tempArrow = Instantiate(arrow, fromPosition, Quaternion.identity);
                _tempArrow.Init(damageMin, damageMax, force * AngleToVector2(beginAngle), gravityScale, isCritical, weaponEffect, BoostItemUI.Instance.currentEffect, target.y, criticalMultiplier, this.gameObject);
                break;
        }

        SoundManager.PlaySfx(soundShoot[Random.Range(0, soundShoot.Length)], soundShootVolume);

        StartCoroutine(ReloadingCo());
    }


    IEnumerator ReloadingCo()
    {
        isAvailable = false;
        lastShoot = Time.time;
        isLoading = true;
        yield return new WaitForSeconds(0.1f);
        // anim.SetBool("isLoading", true);

        while (Time.time < (lastShoot + shootRate)) { yield return null; }

        // anim.SetBool("isLoading", false);

        yield return new WaitForSeconds(0.2f);

        isAvailable = true;
        isLoading = false;
    }


    public static Vector2 AngleToVector2(float degree)
    {
        Vector2 dir = (Vector2)(Quaternion.Euler(0, 0, degree) * Vector2.right);

        return dir;
    }

    public float Vector2ToAngle(Vector2 vec2)
    {
        var angle = Mathf.Atan2(vec2.y, vec2.x) * Mathf.Rad2Deg;
        return angle;
    }
}
