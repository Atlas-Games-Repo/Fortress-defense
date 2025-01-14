using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArcherTargetSelector : MonoBehaviour, IGetTouchEvent
{
    public void TouchEvent()
    {
        if (GameManager.Instance.State != GameManager.GameState.Playing)
            return;
        bool is_enable = gameObject.GetComponentInParent<Player_Archer>().is_manual_enable;
        if (is_enable)
        {
            gameObject.GetComponentInParent<Player_Archer>().is_manual_enable = false;
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        }
        else
        {
            gameObject.GetComponentInParent<Player_Archer>().is_manual_enable = true;
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.3f);
        }
    }
}
