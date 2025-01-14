using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camMove : MonoBehaviour
{
    public GameObject camera;

    void Update()
    {
        if(Input.GetKeyDown("d"))
        {
            moveCamera();
        }
        else if(Input.GetKeyDown("a"))
        {
            moveCameraBack();
        }
    }

    public void moveCamera()
    {
        camera.transform.position = new Vector3 (camera.transform.position.x + 20f ,camera.transform.position.y ,-10);
    }
    public void moveCameraBack()
    {
        camera.transform.position = new Vector3 (camera.transform.position.x - 20f ,camera.transform.position.y ,-10);
    }
}
