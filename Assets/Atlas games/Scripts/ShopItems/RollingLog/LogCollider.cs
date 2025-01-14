using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(PolygonCollider2D))]
public class LogCollider : MonoBehaviour
{
    public float rollBackTime = 0.5f;
    public float logDamage = 5f;
    public Transform logParent;
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.GetComponent<SmartEnemyGrounded>())
        {
            SmartEnemyGrounded smartEnemyGrounded = col.GetComponent<SmartEnemyGrounded>();
            smartEnemyGrounded.HitLog(rollBackTime, logParent);
            smartEnemyGrounded.TakeDamage(logDamage, Vector2.zero, col.transform.position,
                gameObject, BODYPART.NONE, null);
        }
    }
}
