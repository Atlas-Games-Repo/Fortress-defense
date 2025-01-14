using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Rendering;

[AddComponentMenu("ADDP/Enemy AI/Smart Enemy Ground Control")]
[RequireComponent(typeof(Controller2D))]
public class SmartEnemyGrounded : Enemy, ICanTakeDamage, IGetTouchEvent
{
    [HideInInspector] public bool isPet;
    public bool isSocking { get; set; }
    public bool isDead { get; set; }

    public bool magnet = false;
    [HideInInspector] public Vector3 magnetPos;
    [HideInInspector] public float magnetAttractionSpeed;
    [HideInInspector] public float minMagnetDistance = 1;
    [HideInInspector] public Vector3 velocity;
    private Vector2 _direction;
    [HideInInspector] public Controller2D controller;

    float velocityXSmoothing = 0;
    Vector2 pushForce;
    private float _directionFace;

    [Header("Manul options")] public bool is_targeted;


    [Header("New")] bool allowCheckAttack = true;

    EnemyRangeAttack rangeAttack;
    EnemyMeleeAttack meleeAttack;
    EnemyThrowAttack throwAttack;
    EnemyCallMinion callMinion;
    SpawnItemHelper spawnItem;


    [Space(3)] [Header("Spawning from Underground")]
    public bool spawnFromUnderground;

    public GameObject undergroundSandPile;
    private GameObject _spawningArea;
    private Vector2 _spawnPos;
    public SpriteRenderer[] characterSprites;
    public float undergroundPileScale = 1f;
    public float climbingTime = 1f;
    public float maskYScale = 2f;
    public float climbDepth = 1f;
    private bool _canMove = false;
    public float xOffset = 0.3f;
    public float secondaryHeight = 0.8f;
    public float xDistanceFromEnemy = 1.5f;
    public float holeAnimationTime = 0.5f;
    public string warriorTag = "Warrior";
    private float _zPos;
    public GameObject shadow;
    private float _initialMoveSpeed;
    IEnumerator Climb()
    {
        yield return new WaitForSeconds(climbingTime);
        _canMove = true;
    }

    IEnumerator ClimbUp(Vector3 start, Vector3 end)
    {
        float elapsedTime = 0f;

        while (elapsedTime < climbingTime)
        {
            transform.position = Vector3.Lerp(start, end, elapsedTime / climbingTime);
            elapsedTime += Time.deltaTime;
            yield return null; 
        }

        GetComponent<SortingGroup>().sortingOrder = 0;
        transform.position = end;
        transform.position =
            new Vector3(transform.position.x, transform.position.y,
                _zPos);
        if (is_spine)
        {
            SetSkeletonAnimation(ANIMATION_STATE.WALK, true);

        }

        if (shadow)
        {
            shadow.SetActive(true);
        }
        _hole.DisableMask();
    }

    private UndergroundHole _hole;
    IEnumerator SpawnFromUnderTheGround()
    {
        yield return null;
        _canMove = false;
        if (shadow)
        {
            shadow.SetActive(false);
        }
        GetComponent<SortingGroup>().sortingOrder = -2;
        if (is_spine)
        {
            SetSkeletonAnimation(ANIMATION_STATE.IDLE, true);
        }
        else
        {
            foreach (SpriteRenderer sprite in characterSprites)
            {
                sprite.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
            }
        }
        StartCoroutine(Climb());
        GameObject[] playerWarriors = GameObject.FindGameObjectsWithTag(warriorTag);
        if (playerWarriors.Length == 0)
        {
            _spawningArea = GameObject.FindWithTag("UndergroundSpawn");
            Bounds bounds = _spawningArea.GetComponent<SpriteRenderer>().bounds;
            float randomX = Random.Range(bounds.min.x, bounds.max.x);
            float randomY = Random.Range(bounds.min.y, bounds.max.y);
            _spawnPos = new Vector2(randomX, randomY);
        }
        else
        {
            float[] warriorXPositions = new float[playerWarriors.Length];
            int chosenWarrior = 0;
            for (int i = 0; i < playerWarriors.Length; i++)
            {
                warriorXPositions[i] = 1000f;
                warriorXPositions[i] = playerWarriors[i].transform.position.x;
                float xPosition = Mathf.Min(warriorXPositions);
                if (xPosition == playerWarriors[i].transform.position.x)
                {
                    chosenWarrior = i;
                }
            }

            Transform chosenWarriorTransform = playerWarriors[chosenWarrior].transform;
            float spawnPosY = chosenWarriorTransform.position.y +
                              chosenWarriorTransform.GetComponent<BoxCollider2D>().offset.y -
                              (chosenWarriorTransform.GetComponent<BoxCollider2D>().size.y / 2) - secondaryHeight;
            float spawnPosX = chosenWarriorTransform.position.x - xDistanceFromEnemy;
            _spawnPos = new Vector2(spawnPosX, spawnPosY);
        }

        _hole = Instantiate(undergroundSandPile, _spawnPos, Quaternion.identity)
            .GetComponent<UndergroundHole>();
        _hole.Init(climbingTime, maskYScale, undergroundPileScale, holeAnimationTime, is_spine);
        transform.position = new Vector3(_spawnPos.x + xOffset, _spawnPos.y - climbDepth);
        Vector2 groundPos = new Vector2(transform.position.x, transform.position.y + secondaryHeight);
        if (is_spine)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y,
                _hole.meshMask.transform.position.z + 10);
            _zPos = transform.position.z;
        }

        yield return new WaitForSeconds(holeAnimationTime);
        StartCoroutine(ClimbUp(transform.position, groundPos));
    }

    public void StartClimbing()
    {
        StartCoroutine(SpawnFromUnderTheGround());
    }

    public override void Start()
    {
      

        base.Start();
        controller = GetComponent<Controller2D>();
        _direction = isFacingRight() ? Vector2.right : Vector2.left;

        if ((_direction == Vector2.right && startBehavior == STARTBEHAVIOR.WALK_LEFT) ||
            (_direction == Vector2.left && startBehavior == STARTBEHAVIOR.WALK_RIGHT))
        {
            Flip();
        }

        isPlaying = true;
        isSocking = false;

        controller.collisions.faceDir = 1;

        rangeAttack = GetComponent<EnemyRangeAttack>();
        meleeAttack = GetComponent<EnemyMeleeAttack>();
        throwAttack = GetComponent<EnemyThrowAttack>();
        callMinion = GetComponent<EnemyCallMinion>();

        if (rangeAttack && rangeAttack.GunObj)
            rangeAttack.GunObj.SetActive(attackType == ATTACKTYPE.RANGE);
        if (meleeAttack && meleeAttack.MeleeObj)
            meleeAttack.MeleeObj.SetActive(attackType == ATTACKTYPE.MELEE);

        spawnItem = GetComponent<SpawnItemHelper>();


        //Do Get Upgrade
        if (upgradedCharacterID != null)
        {
            //if (meleeAttack)
            //{
            //    meleeAttack.dealDamage = upgradedCharacterID.UpgradeMeleeDamage;
            //    meleeAttack.criticalPercent = upgradedCharacterID.UpgradeCriticalDamage;
            //}
            if (rangeAttack)
            {
                rangeAttack.damage = upgradedCharacterID.UpgradeRangeDamage;
            }
        }
        _initialMoveSpeed = moveSpeed;
    }



    public override void Update()
    {
        base.Update();
        HandleAnimation();

        if (enemyState != ENEMYSTATE.WALK || GameManager.Instance.State != GameManager.GameState.Playing)
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
        else if (!isPlaying || isSocking || enemyEffect == ENEMYEFFECT.SHOKING)
        {
            velocity = Vector2.zero;
            return;
        }

        float targetVelocityX = 0;
        if (spawnFromUnderground)
        {
            if (_canMove)
            {
                targetVelocityX = _direction.x * moveSpeed;
            }
            else
            {
                targetVelocityX = 0;
            }
        }
        else
        {
            targetVelocityX = _direction.x * moveSpeed;
        }

        if (isSocking || enemyEffect == ENEMYEFFECT.SHOKING)
        {
            targetVelocityX = 0;
        }

        if (enemyState != ENEMYSTATE.WALK || enemyEffect == ENEMYEFFECT.FREEZE)
            targetVelocityX = 0;

        if (isStopping || isStunning)
            targetVelocityX = 0;


        if ((_direction.x > 0 && controller.collisions.right) || (_direction.x < 0 && controller.collisions.left))
            velocity.x = 0;
        if (!magnet)
        {
            controller.Move(velocity * Time.deltaTime * multipleSpeed, false, isFacingRight());
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing,
                (controller.collisions.below) ? 0.1f : 0.2f);
            velocity.y += -gravity * Time.deltaTime;
        }
        else
        {
            controller.Move(velocity * Time.deltaTime * multipleSpeed, false, isFacingRight());
            velocity.x = 0;
            velocity.y += -0;
            if (Vector3.Distance(transform.position, magnetPos) > minMagnetDistance)
            {
                float translationDistance = magnetAttractionSpeed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, magnetPos, translationDistance);
            }
        }

        if (controller.collisions.above || controller.collisions.below)
            velocity.y = 0;

        if (isPlaying && isPlayerDetected && allowCheckAttack && enemyEffect != ENEMYEFFECT.FREEZE)
        {
            CheckAttack();
        }
        // set slow down rate here : 
        if (!isPet)
        {
            moveSpeed = _initialMoveSpeed * GlobalValue.SlowDownRate;
        }
    }

    void Flip()
    {
        _direction = -_direction;
        transform.rotation =
            Quaternion.Euler(new Vector3(transform.rotation.x, isFacingRight() ? 0 : 180, transform.rotation.z));
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

    public void CallMinion()
    {
        AnimSetTrigger("callMinion");
        SetEnemyState(ENEMYSTATE.ATTACK);
        allowCheckAttack = false;
    }

    void CheckAttack()
    {
        //CHECK AND CALL MINION IF THIS ENEMY HAS SCRIPT CALLMINION
        switch (attackType)
        {
            case ATTACKTYPE.RANGE:
                if (rangeAttack.AllowAction())
                {
                    SetEnemyState(ENEMYSTATE.ATTACK);

                    if (rangeAttack.CheckPlayer(isFacingRight()))
                    {
                        rangeAttack.Action();
                        AnimSetTrigger("shoot");
                        SetSkeletonAnimation(ANIMATION_STATE.ATTACK, true);
                        DetectPlayer();
                    }
                    else if (!rangeAttack.isAttacking && enemyState == ENEMYSTATE.ATTACK)
                    {
                        SetEnemyState(ENEMYSTATE.WALK);
                    }
                }

                break;

            case ATTACKTYPE.WIZARD:
                if (rangeAttack.AllowAction())
                {
                    SetEnemyState(ENEMYSTATE.ATTACK);

                    if (rangeAttack.CheckPlayer(isFacingRight()))
                    {
                        rangeAttack.Action();
                        AnimSetTrigger("wizard_attack_dark");
                        SetSkeletonAnimation(ANIMATION_STATE.IDLE, true);
                        DetectPlayer();
                    }
                    else if (!rangeAttack.isAttacking && enemyState == ENEMYSTATE.ATTACK)
                    {
                        SetEnemyState(ENEMYSTATE.WALK);
                    }
                }

                break;
            case ATTACKTYPE.MELEE:
                if (meleeAttack.AllowAction())
                {
                    if (meleeAttack.CheckPlayer(isFacingRight()))
                    {
                        SetEnemyState(ENEMYSTATE.ATTACK);
                        meleeAttack.Action();
                        AnimSetTrigger("melee");
                        SetSkeletonAnimation(ANIMATION_STATE.ATTACK, true);
                    }
                    else if (!meleeAttack.isAttacking && enemyState == ENEMYSTATE.ATTACK)
                    {
                        SetEnemyState(ENEMYSTATE.WALK);
                    }
                }

                break;

            case ATTACKTYPE.THROW:
                if (throwAttack.AllowAction())
                {
                    SetEnemyState(ENEMYSTATE.ATTACK);

                    if (throwAttack.CheckPlayer())
                    {
                        throwAttack.Action();
                        AnimSetTrigger("throw");
                        SetSkeletonAnimation(ANIMATION_STATE.ATTACK, true);
                    }
                    else if (!throwAttack.isAttacking && enemyState == ENEMYSTATE.ATTACK)
                    {
                        SetEnemyState(ENEMYSTATE.WALK);
                    }
                }

                break;
            default:
                break;
        }
    }

    void AllowCheckAttack()
    {
        allowCheckAttack = true;
    }

    void HandleAnimation()
    {
        AnimSetFloat("speed", Mathf.Abs(velocity.x));
        //AnimSetBool ("isRunning", Mathf.Abs (velocity.x) > walkSpeed);
        //AnimSetBool("isStunning", isStunning);
    }

    public void SetForce(float x, float y)
    {
        velocity = new Vector3(x, y, 0);
    }

    public void AnimMeleeAttackStart()
    {
        meleeAttack.Check4Hit();
    }

    public void AnimMeleeAttackEnd()
    {
        meleeAttack.EndCheck4Hit();
    }

    public void AnimThrow()
    {
        throwAttack.Throw(isFacingRight());
    }

    public void AnimShoot()
    {
        rangeAttack.Shoot(isFacingRight());
    }

    public void AnimCallMinion()
    {
        callMinion.CallMinion(isFacingRight());
        Invoke("AllowCheckAttack", 2);
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

        if (spawnItem && spawnItem.spawnWhenDie)
            spawnItem.Spawn();

        AnimSetBool("isDead", true);
        SetSkeletonAnimation(ANIMATION_STATE.DEAD);
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

        if (spawnItem && spawnItem.spawnWhenHit)
            spawnItem.Spawn();

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
            SpawnSystemHelper.GetNextObject(disableFX, true).transform.position =
                spawnDisableFX != null ? spawnDisableFX.position : transform.position;
        gameObject.SetActive(false);
    }

    public void TouchEvent()
    {
        GameObject[] archers = GameObject.FindGameObjectsWithTag("Player");
        foreach (var archer in archers)
        {
            archer.GetComponent<Player_Archer>().manual_targeted_enemy = this.gameObject;
        }
    }

    private float _mainMoveSpeed;
    private Transform _enemyParent;

    public void HitLog(float rollBackTime, Transform logTransform)
    {
        StartCoroutine(RollBackForLog(rollBackTime, logTransform));
    }
    IEnumerator RollBackForLog(float rollBackTime,Transform logTransform)
    {
        yield return new WaitForSeconds(0.01f);
        if (transform.parent)
        {
            _enemyParent = transform.parent;
        }

        _mainMoveSpeed = moveSpeed;
        moveSpeed = 0;
        transform.SetParent(logTransform);
        yield return new WaitForSeconds(rollBackTime);
        transform.SetParent(_enemyParent);
        moveSpeed = _mainMoveSpeed;
    }
}