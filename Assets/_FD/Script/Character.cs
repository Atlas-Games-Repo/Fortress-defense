using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour, IListener, ICanTakeDamage
{
    
    public virtual void Start()
    {
    }

   
    public bool IEnabled() {
        return this.enabled;
    }
    public virtual void Update()
    {
    }

    public virtual void LateUpdate()
    {
    }

    //ICanTakeDamage
    public virtual void TakeDamage(float damage, Vector2 force, Vector2 hitPoint, GameObject instigator, BODYPART bodyPart = BODYPART.NONE, WeaponEffect weaponEffect = null, WEAPON_EFFECT forceEffect = WEAPON_EFFECT.NONE) { }

    public virtual void Hit() { }
    public virtual void Dead() { }
    public virtual void GetFrezze() { }
    public virtual void GetPosion() { }

    //IListener
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
}
