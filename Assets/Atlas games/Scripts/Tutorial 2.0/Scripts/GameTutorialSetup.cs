using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTutorialSetup : MonoBehaviour
{
    public static GameTutorialSetup self;
    public GameObject[] tutorials;
   public void Awake() {
       if (self == null) {
           self = this;
           DontDestroyOnLoad(gameObject);
       } else {
           Destroy(gameObject);
       }
   }

    public GameObject SceneTutorial() {
        foreach (GameObject obj in tutorials) {
            if (obj.GetComponent<TutorialNew>().tutorialLevel == GlobalValue.levelPlaying && obj.GetComponent<TutorialNew>().placing == TutorialPlacing.Game) {
                return obj;
            }
        }

        return null;
    }
    
}
