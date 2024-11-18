using UnityEditor;
using UnityEngine;
using UnityEngine.Video;

[CustomEditor(typeof(TutorialNew))]
public class TutorialNewInspector : Editor
{
    private Tip newTip = new Tip();
    private bool showAddTipSection = false;
    private string errorMessage = "";

    private void OnEnable()
    {
        newTip = new Tip(); // Initialize the new tip
    }

    public override void OnInspectorGUI()
    {
        TutorialNew tutorial = (TutorialNew)target;

        // Display existing steps
        EditorGUILayout.LabelField("Tutorial Steps", EditorStyles.boldLabel);
        DisplayExistingSteps(tutorial);


        EditorGUILayout.Space();
        // Section to add a new tip
        GUILayout.Space(10);
        showAddTipSection = EditorGUILayout.Foldout(showAddTipSection, "Add New Tip");
        if (showAddTipSection)
        {
            DisplayAddTipSection(tutorial);
        }

        EditorGUILayout.LabelField("Tutorial Placing", EditorStyles.boldLabel);
        tutorial.placing = (TutorialPlacing)EditorGUILayout.EnumPopup("Placing", tutorial.placing);

        // Conditional display of tutorialName or tutorialLevel
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
        tutorial.clickPreventer = (GameObject)EditorGUILayout.ObjectField("Click Preventer", tutorial.clickPreventer, typeof(GameObject), true);
        tutorial.transparent = EditorGUILayout.ColorField("Transparent", tutorial.transparent);
        tutorial.darkBackground = EditorGUILayout.ColorField("Dark Background", tutorial.darkBackground);
        tutorial.speed = EditorGUILayout.FloatField("Speed", tutorial.speed);
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
        // Save changes to the TutorialNew object
        if (GUI.changed)
        {
            EditorUtility.SetDirty(tutorial);
        }
    }

    private void DisplayExistingSteps(TutorialNew tutorial)
    {
        if (tutorial.tutorialStep != null && tutorial.tutorialStep.Length > 0)
        {
            for (int i = 0; i < tutorial.tutorialStep.Length; i++)
            {
                string stepText = !string.IsNullOrEmpty(tutorial.tutorialStep[i]?.tipText)
                    ? tutorial.tutorialStep[i].tipText
                    : $"Step {i + 1} ({tutorial.tutorialStep[i].tipType})";
                EditorGUILayout.LabelField($"Step {i + 1}", stepText);
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

        // Display fields based on TipType
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

        // Display error message if any
        if (!string.IsNullOrEmpty(errorMessage))
        {
            EditorGUILayout.HelpBox(errorMessage, MessageType.Error);
        }

        // Add Tip Button
        if (GUILayout.Button("Add Tip", GUILayout.Height(30)))
        {
            if (ValidateNewTip(newTip))
            {
                ArrayUtility.Add(ref tutorial.tutorialStep, CloneTip(newTip));
                newTip = new Tip(); // Reset new tip
                errorMessage = "";  // Clear error message
                EditorUtility.SetDirty(tutorial); // Mark as changed
            }
            else
            {
                errorMessage = "Please fill all required fields for the selected Tip Type.";
            }
        }

        // Reset Button
        if (GUILayout.Button("Reset All Tips", GUILayout.Height(30)))
        {
            tutorial.tutorialStep = new Tip[0]; // Clear all steps
            EditorUtility.SetDirty(tutorial); // Mark as changed
        }
    }

    private void DisplayDialogFields()
    {
        newTip.dialogContentType = (DialogContent)EditorGUILayout.EnumPopup("Dialog Content Type", newTip.dialogContentType);
        newTip.tipTitle = EditorGUILayout.TextField("Tip Title", newTip.tipTitle);
        newTip.tipText = EditorGUILayout.TextField("Tip Text", newTip.tipText);
        newTip.isLastDialog = EditorGUILayout.Toggle("Is Last Dialog", newTip.isLastDialog);
        newTip.isUiInteractible = EditorGUILayout.Toggle("Is UI Interactible", newTip.isUiInteractible);
        newTip.pauseGame = EditorGUILayout.Toggle("Pause Game", newTip.pauseGame);

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
                return !string.IsNullOrEmpty(tip.tipTitle) && !string.IsNullOrEmpty(tip.tipText) &&
                       ((tip.dialogContentType == DialogContent.Video && tip.dialogVideo != null) ||
                        (tip.dialogContentType == DialogContent.Image && tip.dialogImage != null));
            case TipType.Hint:
                return !string.IsNullOrEmpty(tip.tipText) && !string.IsNullOrEmpty(tip.uiPartName);
            case TipType.Task:
                return !string.IsNullOrEmpty(tip.uiPartName);
            default:
                return false;
        }
    }

    private Tip CloneTip(Tip original)
    {
        return new Tip
        {
            tipType = original.tipType,
            tipText = original.tipText,
            tipTitle = original.tipTitle,
            dialogContentType = original.dialogContentType,
            dialogImage = original.dialogImage,
            dialogVideo = original.dialogVideo,
            isLastDialog = original.isLastDialog,
            isUiInteractible = original.isUiInteractible,
            pauseGame = original.pauseGame,
            tipDirection = original.tipDirection,
            pointerDirection = original.pointerDirection,
            uiPartName = original.uiPartName,
            circleMaskScale = original.circleMaskScale,
            delay = original.delay
        };
    }
}
