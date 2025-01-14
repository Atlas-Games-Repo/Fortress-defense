using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DefenseWall : MonoBehaviour, ICanTakeDamage
{
    public Sprite[] states;
    private bool _isInit;
    private int health;
    private int currentHealth;
    private AffectZone _zone;
    // Start is called before the first frame update
    public void Init(int wallHealth,AffectZone zone)
    {
        if (!_isInit)
        {
            health = wallHealth;
            currentHealth = wallHealth;
            _isInit = true;
            _zone = zone;
        }
        
    }
 public void TakeDamage(float damage, Vector2 force, Vector2 hitPoint, GameObject instigator, BODYPART bodyPart = BODYPART.NONE, WeaponEffect weaponEffect = null, WEAPON_EFFECT forceEffect = WEAPON_EFFECT.NONE)
    {
        float _damage = damage;
        bool isExplosion = false;
        currentHealth -= (int)_damage;

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
            _zone.Stop();
        }
    }
  
    void Update()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (currentHealth > 0 && currentHealth <= health / 3)
        {
            renderer.sprite = states[0];
        }else if (currentHealth > health / 3 && currentHealth <= health * 2 / 3)
        {
            renderer.sprite = states[1];
        }
        else if(currentHealth > health * 2 /3)
        {
            renderer.sprite = states[2];
        }
        else
        {
            Destroy(gameObject);
        }
        
    }
    
}
