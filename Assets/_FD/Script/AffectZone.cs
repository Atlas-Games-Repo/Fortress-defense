using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AffectZone : MonoBehaviour
{
    [ReadOnly] public bool isActived = false;

    [Header("LIGHTNING")] public float lightingActiveTime = 3;
    public float lightingDamage = 10;
    public GameObject lightingFX;
    public float lightingRate = 1;
    public AudioClip lightingSound;
    [Header("FIRE")] public float fireAffectTime = 3;
    public float fireDamage = 10;
    public GameObject fireFX;
    public float fireRate = 1;
    public AudioClip fireSound;

    [Header("FROZEN")] public float frozenActiveTime = 3;
    public float frozenAffectTime = 3;
    public float frozenDamage = 10;
    public GameObject frozenFX;
    public AudioClip forzenSound;
    
    [Header("DARK")]
    public float darkActiveTime = 3;
    public float darkAffectTime = 3;
    public float darkDamage = 10;
    public GameObject darkFX;
    public AudioClip darkSound;
    
    [Header("POISON")] public float poisonActiveTime = 3;
    public float poisonAffectTime = 3;
    public float poisonDamage = 10;
    public GameObject poisonFX;
    public float poisonRate = 0.5f;
    public AudioClip poisonSound;

    [Header("MAGNET")] public float magnetRate = 3f;
    public float magnetActiveTime = 3;
    public GameObject magnetIcon;
    public float magnetScaleTime = 0.5f;
    public float elapsedTime = 0;
    public float magnetDamage = 3;
    private Vector3 _initialScale = new Vector3(0, 0, 0);
    private Vector3 _targetScale = new Vector3(1, 1, 1);
    public float magnetAttractionSpeed = 5f;
     GameObject _magnet;
    [HideInInspector]public List<Enemy> tempMagnetList;
    public float minMagnetDistance = 0.2f;
    public AudioClip magnetSound;

    [Header("AERO")]
    public float aeroRate = 3f;
    public float aeroActiveTime = 3;
    public GameObject aeroIcon;
    public float aeroScaleTime = 0.5f;
    public float elapsedAeroTime = 0;
    public float aeroDamage = 3;
    private Vector3 _initialAeroScale = new Vector3(0, 0, 0);
    private Vector3 _targetAeroScale = new Vector3(1, 1, 1);
    public float aeroAttractionSpeed = 5f;
     GameObject _aero;
    [HideInInspector]public List<Enemy> tempAeroList;
    public float minAeroDistance = 0.2f;
    public AudioClip aeroSound;
    // Start is called before the first frame update
    public List<Enemy> listEnemyInZone;
    AffectZoneType zoneType;


    [Header("Armagdon")] public GameObject fireBall;
    public float aramgdonFallTime = 1.3f;
    private GameObject[] _spawnedArmagdons;
    public Vector2[] fireBallSpawnOffset;
    public AudioClip impaceSfx;
    public float offset;
    public float fireBallSpeed = 2f;

    [Header("Fortress wall")] public GameObject defenseWall;
    public Transform wallPosition;
    public int wallHealth = 150;
    public AffectZoneType getAffectZoneType
    {
        get { return zoneType; }
    }
    [Range(1,500)]
    public int min_xp_consum, max_xp_consum;


    Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        listEnemyInZone = new List<Enemy>();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        isActived = false;
    }

    public void Active(AffectZoneType _type)
    {

        if (!isActived)
        {
            zoneType = _type;
            StartCoroutine(ActiveCo());
            switch (zoneType)
            {
                case AffectZoneType.Lighting:
                    StartCoroutine(StopActiveCo());
                    break;
                case AffectZoneType.Fire:
                    StartCoroutine(StopActiveCo());
                    break;
                case AffectZoneType.Poison:
                    StartCoroutine(StopActiveCo());
                    break;
                case AffectZoneType.Magnet:
                    StartCoroutine(StopActiveCo());
                    break;
                case AffectZoneType.Aero:
                    StartCoroutine(StopActiveCo());
                    break;
                case AffectZoneType.Frozen:
                    StartCoroutine(StopActiveCo());
                    break;
                case AffectZoneType.Dark:
                    StartCoroutine(StopActiveCo());
                    break;
            }
            // Consume XP from user.
            int XPConsume = AffectZoneManager.Instance.XPconsume(_type);
            if (GameManager.Instance.currentExp < XPConsume)
                return; // dont do anything if accidently we end up here and we dont have enough xp to spend
            GameManager.Instance.currentExp -= XPConsume;
            FloatingTextManager.Instance.ShowText("-" + XPConsume + " XP", Vector2.up * 1, Color.red, transform.position,40);
        }
    }
    

    IEnumerator ActiveCo()
    {
        yield return new WaitForSeconds(Time.fixedDeltaTime);
        yield return null;
        isActived = true;
        if (anim)
            anim.SetBool("isActivating", true);
        while (true)
        {
            if (listEnemyInZone.Count > 0)
            {
                List<Enemy> _tempList = new List<Enemy>(listEnemyInZone);
                foreach (var target in _tempList)
                {
                    if (target.gameObject != null)
                    {
                        var _weaponFX = new WeaponEffect();

                        switch (zoneType)
                        {
                            case AffectZoneType.Lighting:
                                _weaponFX.effectType = WEAPON_EFFECT.LIGHTING;
                                target.TakeDamage(lightingDamage, Vector2.zero, target.gameObject.transform.position,
                                    gameObject, BODYPART.NONE, _weaponFX);
                                if (lightingFX)
                                    SpawnSystemHelper.GetNextObject(lightingFX, true).transform.position =
                                        target.gameObject.transform.position;
                                CameraShake.instance.StartShake(0.05f, 0.05f);
                                SoundManager.PlaySfx(lightingSound);
                                yield return new WaitForSeconds(Random.Range(0.1f, 0.2f));
                                break;
                            case AffectZoneType.Fire:
                                target.TakeDamage(fireDamage, Vector2.zero, target.gameObject.transform.position,
                                    gameObject);
                               // target.Freeze(fireAffectTime, gameObject);
                                if (fireFX)
                                {
                                    var _fx = SpawnSystemHelper.GetNextObject(fireFX, true);
                                    _fx.GetComponent<AutoDestroy>().Init(fireAffectTime);
                                    _fx.transform.position = target.gameObject.transform.position;
                                }

                                SoundManager.PlaySfx(fireSound);
                                yield return new WaitForSeconds(Random.Range(0.1f, 0.2f));
                                break;
                            case AffectZoneType.Frozen:
                                target.TakeDamage(frozenDamage, Vector2.zero, target.gameObject.transform.position,
                                    gameObject);
                                target.Freeze(frozenAffectTime, gameObject);
                                if (frozenFX)
                                {
                                    var _fx = SpawnSystemHelper.GetNextObject(frozenFX, true);
                                    _fx.GetComponent<AutoDestroy>().Init(frozenAffectTime);
                                    _fx.transform.position = target.gameObject.transform.position;
                                }

                                SoundManager.PlaySfx(forzenSound);
                                yield return new WaitForSeconds(Random.Range(0.1f, 0.2f));
                                break;
                            case AffectZoneType.Dark:
                                _weaponFX = new WeaponEffect();
                                _weaponFX.effectType = WEAPON_EFFECT.DARK;
                                target.TakeDamage(darkDamage, Vector2.zero, target.gameObject.transform.position,
                                    gameObject, BODYPART.NONE, _weaponFX);
                                if (darkFX)
                                    SpawnSystemHelper.GetNextObject(darkFX, true).transform.position =
                                        target.gameObject.transform.position;
                                SoundManager.PlaySfx(darkSound);
                              
                                yield return new WaitForSeconds(Random.Range(0.1f, 0.2f));
                                break;

                            case AffectZoneType.Poison:
                                target.Poison(poisonDamage, poisonActiveTime, gameObject);
                                if (poisonFX)
                                {
                                    var _fx = SpawnSystemHelper.GetNextObject(poisonFX, true);
                                    _fx.GetComponent<AutoDestroy>().Init(poisonAffectTime);
                                    _fx.transform.position = target.gameObject.transform.position;
                                }

                                SoundManager.PlaySfx(poisonSound);
                                break;
                            case AffectZoneType.Magnet:

                                if (!_magnet)
                                {
                                    _magnet = Instantiate(magnetIcon, transform.position, Quaternion.identity, transform);
                                    _initialScale = _magnet.transform.localScale;
                                    if (elapsedTime < magnetScaleTime)
                                    {
                                        elapsedTime += Time.deltaTime;
                                        float t = elapsedTime / magnetScaleTime;
                                        _magnet.transform.localScale = Vector3.Lerp(_initialScale, _targetScale, t);
                                    }
                                }
                                if (!target.GetComponent<SmartEnemyGrounded>().magnet)
                                {
                                    tempMagnetList.Add(target);
                                }
                                target.GetComponent<SmartEnemyGrounded>().magnet = true;
                                target.GetComponent<SmartEnemyGrounded>().magnetPos = transform.position;
                                target.GetComponent<SmartEnemyGrounded>().minMagnetDistance = minMagnetDistance;
                                target.GetComponent<SmartEnemyGrounded>().magnetAttractionSpeed = magnetAttractionSpeed;
                                target.TakeDamage(magnetDamage, Vector2.zero, target.gameObject.transform.position,
                                    gameObject, BODYPART.NONE,null);
                                SoundManager.PlaySfx(magnetSound);
                                // code for magnet
                                break;
                            case AffectZoneType.Aero:

                                if (!_aero)
                                {
                                    _aero = Instantiate(aeroIcon, transform.position, Quaternion.identity, transform);
                                    _initialAeroScale = _aero.transform.localScale;
                                    if (elapsedAeroTime < aeroScaleTime)
                                    {
                                        elapsedAeroTime += Time.deltaTime;
                                        float t = elapsedTime / aeroScaleTime;
                                        _aero.transform.localScale = Vector3.Lerp(_initialAeroScale, _targetAeroScale, t);
                                    }
                                }
                                if (!target.GetComponent<SmartEnemyGrounded>().magnet)
                                {
                                    tempAeroList.Add(target);
                                }
                                target.GetComponent<SmartEnemyGrounded>().magnet = true;
                                target.GetComponent<SmartEnemyGrounded>().magnetPos = transform.position;
                                target.GetComponent<SmartEnemyGrounded>().minMagnetDistance = minAeroDistance;
                                target.GetComponent<SmartEnemyGrounded>().magnetAttractionSpeed = aeroAttractionSpeed;
                                target.TakeDamage(aeroDamage, Vector2.zero, target.gameObject.transform.position,
                                    gameObject, BODYPART.NONE,null);
                                                                    SoundManager.PlaySfx(aeroSound);
                                // code for magnet
                                break;
                            case AffectZoneType.DefenseWall:
                                ActivateDefenseWall();
                                break;
                        }
                    }
                }
            }

            switch (zoneType)
            {
                case AffectZoneType.Lighting:
                    yield return new WaitForSeconds(lightingRate);
                    break;
                case AffectZoneType.Fire:
                    yield return new WaitForSeconds(fireRate);
                    break;
                case AffectZoneType.Frozen:
                    Stop();
                    break;
                case AffectZoneType.Dark:
                    Stop();
                    break;
                case AffectZoneType.Poison:
                    yield return new WaitForSeconds(poisonRate);
                    break;
                case AffectZoneType.Magnet:
                    yield return new WaitForSeconds(magnetRate);
                    break;
                case AffectZoneType.Aero:
                    yield return new WaitForSeconds(aeroRate);
                    break;
                
            }

            yield return null;
        }
    }

    IEnumerator StopActiveCo()
    {
        float delay = 0;
        switch (zoneType)
        {
            case AffectZoneType.Lighting:
                delay = lightingActiveTime;
                break;
            case AffectZoneType.Fire:
                delay = fireAffectTime;
                break;
            case AffectZoneType.Frozen:
                delay = frozenActiveTime;
                break;
            case AffectZoneType.Dark:
                delay = darkActiveTime;
                break;
            case AffectZoneType.Poison:
                delay = poisonActiveTime;
                break;
            case AffectZoneType.Magnet:
                delay = magnetActiveTime;
                // stop magnet 
                break;
            case AffectZoneType.Aero:
                delay = aeroActiveTime;
                // stop magnet 
                break;
        }

        yield return new WaitForSeconds(delay);
        if (zoneType == AffectZoneType.Magnet)
        {
            StopMagnet();
        }else if (zoneType == AffectZoneType.Aero)
        {
            StopAero();
        }
        
        Stop();
    }

    void StopMagnet()
    {
        for (int i = 0; i < tempMagnetList.Count; i++)
        {
            tempMagnetList[i].GetComponent<SmartEnemyGrounded>().magnet = false;
        }
        Destroy(_magnet);
        tempMagnetList.Clear();   
    }
    void StopAero()
    {
        for (int i = 0; i < tempAeroList.Count; i++)
        {
            tempAeroList[i].GetComponent<SmartEnemyGrounded>().magnet = false;
        }
        Destroy(_aero);
        tempAeroList.Clear();   
    }
    public void Stop()
    {

        AffectZoneManager.Instance.FinishAffect();
        StopAllCoroutines();
        isActived = false;
        if (anim)
            anim.SetBool("isActivating", false);
        gameObject.SetActive(false);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            var enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                if (!listEnemyInZone.Contains(enemy))
                {
                    listEnemyInZone.Add(enemy);
                 //   Debug.LogError("Add: " + collision.gameObject.name + "list: " + listEnemyInZone.Count + " Zone: " + this.gameObject.name);
                }
            }
        }
        // Debug.LogError(collision.gameObject + "list: " + listEnemyInZone.Count);
    }

    public void ActivateArmagdon()
    {
        Vector2 firstOffsetTarget = new Vector2();
        for (int i = 0; i < fireBallSpawnOffset.Length; i++)
        {
            Vector2 newTarget = new Vector2();
            if (i == 0)
            {
                firstOffsetTarget = fireBallSpawnOffset[i];
            }
            else
            {
                newTarget = fireBallSpawnOffset[i] - firstOffsetTarget;
            }
            var spawnedFireBall = SpawnSystemHelper.GetNextObject(fireBall, true);
            spawnedFireBall.transform.position = (Vector2)transform.position + fireBallSpawnOffset[i] + firstOffsetTarget;
            StartCoroutine(ThrowFireBall(spawnedFireBall.gameObject, spawnedFireBall.transform.position, (Vector2)transform.position + newTarget));
        }
      
    }

    public void ActivateDefenseWall()
    {
        var newWall = SpawnSystemHelper.GetNextObject(defenseWall);
        newWall.transform.position = wallPosition.position;
        newWall.GetComponent<DefenseWall>().Init(wallHealth,GetComponent<AffectZone>());
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            var enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                listEnemyInZone.Remove(enemy);
//                Debug.LogError("Remove: " + collision.gameObject.name + "list: " + listEnemyInZone.Count + " Zone: " + this.gameObject.name);
            }
        }
        // Debug.LogError(collision.gameObject);
    }
    private IEnumerator ThrowFireBall(GameObject fireBall, Vector3 startPos, Vector3 target)
    {
        fireBall.transform.position = startPos;
       
       

        while (Vector3.Distance(fireBall.transform.position, target) > 0.01f)
        {
            Vector3 direction = (target - fireBall.transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            angle += offset;

            fireBall.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            fireBall.transform.Translate(direction * fireBallSpeed * Time.deltaTime,Space.World);
           print("hey");
            yield return null;
        }

        Destroy(fireBall);
        GameObject fireFx = SpawnSystemHelper.GetNextObject(fireFX, true);
        fireFx.GetComponent<AutoDestroy>().Init(fireAffectTime);
        fireFx.transform.position = target ;
        fireBall.transform.position = target;
        SoundManager.PlaySfx(impaceSfx);
        Active(AffectZoneType.Fire);
        CameraShake.instance.StartShake(0.1f, 0.1f);
    }
}
