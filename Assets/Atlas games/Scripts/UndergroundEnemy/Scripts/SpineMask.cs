using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpineMask : MonoBehaviour
{
    private void Start()
    {
        GetComponent<MeshRenderer>().material.renderQueue = 3002;
    }
}
