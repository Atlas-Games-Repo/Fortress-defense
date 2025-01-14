// using UnityEditor;
// using UnityEngine;
// using UnityEngine.Video;
//
// [CustomEditor(typeof(Tip))]
// public class TipInspector : Editor
// {
//     public override void OnInspectorGUI()
//     {
//         Tip tip = (Tip)target;
//
//         EditorGUI.BeginChangeCheck();
//
//         tip.tipType = (TipType)EditorGUILayout.EnumPopup("Tip Type", tip.tipType);
//
//         // Handle fields based on TipType
//         switch (tip.tipType)
//         {
//             case TipType.Dialog:
//                 tip.dialogContentType = (DialogContent)EditorGUILayout.EnumPopup("Dialog Content Type", tip.dialogContentType);
//                 tip.delay = EditorGUILayout.FloatField("Delay", tip.delay);
//                 tip.isLastDialog = EditorGUILayout.Toggle("Is Last Dialog", tip.isLastDialog);
//                // tip.dialog = (Dialog)EditorGUILayout.ObjectField("Dialog", tip.dialog, typeof(Dialog), false);
//            //     tip.isUiInteractible = EditorGUILayout.Toggle("Is UI Interactible", tip.isUiInteractible);
//           //      tip.pauseGame = EditorGUILayout.Toggle("Pause Game", tip.pauseGame);
//           //      tip.tipTitle = EditorGUILayout.TextField("Tip Title", tip.tipTitle);
//                 tip.tipText = EditorGUILayout.TextField("Tip Text", tip.tipText);
//
//                 if (tip.dialogContentType == DialogContent.Video)
//                 {
//                     tip.dialogVideo = (VideoClip)EditorGUILayout.ObjectField("Dialog Video", tip.dialogVideo, typeof(VideoClip), false);
//                 }
//                 else if (tip.dialogContentType == DialogContent.Image)
//                 {
//                     tip.dialogImage = (Sprite)EditorGUILayout.ObjectField("Dialog Image", tip.dialogImage, typeof(Sprite), false);
//                 }
//                 break;
//
//             case TipType.Hint:
//                 tip.tipText = EditorGUILayout.TextField("Tip Text", tip.tipText);
//                 tip.tipDirection = (Direction)EditorGUILayout.EnumPopup("Tip Direction", tip.tipDirection);
//                 tip.uiPartName = EditorGUILayout.TextField("UI Part Name", tip.uiPartName);
//                 tip.circleMaskScale = EditorGUILayout.FloatField("Circle Mask Scale", tip.circleMaskScale);
//                 tip.pauseGame = EditorGUILayout.Toggle("Pause Game", tip.pauseGame);
//                 tip.hintDirection = (Direction)EditorGUILayout.EnumPopup("Hint Direction", tip.hintDirection);
//                 break;
//
//                 tip.uiPartName = EditorGUILayout.TextField("UI Part Name", tip.uiPartName);
//                 tip.isUiInteractible = EditorGUILayout.Toggle("Is UI Interactible", tip.isUiInteractible);
//                 tip.delay = EditorGUILayout.FloatField("Delay", tip.delay);
//                 tip.pointerDirection = (Direction)EditorGUILayout.EnumPopup("Pointer Direction", tip.pointerDirection);
//                 break;
//
//             default:
//                 EditorGUILayout.HelpBox("Unsupported Tip Type", MessageType.Warning);
//                 break;
//         }
//
//         if (EditorGUI.EndChangeCheck())
//         {
//             Undo.RecordObject(tip, "Modified Tip");
//             EditorUtility.SetDirty(tip);
//         }
//     }
// }
