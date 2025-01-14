using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animSwitch : MonoBehaviour
{

    public Animator anim;
    private float count = 0f;
    private float num = 1f;

    void Update()
    {
        if(Input.GetKeyDown("0"))
        {
           count = 0f;
        }

        if(Input.GetKeyDown("p"))
        {
           anim.SetBool("Death", true);

           anim.SetBool("Walk", false);
           anim.SetBool("Throw", false);
           anim.SetBool("Damage", false);
        }
        
        else if(Input.GetKeyDown("1"))
        {
           count = 1f;
        }
        else if(Input.GetKeyDown("2"))
        {
           count = 2f;
        }
        else if(Input.GetKeyDown("3"))
        {
           count = 3f;
        }
        else if(Input.GetKeyDown("4"))
        {
           count = 4f;
        }


        // switchAnimation();
    }

    void switchAnimation()
    {
        switch (count)
        {
            case 0f:
            anim.SetBool("Idle", true);

            anim.SetBool("Walk", false);
            anim.SetBool("Throw", false);
            anim.SetBool("Damage", false);
            anim.SetBool("Death", false);

            Debug.Log("num is 0");
            break;

            case 1f:
            anim.SetBool("Walk", true);

            anim.SetBool("Idle", false);
            anim.SetBool("Throw", false);
            anim.SetBool("Damage", false);
            anim.SetBool("Death", false);

            Debug.Log("num is 1");
            break;

            case 2f:
            anim.SetBool("Throw", true);
            
            anim.SetBool("Walk", false);
            anim.SetBool("Idle", false);
            anim.SetBool("Damage", false);
            anim.SetBool("Death", false);

            Debug.Log("num is 2");
            break;

            case 3f:
            anim.SetBool("Damage", true);

            anim.SetBool("Walk", false);
            anim.SetBool("Throw", false);
            anim.SetBool("Idle", false);
            anim.SetBool("Death", false);
            
            Debug.Log("num is 3");
            break;

            case 4f:
            anim.SetBool("Death", true);

            anim.SetBool("Walk", false);
            anim.SetBool("Throw", false);
            anim.SetBool("Damage", false);
            anim.SetBool("Idle", false);

            Debug.Log("num is 4");
            break;
        }
    }
}