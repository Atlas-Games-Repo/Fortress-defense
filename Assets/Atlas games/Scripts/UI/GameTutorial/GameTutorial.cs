using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using UnityEngine.UI;

public class GameTutorial : MonoBehaviour
{
    public enum TipType
    {
        Dialog,
        Tip,
        Task
    }

    public enum Direction
    {
        Top,
        Bottom,
        Left,
        Right,
        UpperLeft,
        UpperRight,
        BottomLeft,
        BottomRight
    }

    public bool inMenu = false;
    public string menuPlacing;
    [HideInInspector] public TipType type;
    [HideInInspector] public Direction direction;


    public int level;
    public List<TipSetup> tipsList;
    [HideInInspector] public Transform[] uiParts;
    public Transform mask;
    public GameObject darkBackground;
    public GameObject dialogBackground;
    public Transform pointerObject;
    public Transform pointerIcon;
    public GameObject clickPreventer;
    private int _tipOrder;
    public float speed = 30f;
    private float _maxDistance = 0.01f;
    private int _childIndex;
    private Transform _buttonParent;
    private GameObject _uiPartClone;
    public Transform _pointerEnv;
    public Transform _pointerPlacerEnv;
    [Serializable]
    public class TipSetup
    {
        public string openTrigger;
        public string closeTrigger;
        public string pointerDirection;
        public float delay;
        public bool isLastDialog;
        public Animator tipAnimator;
        public string uiPartName;
        public float scale;
        public string type;
        public bool isUiInteractible;
        public bool pauseGame;
    }

    public float initialWait = 0.5f;

    [HideInInspector] public string tutorialName;
    Camera _main;


    // start game and open tutorial automatically if never watched 
    void Start()
    {
        darkBackground.SetActive(false);
        dialogBackground.SetActive(false);
        pointerObject.transform.SetParent(transform.parent);
        pointerObject.transform.SetSiblingIndex(transform.parent.childCount - 1);
        Camera[] cams = FindObjectsOfType<Camera>();
        foreach (Camera cam in cams)
        {
            if (cam.name == "Main Camera")
            {
                _main = cam;
            }
        }

        clickPreventer.SetActive(false);
        _tipOrder = -1;
        StartCoroutine(OpenTutorialAtStart());
        pointerIcon.gameObject.SetActive(false);
        

        GameObject pointer = GameObject.FindWithTag("Pointer");
        GameObject pointer_placer = GameObject.Find("PointerPlacer");
        if (pointer) _pointerEnv = pointer.transform;
        if (pointer_placer) _pointerPlacerEnv = pointer_placer.transform;
        if (_pointerEnv)
            _pointerEnv.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
    }

// open tutorial if never watched
    IEnumerator OpenTutorialAtStart()
    {
        yield return new WaitForSeconds(initialWait);
        StartTutorial();

        //makes sure that tutorial doesnt play anymore at the start of the scene 
        PlayerPrefs.SetInt("GameTutorial" + tutorialName, 1);
    }

// start tutorial with first tip and pause game 
    public void StartTutorial()
    {
        NextTip();
    }

// go to next tip and animate enter/exit states of tips 
    public void NextTip()
    {
        _tipOrder++;
        // close last one 
        if (_tipOrder > 0)
        {
            CloseTip();
            OpenTip();
        }
        else
        {
            OpenTip();
        }
    }

    private GameObject _prevUiPart;

    void CloseTip()
    {
        if (_tipOrder > 0)
        {
            TipSetup prevSetup = tipsList[_tipOrder - 1];
            if (prevSetup.uiPartName != "")
            {
                TutorialFinder[] uiParts = FindObjectsOfType<TutorialFinder>();
                foreach (var uiPart in uiParts)
                {
                    if (uiPart.GetComponent<TutorialFinder>().uiPartName == prevSetup.uiPartName)
                    {
                        _prevUiPart = uiPart.gameObject;
                    }
                }
                //  _prevUiPart = GameObject.Find(prevSetup.uiPartName);
            }

            switch (prevSetup.type)
            {
                case "Dialog":
                    if (prevSetup.isLastDialog)
                    {
                        prevSetup.tipAnimator.SetTrigger(prevSetup.closeTrigger);
                    }
dialogBackground.SetActive(true);
                    break;
                case "Tip":
                    prevSetup.tipAnimator.SetTrigger(prevSetup.closeTrigger);
                    break;
                case "Task":
                    pointerObject.gameObject.SetActive(false);
                    pointerIcon.gameObject.SetActive(false);

                    if (!prevSetup.isUiInteractible)
                    {
                        _prevUiPart.transform.SetParent(_buttonParent);
                        _prevUiPart.transform.SetSiblingIndex(_childIndex);
                        Destroy(_uiPartClone);
                        clickPreventer.SetActive(false);
                    }

                    if (prevSetup.pauseGame)
                    {
                        Time.timeScale = 1;
                    }

                    if (_prevUiPart.GetComponent<Button>())
                    {
                        _prevUiPart.GetComponent<Button>().onClick.RemoveListener(NextTip);
                    }
                    else
                    {
                        _prevUiPart.GetComponent<TutorialFinder>().isClickable = false;
                        _pointerEnv.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
                    }

                    break;
            }
        }
    }


    private GameObject _nextUiPart;

    public void OpenTip()
    {
        if (_tipOrder <= tipsList.Count - 1)
        {
            TipSetup nextSetup = tipsList[_tipOrder];
            if (nextSetup.uiPartName != "")
            {
                TutorialFinder[] uiParts = FindObjectsOfType<TutorialFinder>();
                foreach (var uiPart in uiParts)
                {
                    if (uiPart.GetComponent<TutorialFinder>().uiPartName == nextSetup.uiPartName)
                    {
                        _nextUiPart = uiPart.gameObject;
                    }
                }
            }

            switch (nextSetup.type)
            {
                case "Dialog":
                    nextSetup.tipAnimator.SetTrigger(nextSetup.openTrigger);
                    darkBackground.SetActive(false);
                    dialogBackground.SetActive(true);
                    mask.gameObject.SetActive(false);
                    dialogBackground.SetActive(true);
                    Time.timeScale = 0;
                    break;
                case "Task":
                    if (!nextSetup.isUiInteractible && _nextUiPart.GetComponent<Button>())
                    {
                        _buttonParent = _nextUiPart.transform.parent;
                        _childIndex = _nextUiPart.transform.GetSiblingIndex();
                        clickPreventer.SetActive(true);
                        _uiPartClone = Instantiate(_nextUiPart, _nextUiPart.transform.position,
                            _nextUiPart.transform.rotation,
                            _nextUiPart.transform.parent);
                        _nextUiPart.transform.transform.SetParent(clickPreventer.transform);
                        pointerObject.gameObject.SetActive(true);
                        pointerIcon.gameObject.SetActive(true);
                    }
                    Thread.Sleep(Mathf.RoundToInt(nextSetup.delay * 1000));
                    _nextUiPart.GetComponent<TutorialFinder>().isClickable = true;
                    if (_nextUiPart.GetComponent<Button>())
                    {
                        _nextUiPart.transform.GetComponent<Button>().onClick.AddListener(NextTip);
                        pointerObject.transform.position = _nextUiPart.transform.position;
                        for (int a = 0; a < pointerObject.childCount; a++)
                        {
                            if (pointerObject.GetChild(a).name == nextSetup.pointerDirection)
                            {
                                pointerIcon.gameObject.SetActive(true);
                                pointerIcon.transform.position = pointerObject.GetChild(a).position;
                                float rotationAngel = 0f;
                                switch (nextSetup.pointerDirection)
                                {
                                    case "Top":
                                        rotationAngel = 0f;
                                        break;
                                    case "Bottom":
                                        rotationAngel = 180f;
                                        break;
                                    case "Left":
                                        rotationAngel = 90f;
                                        break;
                                    case "Right":
                                        rotationAngel = -90f;
                                        break;
                                    case "TopLeft":
                                        rotationAngel = -45f;
                                        break;
                                    case "TopRight":
                                        rotationAngel = 45f;
                                        break;
                                    case "BottomLeft":
                                        rotationAngel = -135f;
                                        break;
                                    case "BottomRight":
                                        rotationAngel = 135f;
                                        break;
                                }

                                Quaternion newRotation = Quaternion.Euler(0, 0, rotationAngel);
                                pointerIcon.transform.rotation = newRotation;
                            }
                        }


                    }
                    else
                    {
                     //   float ratio = Screen.width / Screen.height;
                     //   float size = _main.GetComponent<FixedCamera>().orthographicSize;
                     //   Vector3 camPosition = _main.transform.position;
                     //   float topPos = 1f * size + camPosition.y;
                     //   float bottomPos = -1f * size + camPosition.y;
                     //   float leftPos = -ratio * size + camPosition.x;
                     //   float rightPos = ratio * size + camPosition.x;
                     //   Vector3 finalPosition = new Vector3(
                     //       (_nextUiPart.transform.position.x - camPosition.x) / (rightPos - camPosition.x) *
                     //       Screen.width,
                     //       (_nextUiPart.transform.position.y - camPosition.y) / (topPos - camPosition.y) *
                     //       Screen.height, 0);
                     //   pointerObject.GetComponent<RectTransform>().anchoredPosition =
                     //       new Vector2(finalPosition.x, finalPosition.y);
                     //   Vector3 currentPos = pointerObject.position;
                     //   pointerObject.GetComponent<RectTransform>().anchoredPosition =
                     //       new Vector2(currentPos.x, currentPos.y);
                     _pointerEnv.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
                     _pointerPlacerEnv.transform.position = _nextUiPart.transform.position;
                     for (int a = 0; a < _pointerPlacerEnv.childCount; a++)
                     {
                         if (_pointerPlacerEnv.GetChild(a).name == nextSetup.pointerDirection)
                         {
                             _pointerEnv.position = _pointerPlacerEnv.GetChild(a).position;
                             float rotationAngel = 0f;
                             switch (nextSetup.pointerDirection)
                             {
                                 case "Top":
                                     rotationAngel = 0f;
                                     break;
                                 case "Bottom":
                                     rotationAngel = 180f;
                                     break;
                                 case "Left":
                                     rotationAngel = 90f;
                                     break;
                                 case "Right":
                                     rotationAngel = -90f;
                                     break;
                                 case "TopLeft":
                                     rotationAngel = -45f;
                                     break;
                                 case "TopRight":
                                     rotationAngel = 45f;
                                     break;
                                 case "BottomLeft":
                                     rotationAngel = -135f;
                                     break;
                                 case "BottomRight":
                                     rotationAngel = 135f;
                                     break;
                             }

                             Quaternion newRotation = Quaternion.Euler(0, 0, rotationAngel);
                             _pointerEnv.rotation = newRotation;
                         }
                     }
                    }
                    
                    darkBackground.SetActive(false);
                    mask.gameObject.SetActive(false);
                    dialogBackground.SetActive(false);
                    if (nextSetup.pauseGame)
                    {
                        Time.timeScale = 0;
                    }
                    else
                    {
                        Time.timeScale = 1;
                    }

                    break;
                case "Tip":
                    nextSetup.tipAnimator.SetTrigger(nextSetup.openTrigger);
                    Time.timeScale = 0;
                    darkBackground.SetActive(true);
                    mask.gameObject.SetActive(true);
                    dialogBackground.SetActive(false);
                    StartCoroutine(SmoothTransition(_nextUiPart.transform.position, nextSetup.scale));
                    break;
            }
        }
        else
        {
            darkBackground.SetActive(false);
            dialogBackground.SetActive(false);
            Time.timeScale = 1;
          //  _tipOrder = -1;
        }
    }


    IEnumerator SmoothTransition(Vector3 targetPosition, float targetScale)
    {
        float appliedScale = mask.localScale.x;
        Vector3 maskPosition = mask.position;
        while (Vector3.Distance(mask.position, targetPosition) > _maxDistance)
        {
            maskPosition = Vector3.Lerp(mask.position, targetPosition, Time.unscaledDeltaTime * speed);
            appliedScale = Mathf.Lerp(mask.localScale.x, targetScale, Time.unscaledDeltaTime * speed);
            mask.localScale = new Vector3(appliedScale, appliedScale, appliedScale);
            mask.position = maskPosition;
            yield return null;
        }
    }

    public void SkipTutorial()
    {
        _tipOrder++;
        TipSetup prevSetup = tipsList[_tipOrder - 1];
        if (prevSetup.uiPartName != "")
        {
            TutorialFinder[] uiParts = FindObjectsOfType<TutorialFinder>();
            foreach (var uiPart in uiParts)
            {
                if (uiPart.name == prevSetup.uiPartName)
                {
                    _prevUiPart = uiPart.gameObject;
                }
            }
            //  _prevUiPart = GameObject.Find(prevSetup.uiPartName);
        }

        switch (prevSetup.type)
        {
            case "Dialog":
                if (prevSetup.isLastDialog)
                {
                    prevSetup.tipAnimator.SetTrigger(prevSetup.closeTrigger);
                }
                else
                {
                    prevSetup.tipAnimator.gameObject.SetActive(false);
                }

                break;
            case "Tip":
                prevSetup.tipAnimator.SetTrigger(prevSetup.closeTrigger);
                break;
            case "Task":
                pointerObject.gameObject.SetActive(false);
                pointerIcon.gameObject.SetActive(false);

                if (!prevSetup.isUiInteractible)
                {
                    _prevUiPart.transform.SetParent(_buttonParent);
                    _prevUiPart.transform.SetSiblingIndex(_childIndex);
                    Destroy(_uiPartClone);
                    clickPreventer.SetActive(false);
                }

                if (prevSetup.pauseGame)
                {
                    Time.timeScale = 1;
                }

                if (_prevUiPart.GetComponent<Button>())
                {
                    _prevUiPart.GetComponent<Button>().onClick.RemoveListener(NextTip);
                }
                else
                {
                    _pointerEnv.GetComponent<SpriteRenderer>().enabled = false;
                }

                break;
        }

        darkBackground.SetActive(false);
        dialogBackground.SetActive(false);
       // _tipOrder = -1;
    }
    public void DestroyTutorial()
    {
        Destroy(gameObject);
    }
}