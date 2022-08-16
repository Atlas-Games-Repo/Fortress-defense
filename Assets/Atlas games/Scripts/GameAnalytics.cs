using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class GameAnalytics : MonoBehaviour
{
    // Start is called before the first frame update
    public AddAndUpgradePlayer[] Players;
    public float DPS, EDPS, ETH, EXPGPS, DIFF;
    void Start()
    {
        //File.AppendAllLines()
        StartCoroutine(update());
    }
    void Update()
    {

    }
    // Update is called once per frame
    IEnumerator update()
    {
        int number = 1;
        string filename = $"d:\\Projects\\Hadi Asadollahi\\Hadi Asadollahi\\FORTRESS DEFENSE\\Logs\\Analytics{number}.txt";
        while (true)
        {
            bool is_path_exist = System.IO.File.Exists(filename);
            if (is_path_exist)
            {
                number++;
                filename = $"d:\\Projects\\Hadi Asadollahi\\Hadi Asadollahi\\FORTRESS DEFENSE\\Logs\\Analytics{number}.txt";
            }
            else
            {

                break;
            }
            yield return new WaitForSeconds(0.01f);
        }
        while (true)
        {
            yield return new WaitForSeconds(1);
            float _EDPS = 0, _ETH = 0, _DPS = 0, _EXPGPS = 0, _DIFF = 0;
            foreach (GameObject enemy in LevelEnemyManager.Instance.listEnemySpawned)
            {

                if (enemy.activeInHierarchy && enemy.GetComponent<Enemy>().enemyState == ENEMYSTATE.ATTACK)
                {
                    if (enemy.GetComponent<Enemy>().attackType == ATTACKTYPE.MELEE)
                    {
                        _EDPS += enemy.GetComponent<EnemyMeleeAttack>().dealDamage;
                    }
                    if (enemy.GetComponent<Enemy>().attackType == ATTACKTYPE.RANGE)
                    {
                        _EDPS += enemy.GetComponent<EnemyRangeAttack>().damage;
                    }
                    if (enemy.GetComponent<Enemy>().attackType == ATTACKTYPE.THROW)
                    {
                        _EDPS += enemy.GetComponent<EnemyThrowAttack>().damage;
                    }

                    //_EXPGPS += (enemy.GetComponent<GiveExpWhenDie>().expMin + enemy.GetComponent<GiveExpWhenDie>().expMax) / 2;
                }
                foreach (AddAndUpgradePlayer item in Players)
                {
                    Player_Archer player = item.GetcurrentPlayer;
                    if (player.gameObject.activeInHierarchy && player.is_attacking)
                    {
                        _ETH += enemy.GetComponent<Enemy>().currentHealth;
                        break;
                    }
                }

            }
            foreach (AddAndUpgradePlayer item in Players)
            {
                Player_Archer player = item.GetcurrentPlayer;
                if (player.gameObject.activeInHierarchy && player.is_attacking)
                {
                    _DPS += (player.damageMin + player.damageMax) / 2;
                }
            }
            _EXPGPS = GameManager.Instance.currentExp;
            EDPS = _EDPS;
            ETH = _ETH;
            DPS = _DPS;
            EXPGPS = _EXPGPS;
            _DIFF = ETH - DPS - EXPGPS + EDPS;
            if (_DIFF < 0)
            {
                _DIFF = 0;
            }
            DIFF = ((float)_DIFF / (100 + (float)_DIFF)) * 100;
            DIFF = Mathf.Floor(DIFF);
            System.IO.StreamWriter file = new System.IO.StreamWriter(filename, true);
            file.Write($"{DPS}\t{ETH}\t{EDPS}\t{EXPGPS}\t{DIFF}\n");
            file.Close();
        }
    }





}
