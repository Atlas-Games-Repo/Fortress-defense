using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTutorialManager : MonoBehaviour
{
    public GameTutorialSetup setup;
    void Start() {
            setup = FindObjectOfType<GameTutorialSetup>();
            if (GlobalValue.GetTutorialState(GlobalValue.levelPlaying.ToString()) == 0) {
                GameObject setupObj = setup.SceneTutorial();
                if (!setupObj) return;
                if (setup.SceneTutorial() != null)
                {
                    MenuManager manager = FindObjectOfType<MenuManager>();
                    if (manager == null) {
                        // do something logical
                        return;
                    }
                    GameObject obj = Instantiate(setup.SceneTutorial(), manager.transform.position, Quaternion.identity, manager.transform);
                    obj.SetActive(true);
                    obj.transform.SetSiblingIndex(manager.transform.childCount - 1);
                    obj.GetComponent<TutorialNew>().InitTutorial();
                }
           
                GlobalValue.SetTutorialState(GlobalValue.levelPlaying.ToString(),1);
            }
    }

 
    GameObject _tutorialObj;
    public void StartTutorialInMenu(string placing)
    {
       if (GlobalValue.GetTutorialState(placing) == 0)
       {
            for (int a = 0; a < setup.tutorials.Length; a++)
            {
                if (setup.tutorials[a].GetComponent<TutorialNew>().placing == TutorialPlacing.Menu && setup.tutorials[a].GetComponent<TutorialNew>().tutorialName == placing)
                {
                    _tutorialObj = setup.tutorials[a].gameObject;
                }
            }
            GameObject spawnedObj = Instantiate(_tutorialObj,
            FindObjectOfType<MainMenuHomeScene>().transform.position, Quaternion.identity,
            FindObjectOfType<MainMenuHomeScene>().transform);
            spawnedObj.SetActive(true);
            spawnedObj.transform.SetSiblingIndex(FindObjectOfType<MainMenuHomeScene>().transform.childCount - 1);
            spawnedObj.GetComponent<TutorialNew>().InitTutorial();
            GlobalValue.SetTutorialState(placing,1);
            
        }
    }
}
