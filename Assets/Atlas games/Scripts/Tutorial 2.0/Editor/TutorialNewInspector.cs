using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Video;

[CustomEditor(typeof(TutorialNew))]
public class TutorialNewInspector : Editor
{
    private Tip newTip = new Tip();
    private bool showAddTipSection = false;
    private string errorMessage = "";
    private SerializedObject serializedTutorial;
    private SerializedProperty tutorialStepProperty;

    private void OnEnable()
    {
        serializedTutorial = new SerializedObject(target);
        tutorialStepProperty = serializedTutorial.FindProperty("tutorialStep");
        newTip = new Tip(); // Initialize the new tip
    }

    public override void OnInspectorGUI()
    {
        TutorialNew tutorial = (TutorialNew)target;
        EditorGUILayout.LabelField("Tutorial Steps", EditorStyles.boldLabel);
        DisplayExistingSteps(tutorial);


        EditorGUILayout.Space();
        GUILayout.Space(10);
        
        
        GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout)
        {
            fontStyle = FontStyle.Bold,
            fontSize = 12,
            normal = new GUIStyleState
            {
                textColor = Color.white
            },
            richText = true
        };

        GUIStyle boxStyle = new GUIStyle()
        {
            normal = new GUIStyleState()
            {
                background = CreateTexture(1, 1, new Color(0.3f, 0.3f, 0.3f)) 
            },
            padding = new RectOffset(20, 10, 10, 10) 
        };

        EditorGUILayout.BeginVertical(boxStyle);
        showAddTipSection = EditorGUILayout.Foldout(showAddTipSection, "Add New Tip", false);
        if (showAddTipSection)
        {
            DisplayAddTipSection(tutorial);
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Tutorial Placing", EditorStyles.boldLabel);
        tutorial.placing = (TutorialPlacing)EditorGUILayout.EnumPopup("Placing", tutorial.placing);

        if (tutorial.placing == TutorialPlacing.Menu)
        {
            tutorial.tutorialName = EditorGUILayout.TextField("Tutorial Name", tutorial.tutorialName);
        }
        else if (tutorial.placing == TutorialPlacing.Game)
        {
            tutorial.tutorialLevel = EditorGUILayout.IntField("Tutorial Level", tutorial.tutorialLevel);
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Prefabs", EditorStyles.boldLabel);
        tutorial.hint = (Hint)EditorGUILayout.ObjectField("Hint", tutorial.hint, typeof(Hint), true);
        tutorial.dialog = (Dialog)EditorGUILayout.ObjectField("Dialog", tutorial.dialog, typeof(Dialog), true);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("UI Elements", EditorStyles.boldLabel);
        tutorial.circleMask = (RectTransform)EditorGUILayout.ObjectField("Circle Mask", tutorial.circleMask, typeof(RectTransform), true);
        tutorial.pointerObject = (Transform)EditorGUILayout.ObjectField("Pointer Object", tutorial.pointerObject, typeof(Transform), true);
        tutorial.pointerIcon = (Transform)EditorGUILayout.ObjectField("Pointer Icon", tutorial.pointerIcon, typeof(Transform), true);
        tutorial.pointerPlacerEnvironment = (Transform)EditorGUILayout.ObjectField("Environment Pointer", tutorial.pointerPlacerEnvironment, typeof(Transform), true);
        tutorial.environmentPointer = (Transform)EditorGUILayout.ObjectField("Environment Pointer Icon", tutorial.environmentPointer, typeof(Transform), true);
        tutorial.clickPreventer = (GameObject)EditorGUILayout.ObjectField("Click Preventer", tutorial.clickPreventer, typeof(GameObject), true);
        tutorial.transparent = EditorGUILayout.ColorField("Transparent", tutorial.transparent);
        tutorial.darkBackground = EditorGUILayout.ColorField("Dark Background", tutorial.darkBackground);
        tutorial.speed = EditorGUILayout.FloatField("Speed", tutorial.speed);
        tutorial.initialDelay = EditorGUILayout.FloatField("Initial Delay", tutorial.initialDelay);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Positions", EditorStyles.boldLabel);
        tutorial.TL_Pos = (Transform)EditorGUILayout.ObjectField("Top Left Position", tutorial.TL_Pos, typeof(Transform), true);
        tutorial.TM_Pos = (Transform)EditorGUILayout.ObjectField("Top Middle Position", tutorial.TM_Pos, typeof(Transform), true);
        tutorial.TR_Pos = (Transform)EditorGUILayout.ObjectField("Top Right Position", tutorial.TR_Pos, typeof(Transform), true);
        tutorial.L_Pos = (Transform)EditorGUILayout.ObjectField("Left Position", tutorial.L_Pos, typeof(Transform), true);
        tutorial.R_Pos = (Transform)EditorGUILayout.ObjectField("Right Position", tutorial.R_Pos, typeof(Transform), true);
        tutorial.BL_Pos = (Transform)EditorGUILayout.ObjectField("Bottom Left Position", tutorial.BL_Pos, typeof(Transform), true);
        tutorial.BR_Pos = (Transform)EditorGUILayout.ObjectField("Bottom Right Position", tutorial.BR_Pos, typeof(Transform), true);
        tutorial.BM_Pos = (Transform)EditorGUILayout.ObjectField("Bottom Middle Position", tutorial.BM_Pos, typeof(Transform), true);
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Positions in game", EditorStyles.boldLabel);
        tutorial.TL_Pos_env = (Transform)EditorGUILayout.ObjectField("Top Left Position", tutorial.TL_Pos_env, typeof(Transform), true);
        tutorial.TM_Pos_env = (Transform)EditorGUILayout.ObjectField("Top Middle Position", tutorial.TM_Pos_env, typeof(Transform), true);
        tutorial.TR_Pos_env = (Transform)EditorGUILayout.ObjectField("Top Right Position", tutorial.TR_Pos_env, typeof(Transform), true);
        tutorial.L_Pos_env = (Transform)EditorGUILayout.ObjectField("Left Position", tutorial.L_Pos_env, typeof(Transform), true);
        tutorial.R_Pos_env = (Transform)EditorGUILayout.ObjectField("Right Position", tutorial.R_Pos_env, typeof(Transform), true);
        tutorial.BL_Pos_env = (Transform)EditorGUILayout.ObjectField("Bottom Left Position", tutorial.BL_Pos_env, typeof(Transform), true);
        tutorial.BR_Pos_env = (Transform)EditorGUILayout.ObjectField("Bottom Right Position", tutorial.BR_Pos_env, typeof(Transform), true);
        tutorial.BM_Pos_env = (Transform)EditorGUILayout.ObjectField("Bottom Middle Position", tutorial.BM_Pos_env, typeof(Transform), true);
        tutorial.pointerPlacerEnvironment = (Transform)EditorGUILayout.ObjectField("Pointer Environment", tutorial.pointerPlacerEnvironment, typeof(Transform), true);
        tutorial.environmentPointer =
            (Transform)EditorGUILayout.ObjectField("Pointer Environment Icon", tutorial.environmentPointer,typeof(Transform),true);
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Use when testing the created tutorial", EditorStyles.boldLabel);
        tutorial.devMode = EditorGUILayout.Toggle("Dev mode", tutorial.devMode);

EditorGUILayout.Space();
        if (GUI.changed)
        {
            EditorUtility.SetDirty(tutorial);
        }       
    }

  
    private void DisplayExistingSteps(TutorialNew tutorial)
    {
        GUIStyle addButtonStyle = new GUIStyle(GUI.skin.button)
        {
            normal = new GUIStyleState() { background = MakeTex(2, 2, new Color(0.6f, 0f, 0f)), textColor = Color.white },
            hover = new GUIStyleState() { background = MakeTex(2, 2, new Color(0.9f, 0f, 0f)) , textColor = Color.white}, 
            active = new GUIStyleState() { background = MakeTex(2, 2, new Color(0.3f, 0f, 0f)) , textColor = Color.white},
            alignment = TextAnchor.MiddleCenter,
            fontSize = 12
        };
        GUIStyle deleteButtonStyle = new GUIStyle(GUI.skin.button)
        {
            normal = new GUIStyleState() { background = MakeTex(2, 2, new Color(0.2f, 0.2f, 0.2f)), textColor = Color.white },
            hover = new GUIStyleState() { background = MakeTex(2, 2, new Color(0.1f, 0.1f, 0.1f)) , textColor = Color.white}, 
            active = new GUIStyleState() { background = MakeTex(2, 2, new Color(0.2f, 0.05f, 0.2f)) , textColor = Color.white},
            alignment = TextAnchor.MiddleCenter,
            fontSize = 12
        };
        if (tutorial.tutorialSteps == null)
        {
            tutorial.tutorialSteps = new List<Tip>();
        }
        if (tutorial.tutorialSteps != null && tutorial.tutorialSteps.Count > 0)
        {
            bool isDeleted = false;
            bool isMoving = false;
            int index = 0;
            int newIndex = 0;
            for (int i = 0; i < tutorial.tutorialSteps.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                string tooltip = $"Type: {tutorial.tutorialSteps[i].tipType}\n" +
                                 $"Text: {tutorial.tutorialSteps[i].tipText}\n" +
                                 $"Delay: {tutorial.tutorialSteps[i].delay}\n" +
                                 $"UIPartName: {tutorial.tutorialSteps[i].uiPartName}\n" +
                                 $"Is Intractible: {tutorial.tutorialSteps[i].isUiInteractible}";
                int tipTexLen = tutorial.tutorialSteps[i].tipText.Length;
                string tipText = "";
                if (tipTexLen > 50) {
                    tipText = tutorial.tutorialSteps[i].tipText.Substring(0, 50);
                } else {
                    tipText = tutorial.tutorialSteps[i].tipText;
                }
                GUIContent content = new GUIContent($"Step {i + 1}: {tutorial.tutorialSteps[i].tipType} - {tipText}", tooltip);
                EditorGUILayout.LabelField(content, GUILayout.Width(200));
                if (GUILayout.Button("-", addButtonStyle, GUILayout.Width(20))) {
                    index = i;
                    isDeleted = true;
                }
                if (GUILayout.Button("↑", deleteButtonStyle, GUILayout.Width(20))) {
                    if (i > 0) {
                        index = i;
                        isMoving = true;
                        newIndex = i - 1;
                    }
                }
                if (GUILayout.Button("↓", deleteButtonStyle, GUILayout.Width(20))) {
                    if (i < tutorial.tutorialSteps.Count - 1) {
                        index = i;
                        isMoving = true;
                        newIndex = i + 1;
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            if (isDeleted) {
                tutorial.tutorialSteps.RemoveAt(index);
                EditorUtility.SetDirty(tutorial);
            }
            if (isMoving) {
                var temp = tutorial.tutorialSteps[index];
                tutorial.tutorialSteps[index] = tutorial.tutorialSteps[newIndex];
                tutorial.tutorialSteps[newIndex] = temp;
                EditorUtility.SetDirty(tutorial);
            }
        }
        else
        {
            EditorGUILayout.LabelField("No tutorial steps available.");
        }
    }

    private void DisplayAddTipSection(TutorialNew tutorial)
    {
        EditorGUILayout.LabelField("New Tip Details", EditorStyles.boldLabel);

        newTip.tipType = (TipType)EditorGUILayout.EnumPopup("Tip Type", newTip.tipType);

        switch (newTip.tipType)
        {
            case TipType.Dialog:
                DisplayDialogFields();
                break;
            case TipType.Hint:
                DisplayHintFields();
                break;
            case TipType.Task:
                DisplayTaskFields();
                break;
        }

        EditorGUILayout.Space();
        if (!string.IsNullOrEmpty(errorMessage))
        {
            EditorGUILayout.HelpBox(errorMessage, MessageType.Error);
        }
        GUIStyle addButtonStyle = new GUIStyle(GUI.skin.button)
        {
            normal = new GUIStyleState() { background = MakeTex(2, 2, new Color(0.6f, 0f, 0f)), textColor = Color.white },
            hover = new GUIStyleState() { background = MakeTex(2, 2, new Color(0.9f, 0f, 0f)) , textColor = Color.white}, 
            active = new GUIStyleState() { background = MakeTex(2, 2, new Color(0.3f, 0f, 0f)) , textColor = Color.white},
            alignment = TextAnchor.MiddleCenter,
            fontSize = 12
        };
        GUIStyle deleteButtonStyle = new GUIStyle(GUI.skin.button)
        {
            normal = new GUIStyleState() { background = MakeTex(2, 2, new Color(0f, 0.6f, 0f)), textColor = Color.white },
            hover = new GUIStyleState() { background = MakeTex(2, 2, new Color(0f, 0.9f, 0f)) , textColor = Color.white}, 
            active = new GUIStyleState() { background = MakeTex(2, 2, new Color(0f, 0.3f, 0f)) , textColor = Color.white},
            alignment = TextAnchor.MiddleCenter,
            fontSize = 12
        };
        
        EditorGUILayout.Space();
        if (GUILayout.Button("Add Tip",deleteButtonStyle, GUILayout.Height(30)))
        {
            if (ValidateNewTip(newTip))
            {
                AddTipToList(tutorial.tutorialSteps, newTip);
                newTip = new Tip(); 
                errorMessage = "";  
                EditorUtility.SetDirty(tutorial); 
            }
            else
            {
                errorMessage = "Please fill all required fields before adding the tip.";
            }
        }
EditorGUILayout.Space();
        if (GUILayout.Button("Reset All Tips",addButtonStyle, GUILayout.Height(30)))
        {
            tutorial.tutorialSteps = new List<Tip>(); 
            EditorUtility.SetDirty(tutorial);
        }
    }
    private void AddTipToList(List<Tip> tutorialStepList, Tip tipToAdd)
    {
        if (tutorialStepList == null)
        {
            tutorialStepList = new List<Tip>();
        }
    
        tutorialStepList.Add(CloneTip(tipToAdd));
    }
    private void DisplayDialogFields()
    {
        newTip.dialogContentType = (DialogContent)EditorGUILayout.EnumPopup("Dialog Content Type", newTip.dialogContentType);
        newTip.tipText = EditorGUILayout.TextField("Tip Text", newTip.tipText);
        newTip.delay = EditorGUILayout.FloatField("Delay", newTip.delay);

        if (newTip.dialogContentType == DialogContent.Video)
        {
            newTip.dialogVideo = (VideoClip)EditorGUILayout.ObjectField("Dialog Video", newTip.dialogVideo, typeof(VideoClip), false);
        }
        else if (newTip.dialogContentType == DialogContent.Image)
        {
            newTip.dialogImage = (Sprite)EditorGUILayout.ObjectField("Dialog Image", newTip.dialogImage, typeof(Sprite), false);
        }
    }

    private void DisplayHintFields()
    {
        newTip.tipText = EditorGUILayout.TextField("Tip Text", newTip.tipText);
        newTip.tipDirection = (Direction)EditorGUILayout.EnumPopup("Tip Direction", newTip.tipDirection);
        newTip.uiPartName = EditorGUILayout.TextField("UI Part Name", newTip.uiPartName);
        newTip.circleMaskScale = EditorGUILayout.FloatField("Circle Mask Scale", newTip.circleMaskScale);
        newTip.pauseGame = EditorGUILayout.Toggle("Pause Game", newTip.pauseGame);
        newTip.delay = EditorGUILayout.FloatField("Delay", newTip.delay);
    }

    private void DisplayTaskFields()
    {
        newTip.uiPartName = EditorGUILayout.TextField("UI Part Name", newTip.uiPartName);
        newTip.isUiInteractible = EditorGUILayout.Toggle("Is UI Interactible", newTip.isUiInteractible);
        newTip.delay = EditorGUILayout.FloatField("Delay", newTip.delay);
        newTip.pointerDirection = (Direction)EditorGUILayout.EnumPopup("Pointer Direction", newTip.pointerDirection);
    }

    private bool ValidateNewTip(Tip tip)
    {
        switch (tip.tipType)
        {
            case TipType.Dialog:
                return  (!string.IsNullOrEmpty(tip.tipText) && tip.dialogContentType == DialogContent.Text) ||
                       ((tip.dialogContentType == DialogContent.Video && tip.dialogVideo != null) || (tip.dialogContentType == DialogContent.Image && tip.dialogImage != null));
            case TipType.Hint:
                return !string.IsNullOrEmpty(tip.tipText) && !string.IsNullOrEmpty(tip.uiPartName);
            case TipType.Task:
                return !string.IsNullOrEmpty(tip.uiPartName);
            default:
                return false;
        }
    }

    private Tip CloneTip(Tip originalTip)
    {
        return new Tip
        {
            tipType = originalTip.tipType,
            tipTitle = originalTip.tipTitle,
            tipText = originalTip.tipText ?? string.Empty,
            uiPartName = originalTip.uiPartName,
            dialogContentType = originalTip.dialogContentType,
            dialogImage = originalTip.dialogImage,
            dialogVideo = originalTip.dialogVideo,
            pointerDirection = originalTip.pointerDirection,
            circleMaskScale = originalTip.circleMaskScale,
            tipDirection = originalTip.tipDirection,
            delay = originalTip.delay,
        };
    }
    
    private Texture2D CreateTexture(int width, int height, Color color)
    {
        Texture2D texture = new Texture2D(width, height);
        Color[] pixels = new Color[width * height];

        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = color;
        }

        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }
    private Texture2D MakeTex(int width, int height, Color color)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; i++) pix[i] = color;
        Texture2D texture = new Texture2D(width, height);
        texture.SetPixels(pix);
        texture.Apply();
        return texture;
    }
}
