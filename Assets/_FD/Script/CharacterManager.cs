using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance;
    //public GameObject[] listCharacter;
    public GameObject[] spawnPoints;
    BuyCharacterBtn currentBtnPicked;
    void Start()
    {
        Instance = this;
        TurnListSpawn(false);
    }

    void TurnListSpawn(bool enable)
    {
        foreach (var point in spawnPoints)
        {
            point.SetActive(enable);
        }
    }
    LayerMask _petLayer;
    public void SpawnCharacter(BuyCharacterBtn _currentBtnPicked, LayerMask layer)
    {
        currentBtnPicked = _currentBtnPicked;
        _petLayer = layer;
        TurnListSpawn(true);
    }

    public void SpawnCharacterFromPoint(Vector3 pos)
    {
        var character = Instantiate(currentBtnPicked.character, pos, Quaternion.identity) as GameObject;
        character.GetComponent<SmartEnemyGrounded>().startBehavior = STARTBEHAVIOR.WALK_LEFT;
        character.GetComponent<SmartEnemyGrounded>().isPet = true;
        switch (character.GetComponent<SmartEnemyGrounded>().attackType)
        {
             case ATTACKTYPE.MELEE:
                 character.GetComponent<EnemyMeleeAttack>().targetLayer = _petLayer;
                 break;
             case ATTACKTYPE.RANGE:
                 character.GetComponent<EnemyRangeAttack>().enemyLayer = _petLayer;
                 break;
             case ATTACKTYPE.THROW:
                 character.GetComponent<EnemyThrowAttack>().targetPlayer = _petLayer;
                 break;
             case ATTACKTYPE.WIZARD:
                 character.GetComponent<EnemyWizardAttack>().enemyLayer = _petLayer;
                 break;
        }
        character.GetComponent<CheckTargetHelper>().targetLayer = _petLayer;
        character.layer = 31;
        character.gameObject.layer = LayerMask.NameToLayer("Player");
        character.gameObject.tag = "Warrior";
        currentBtnPicked.AddCharacter(character);
        TurnListSpawn(false);
    }
}
