using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelWave : MonoBehaviour
{
    public enum LevelType
    {
        Endless,
        Normal
    }

    [Space(3)] 
    public EndlessWaveGenerator.EndlessEnemyWave[] enemiesList;
    public LevelType type = LevelType.Normal;
    public int defaultExp = 200;
    public EnemyWave[] Waves;
    [Range(0,1)]
    public float increaseEnemySpeedDifficultyRate = 0.05f;
    [Range(0,1)]
    public float increaseEnemyAttackDifficultyRate = 0.1f;
    [Range(0,1)]
    public float increaseEnemyHealthDifficultyRate = 0.1f;
    [Range(0,1)]
    public float increaseEnemyAmountDifficultyRate = 0.4f;
    [Range(0,1)]
    public float increaseEnemyWaitDifficultyRate = 0.2f;
    [Range(0,1)]
    public float initialWaitAmount = 3;
    
    [Space(2)]
    [Header("Other settings")]
    public int level = 1;
    public Sprite backgroundSprite;
    public bool nightMode;
    public float nightMultiplierFixedAmount = 2f;
}
