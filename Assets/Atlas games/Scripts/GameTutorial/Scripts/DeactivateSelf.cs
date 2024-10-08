using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateSelf : MonoBehaviour
{
    // Start is called before the first frame update
    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void DeactivateParent()
    {
        transform.parent.GetComponent<GameTutorial>().DestroyTutorial();
        
    }
}
