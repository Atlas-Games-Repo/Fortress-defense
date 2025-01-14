using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemySpawn
{
    public float wait = 3;      //delay for first enemy
    public GameObject enemy;    //enemy spawned
    public int numberEnemy = 5;     //the number of enemy need spawned

    public float rate = 1;  //time delay spawn next enemy
    [Header("CUSTOM VALUE FOR ENEMY: 0 = No set")]
    [Tooltip("0: no custom")]
    public int customHealth = 0;
    [Tooltip("0: no custom")]
    public float customSpeed = 0;
    [Tooltip("0: no custom")]
    public float customAttackDmg = 0;
    [Header("BOSS PROPERTIES")]
    public isBoss boosType = isBoss.NONE;
    public enum isBoss { NONE, MINIBOSS, BOSS }; //is the enemy boos or not
    [Tooltip("0: no custom")]
    [Range(1, 4)]
    public float BossScale = 1;
    public int BossMinExp = 10;
    public int BossMaxExp = 20;
    public bool spawnFromUnderground = false;
}
