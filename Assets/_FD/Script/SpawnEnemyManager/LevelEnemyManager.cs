using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEnemyManager : MonoBehaviour, IListener
{
    public static LevelEnemyManager Instance;
    public GameObject FX_Smoke, FX_Blow;
    public SimpleProjectile bullet;
    public Transform BossSpawnPoint;
    public Transform[] spawnPositions;
    public EnemyWave[] EnemyWaves;
    public BossUIManager bossManeger;
    int currentWave = 0;
    public List<GameObject> listEnemySpawned = new List<GameObject>();

    private void Awake()
    {
        Instance = this;
    }

    [HideInInspector]public int totalEnemy, currentSpawn;
    public LevelWave.LevelType levelType;
    // Start is called before the first frame update
    void Start()
    {
        if (GameLevelSetup.Instance)
        {
        levelType = GameLevelSetup.Instance.type();
            if (levelType == LevelWave.LevelType.Endless) {
                this.enabled = false;
                return;
            }
            EnemyWaves = GameLevelSetup.Instance.GetLevelWave();
        }

        //calculate number of enemies
        totalEnemy = 0;
        for (int i = 0; i < EnemyWaves.Length; i++)
        {
            for (int j = 0; j < EnemyWaves[i].enemySpawns.Length; j++)
            {
                var enemySpawn = EnemyWaves[i].enemySpawns[j];
                for (int k = 0; k < enemySpawn.numberEnemy; k++)
                {
                    totalEnemy++;
                }
            }
        }

        currentSpawn = 0;
    }

    IEnumerator SpawnEnemyCo()
    {
        for (int i = 0; i < EnemyWaves.Length; i++)
        {
            yield return new WaitForSeconds(EnemyWaves[i].wait);
            for (int j = 0; j < EnemyWaves[i].enemySpawns.Length; j++)
            {
                var enemySpawn = EnemyWaves[i].enemySpawns[j];
                yield return new WaitForSeconds(enemySpawn.wait);
                for (int k = 0; k < enemySpawn.numberEnemy; k++)
                {
                    Vector2 spawnPos = Vector2.zero;
                    if (enemySpawn.boosType == EnemySpawn.isBoss.NONE)
                        spawnPos = (Vector2)spawnPositions[Random.Range(0, spawnPositions.Length)].position;
                    else
                        spawnPos = (Vector2)BossSpawnPoint.position;
                    GameObject _temp = Instantiate(enemySpawn.enemy,spawnPos,Quaternion.identity) as GameObject;
                    var isEnemy = (Enemy)_temp.GetComponent(typeof(Enemy));
                    if (isEnemy != null)
                    {
                        isEnemy.disableFX = FX_Smoke;
                        if (enemySpawn.customHealth > 0)
                            isEnemy.health = enemySpawn.customHealth;
                        if (enemySpawn.customSpeed > 0)
                            isEnemy.walkSpeed = enemySpawn.customSpeed;
                        if (enemySpawn.customAttackDmg > 0)
                        {
                            var rangeAttack = _temp.GetComponent<EnemyRangeAttack>();
                            if (rangeAttack)
                            {
                                rangeAttack.damage = enemySpawn.customAttackDmg;
                            }

                            var meleeAttack = _temp.GetComponent<EnemyMeleeAttack>();
                            if (meleeAttack)
                                meleeAttack.dealDamage = enemySpawn.customAttackDmg;
                            var throwAttack = _temp.GetComponent<EnemyThrowAttack>();
                            if (throwAttack)
                            {
                                throwAttack.damage = enemySpawn.customAttackDmg;
                            }
                        }

                        var rangeAttack1 = _temp.GetComponent<EnemyRangeAttack>();
                        if (rangeAttack1)
                            rangeAttack1.bullet = bullet;
                        var meleeAttack1 = _temp.GetComponent<EnemyMeleeAttack>();
                        var throwAttack1 = _temp.GetComponent<EnemyThrowAttack>();
                        if (throwAttack1)
                        {
                            throwAttack1.FX_Blow = FX_Blow;
                            throwAttack1.FX_Smoke = FX_Smoke;
                        }

                        if (enemySpawn.boosType != EnemySpawn.isBoss.NONE)
                        {
                            bossManeger.enemy = _temp.GetComponent<Enemy>();
                                if (enemySpawn.BossScale > 1) {
                                    Vector2 scale = new Vector2(enemySpawn.BossScale, enemySpawn.BossScale);
                                bossManeger.enemy.gameObject.transform.localScale =
                                 bossManeger.enemy.gameObject.transform.localScale * scale;
                                }
                            bossManeger.bossType = enemySpawn.boosType;
                            bossManeger.enemy.gameObject.GetComponent<GiveExpWhenDie>().expMin =
                                enemySpawn.BossMinExp;
                            bossManeger.enemy.gameObject.GetComponent<GiveExpWhenDie>().expMax =
                                enemySpawn.BossMaxExp;

                            bossManeger.gameObject.SetActive(true);
                            bossManeger.enemy.is_boss = true;
                            AudioClip bossMusic = bossManeger.enemy.BossMusic != null
                                ? bossManeger.enemy.BossMusic
                                : SoundManager.Instance.BossMusicClip;
                            SoundManager.PlayMusic(bossMusic, 0.5f);
                        }
                    }

                    _temp.SetActive(false);
                    _temp.transform.parent = transform;

                    yield return new WaitForSeconds(0.1f);
                    _temp.SetActive(true);

                    listEnemySpawned.Add(_temp);

                    currentSpawn++;
                    MenuManager.Instance.UpdateEnemyWavePercent(currentSpawn, totalEnemy);

                    yield return new WaitForSeconds(enemySpawn.rate);
                }
            }

            //check all enemy killed

            while (isEnemyAlive())
            {
                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitForSeconds(0.5f);
            GameManager.Instance.Victory();
        }
    }


    bool isEnemyAlive()
    {
        for (int i = 0; i < listEnemySpawned.Count; i++)
        {
            if (listEnemySpawned[i].gameObject != null && listEnemySpawned[i].activeInHierarchy)
                return true;
        }

        return false;
    }

    public void IGameOver()
    {
        //throw new System.NotImplementedException();
    }

    public void IOnRespawn()
    {
        //throw new System.NotImplementedException();
    }

    public void IOnStopMovingOff()
    {
        //throw new System.NotImplementedException();
    }

    public void IOnStopMovingOn()
    {
        //throw new System.NotImplementedException();
    }

    public void IPause()
    {
        //throw new System.NotImplementedException();
    }

    public void IPlay()
    {
        StartCoroutine(SpawnEnemyCo());
        //throw new System.NotImplementedException();
    }

    public void ISuccess()
    {
        StopAllCoroutines();
        //throw new System.NotImplementedException();
    }

    public void IUnPause()
    {
        //throw new System.NotImplementedException();
    }
    public bool IEnabled() {
        return this.enabled;
    }
}

[System.Serializable]
public class EnemyWave
{
    public float wait = 3;
    public EnemySpawn[] enemySpawns;
}
