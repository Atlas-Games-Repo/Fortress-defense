using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[AddComponentMenu("ADDP/Enemy AI/Melee Attack")]
public class EnemyMeleeAttack : MonoBehaviour
{
    public LayerMask targetLayer;
    public Transform checkPoint;
    //public GameObject meleeDamageObj;
    [ReadOnly] public int maxTargetPerHit = 1;       //how many target damaged per hit
    public float radiusCheck = 1;
    public float dealDamage = 20;

    [Range(1, 100)]
    public int criticalPercent = 10;
    public float meleeRate = 1;
    float lastShoot = -999;
    public bool isAttacking { get; set; }
    [HideInInspector] public GameObject MeleeObj;
    [Range(0, 1)]
    public float soundAttacksVol = 0.5f;
    public AudioClip[] soundAttacks;
    WeaponEffect hasWeaponEffect;
    [Space(3)] [Header("Night Mode")] public float customNightMultiplier = 2f;
    public bool useCustomNightMultiplierOnly = false;

    void Start()
    {
        float initialDamage = dealDamage;
        if (GameLevelSetup.Instance && GameLevelSetup.Instance.NightMode())
        {
            if (useCustomNightMultiplierOnly)
            {
                dealDamage = customNightMultiplier * initialDamage;
            }
            else
            {
                dealDamage = initialDamage * GameLevelSetup.Instance.NightModeXpMultiplier();
            }
        }
    }
    
    public bool AllowAction()
    {
        return Time.time - lastShoot > meleeRate;
    }

    // Update is called once per frame
    bool isFacingRight = false;

    public bool CheckPlayer(bool _isFacingRight)
    {
        isFacingRight = _isFacingRight;
        RaycastHit2D hit = Physics2D.CircleCast(checkPoint.position, radiusCheck, Vector2.zero, 0, targetLayer);
        if (hit)
            return true;
        else
            return false;
    }

    public void Action()
    {


        lastShoot = Time.time;
    }


    void EndAttack()
    {
        isAttacking = false;
    }

    /// <summary>
    /// Called by Enemy
    /// </summary>
    public void Check4Hit()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(checkPoint.position, radiusCheck * 1.2f, Vector2.zero, 0, targetLayer);
        int counterHit = 0;
        if (hits.Length > 0)
        {
            foreach (var hit in hits)
            {
                if (counterHit < maxTargetPerHit)
                {
                    var takeDamage = (ICanTakeDamage)hit.collider.gameObject.GetComponent(typeof(ICanTakeDamage));
                    if (takeDamage != null)
                    {
                        float _damage = dealDamage + (int)(Random.Range(-0.1f, 0.1f) * dealDamage);
                        if (Random.Range(0, 100) < criticalPercent)
                        {
                            _damage *= 2;
                            FloatingTextManager.Instance.ShowText("CRIT!", Vector3.up, Color.red, hit.collider.gameObject.transform.position, 30);
                        }

                        if (hasWeaponEffect != null)
                        {
                            takeDamage.TakeDamage(_damage, Vector2.zero, hit.point, gameObject, BODYPART.NONE, hasWeaponEffect);
                        }
                        else
                            takeDamage.TakeDamage(_damage, Vector2.zero, hit.point, gameObject);

                        counterHit++;
                    }

                    if (soundAttacks.Length > 0)
                        SoundManager.PlaySfx(soundAttacks[Random.Range(0, soundAttacks.Length)], soundAttacksVol);
                }
            }
        }

        //if (hit)
        //{
        //    var takeDamage = (ICanTakeDamage)hit.collider.gameObject.GetComponent(typeof(ICanTakeDamage));
        //    if (takeDamage != null)
        //    {
        //        float _damage = dealDamage + (int)( Random.Range(-0.1f, 0.1f)* dealDamage);
        //        if (Random.Range(0, 100) < criticalPercent)
        //        {
        //            _damage *= 2;
        //            FloatingTextManager.Instance.ShowText("CRIT!", Vector3.up, Color.red, hit.collider.gameObject.transform.position,30);
        //        }

        //        if (hasWeaponEffect!=null)
        //        {
        //            takeDamage.TakeDamage(_damage, Vector2.zero, hit.point, gameObject, BODYPART.NONE, hasWeaponEffect);
        //        }
        //        else
        //            takeDamage.TakeDamage(_damage, Vector2.zero, hit.point, gameObject);
        //    }

        //    if (soundAttacks.Length > 0)
        //        SoundManager.PlaySfx(soundAttacks[Random.Range(0, soundAttacks.Length)], soundAttacksVol);
        //}
    }

    public void EndCheck4Hit()
    {
        //meleeDamageObj.SetActive (false);

        CancelInvoke();
        Invoke("EndAttack", 1);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(checkPoint.position, radiusCheck);
    }
}
