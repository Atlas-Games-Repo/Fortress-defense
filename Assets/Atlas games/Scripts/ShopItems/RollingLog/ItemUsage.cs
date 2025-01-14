using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUsage : MonoBehaviour
{
    public enum Items
    {
        Log
    }

    public GameObject logPrefab;
    public Transform itemSpawnPos;
    public float slowedDownRate = 0.3f;
    public float slowDownTime = 3f;
    public void SpawnItem(string itemName)
    {
        if (itemName == "Log")
        {
         
        }else if (itemName == "SlowDown")
        {
           
        }else if (itemName == "FortressShield")
        {
            
        }
    }

   
}
