using System;
using System.Collections;
using System.Collections.Generic;
using RTLTMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EndlessWaveGenerator : LevelEnemyManager, IListener
{
    [Serializable]
    public class EndlessEnemyWave
    {
        public GameObject enemyObject;
        public int initialCount;
    }

    [SerializeField] private EndlessEnemyWave[] enemiesList;


    [HideInInspector] public EnemyWave wave;
    [HideInInspector] public List<EnemySpawn> enemySpawn;
    private GameObject[] _enemies;
    public float increaseEnemySpeedDifficultyRate = 0.05f;
    public float increaseEnemyAttackDifficultyRate = 0.1f;
    public float increaseEnemyHealthDifficultyRate = 0.1f;
    public float increaseEnemyAmountDifficultyRate = 0.4f;
    public float increaseEnemyWaitDifficultyRate = 0.2f;
    private float[] _enemyCounts;
    public float initialWaitAmount = 3;
    public LevelEnemyManager level_enemy_manager;
    [HideInInspector] public int waveCount;
    public GameObject WaveCountUI;

    int _totalEnemy, _currentSpawn;

    private void Start() {
        if (GameLevelSetup.Instance)
        {
        levelType = GameLevelSetup.Instance.type();
            if (levelType == LevelWave.LevelType.Normal) {
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
    void Awake()
    {
        if (GameLevelSetup.Instance)
        {
        levelType = GameLevelSetup.Instance.type();
            if (levelType == LevelWave.LevelType.Normal) {
                this.enabled = false;
                return;
            } else {
                WaveCountUI.SetActive(true);
            }

            enemiesList = GameLevelSetup.Instance.EndlessInitialWave();
            increaseEnemySpeedDifficultyRate = GameLevelSetup.Instance.IncreaseEnemySpeedDifficultyRate();
            increaseEnemyAttackDifficultyRate = GameLevelSetup.Instance.IncreaseEnemyAttackDifficultyRate();
            increaseEnemyHealthDifficultyRate = GameLevelSetup.Instance.IncreaseEnemyHealthDifficultyRate();
            increaseEnemyAmountDifficultyRate = GameLevelSetup.Instance.IncreaseEnemyAmountDifficultyRate();
            increaseEnemyWaitDifficultyRate = GameLevelSetup.Instance.IncreaseEnemyWaitDifficultyRate();
            initialWaitAmount = GameLevelSetup.Instance.InitialWaitAmount();
        }

        _enemies = new GameObject[enemiesList.Length];
        _enemyCounts = new float[enemiesList.Length];
        for (int a = 0; a < enemiesList.Length; a++)
        {
            _enemyCounts[a] = enemiesList[a].initialCount;
            _enemies[a] = enemiesList[a].enemyObject;
        }
    }


    // generate a new wave harder than last
    private void GenerateWave()
    {
        enemySpawn.Clear();

        // check if ran out of new enemies 
        int enemiesInUse;
        if ((waveCount * increaseEnemyAmountDifficultyRate) + 1 < _enemies.Length)
        {
            enemiesInUse = Convert.ToInt32(Mathf.Round((waveCount * increaseEnemyAmountDifficultyRate) + 1));
        }
        else
        {
            enemiesInUse = _enemies.Length;
        }

        // add new enemies to wave spwan 
        // add to the health, speed, and attack damage of enemies 

        for (int i = 0; i < enemiesInUse; i++)
        {
            EnemySpawn newSpawn = new EnemySpawn();
            newSpawn.enemy = _enemies[i];
            _enemyCounts[i] += increaseEnemyAmountDifficultyRate;
            newSpawn.numberEnemy = Convert.ToInt32(Mathf.Round(_enemyCounts[i]));
            newSpawn.wait = 1 / ((waveCount + 1) * increaseEnemyWaitDifficultyRate);
            newSpawn.customHealth =
                Convert.ToInt32(Mathf.Round(_enemies[i].GetComponent<SmartEnemyGrounded>().health *
                                            (1+(waveCount * increaseEnemyHealthDifficultyRate))));
            newSpawn.customSpeed =
                Convert.ToInt32(Mathf.Round(_enemies[i].GetComponent<SmartEnemyGrounded>().walkSpeed *
                                            (1+(waveCount * increaseEnemySpeedDifficultyRate))));


            var rangeAttack = _enemies[i].GetComponent<EnemyRangeAttack>();
            if (rangeAttack)
            {
                newSpawn.customAttackDmg += (1+(waveCount * increaseEnemyAttackDifficultyRate)) * rangeAttack.damage;
            }

            var meleeAttack = _enemies[i].GetComponent<EnemyMeleeAttack>();
            if (meleeAttack)
            {
                newSpawn.customAttackDmg +=
                    (1+(waveCount * increaseEnemyAttackDifficultyRate)) * meleeAttack.dealDamage;
            }

            var throwAttack = _enemies[i].GetComponent<EnemyThrowAttack>();
            if (throwAttack)
            {
                newSpawn.customAttackDmg += (1+(waveCount * increaseEnemyAttackDifficultyRate)) * throwAttack.damage;
            }


            enemySpawn.Add(newSpawn);
        }

        // set wave data ready for usage in LevelEnemyManager.cs for creation
        waveCount++;
        WaveCountUI.transform.GetChild(0).GetComponent<Text>().text = $"wave {waveCount}";
        wave.wait = initialWaitAmount;
        wave.enemySpawns = enemySpawn.ToArray();
    }

    public new void IPlay()
    {
        StartCoroutine(SpawnEnemyCo());
        //throw new System.NotImplementedException();
    }

    IEnumerator SpawnEnemyCo()
    {
        GenerateWave();
        EnemyWaves = new EnemyWave[1];
        EnemyWaves[0] = wave;

        foreach (GameObject enemy in listEnemySpawned)
        {
            Destroy(enemy);
        }

        listEnemySpawned.Clear();

        for (int i = 0; i < EnemyWaves.Length; i++)
        {
            yield return new WaitForSeconds(EnemyWaves[i].wait);
            for (int j = 0; j < EnemyWaves[i].enemySpawns.Length; j++)
            {
                var enemySpawn = EnemyWaves[i].enemySpawns[j];
                yield return new WaitForSeconds(enemySpawn.wait);
                for (int k = 0; k < enemySpawn.numberEnemy; k++)
                {
                    GameObject _temp = Instantiate(enemySpawn.enemy,
                        (Vector2)spawnPositions[Random.Range(0, spawnPositions.Length)].position,
                        Quaternion.identity) as GameObject;
                    if (enemySpawn.spawnFromUnderground)
                    {
                        _temp.GetComponent<SmartEnemyGrounded>().StartClimbing();
                    }
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

                        if (enemySpawn.boosType != EnemySpawn.isBoss.NONE)
                        {
                            bossManeger.enemy = _temp.GetComponent<Enemy>();
                            if (enemySpawn.BossScale > 1)
                                bossManeger.enemy.gameObject.transform.localScale =
                                    new Vector2(enemySpawn.BossScale, enemySpawn.BossScale);
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
                    //_temp.transform.localPosition = Vector2.zero;
                    listEnemySpawned.Add(_temp);

                
                   
                    _currentSpawn++;
                    MenuManager.Instance.UpdateEnemyWavePercent(_currentSpawn, _totalEnemy);

                    yield return new WaitForSeconds(enemySpawn.rate);
                }
            }

            while (IsEnemyAlive())
            {
                yield return new WaitForSeconds(0.1f);
            }

            EnemyWaves = new EnemyWave[0];
        }
    }

    void Update()
    {
        if (EnemyWaves.Length == 0)
        {
            StartCoroutine(SpawnEnemyCo());
        }
    }

    bool IsEnemyAlive()
    {
        for (int i = 0; i < listEnemySpawned.Count; i++)
        {
            if (listEnemySpawned[i].gameObject != null && listEnemySpawned[i].activeInHierarchy)
                return true;
        }

        return false;
    }
}
