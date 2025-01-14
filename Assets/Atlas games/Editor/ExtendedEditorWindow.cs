using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
public class ExtendedEditorWindow : EditorWindow
{
    protected SerializedObject serializedObject;
    protected SerializedProperty serializedProperty;
    string LevelEnemyManagerPath = "Assets/_FD/Prefab/System/LevelEnemyManager.prefab";
    string GameLevelSetupPath = "Assets/BundledAssets/Loading/Game Level Setup.prefab";
    string AssetsPath = "Assets/GeneratedLevels/";
    public GameLevelSetup gameLevelSetup;

    protected string gameLevelSetupPath
    {
        get { return GameLevelSetupPath; }
    }

    protected void DrawProperties(SerializedProperty prop, bool drawChildren)
    {
        string lastPropPath = string.Empty;
        foreach (SerializedProperty p in prop)
        {
            if (p.isArray && p.propertyType == SerializedPropertyType.Generic)
            {
                EditorGUILayout.BeginHorizontal();
                p.isExpanded = EditorGUILayout.Foldout(p.isExpanded, p.displayName);
                EditorGUILayout.EndHorizontal();

                if (p.isExpanded)
                {
                    EditorGUI.indentLevel++;
                    DrawProperties(p, drawChildren);
                    EditorGUI.indentLevel--;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(lastPropPath) && p.propertyPath.Contains(lastPropPath)) { continue; }
                lastPropPath = p.propertyPath;
                EditorGUILayout.PropertyField(p, drawChildren);
            }
        }
    }
    protected void DrawProperties(LevelWave level, SerializedProperty prop, bool drawChildren)
    {
        LoadLevel(level);
        DrawProperties(prop, drawChildren);
    }
    protected void Apply()
    {
        serializedObject.ApplyModifiedProperties();
    }

    protected void createInstance(string name)
    {
        LevelData obj = serializedObject.targetObject as LevelData;
        // and name it as the GameObject's name with the .Prefab format
        if (!Directory.Exists("Assets/GeneratedLevels"))
            AssetDatabase.CreateFolder("Assets", "GeneratedLevels");

        string localPath = "Assets/GeneratedLevels/" + name + ".prefab";

        GameObject prefabobj = new GameObject(name);

        LevelWave level = prefabobj.AddComponent<LevelWave>();

        level.level = obj.levels[0].level;
        level.defaultExp = obj.levels[0].defaultExp;
        level.backgroundSprite = obj.levels[0].backgroundSprite;
        level.Waves = obj.levels[0].Waves;
        level.nightMode = obj.levels[0].nightMode;
        level.nightMultiplierFixedAmount = obj.levels[0].nightModeXpMultiplier;
        // Create the new Prefab and log whether Prefab was saved successfully.
        bool prefabSuccess;

        PrefabUtility.SaveAsPrefabAssetAndConnect(prefabobj, localPath, InteractionMode.UserAction, out prefabSuccess);
        if (prefabSuccess == true)
            Debug.LogError("Prefab was saved successfully");
        else
            Debug.LogError("Prefab failed to save" + prefabSuccess);

        Apply();
        AssetDatabase.Refresh();
        DestroyImmediate(prefabobj);
    }
    protected void Edit(LevelWave level, string name = null)
    {
        // if (!gameLevelSetup)
        //     EditorUtility.DisplayDialog("Editing level", "First Open Game level setup!", "Ok", "Exit");
        LevelData obj = serializedObject.targetObject as LevelData;
        if (name != null)
            level.gameObject.name = name;
        level.level = obj.levels[0].level;
        level.defaultExp = obj.levels[0].defaultExp;
        level.backgroundSprite = obj.levels[0].backgroundSprite;
        level.Waves = obj.levels[0].Waves;
        level.nightMode = obj.levels[0].nightMode;
        level.nightMultiplierFixedAmount = obj.levels[0].nightModeXpMultiplier;
        Apply();
        EditorUtility.SetDirty(level.gameObject);
        PrefabUtility.RecordPrefabInstancePropertyModifications(level.gameObject);
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        Apply();
        AssetDatabase.Refresh();
    }
    protected void LoadLevel(LevelWave level)
    {
        LevelData obj = serializedObject.targetObject as LevelData;
        obj.levels[0].level = level.level;
        obj.levels[0].defaultExp = level.defaultExp;
        obj.levels[0].backgroundSprite = level.backgroundSprite;
        obj.levels[0].Waves = level.Waves;
        obj.levels[0].nightMode = level.nightMode;
        obj.levels[0].nightModeXpMultiplier = level.nightMultiplierFixedAmount;
        serializedObject = new SerializedObject(obj);
        Apply();
        AssetDatabase.Refresh();
    }
    protected void RunTestScene(LevelEnemyManager LEM)
    {
        Apply();
        GameManager gameManager = FindObjectOfType<GameManager>();
        SpriteRenderer background = GameObject.FindGameObjectWithTag("BackGroundLEM").GetComponent<SpriteRenderer>();
        LevelData obj = serializedObject.targetObject as LevelData;

        LEM.EnemyWaves = obj.levels[0].Waves;
        gameManager.currentExp = obj.levels[0].defaultExp;
        background.sprite = obj.levels[0].backgroundSprite;
        PrefabUtility.RecordPrefabInstancePropertyModifications(LEM.gameObject);
        PrefabUtility.RecordPrefabInstancePropertyModifications(gameManager.gameObject);
        PrefabUtility.RecordPrefabInstancePropertyModifications(background.gameObject);
        PrefabUtility.ApplyPrefabInstance(LEM.gameObject, InteractionMode.AutomatedAction);
        PrefabUtility.ApplyPrefabInstance(gameManager.gameObject, InteractionMode.AutomatedAction);
        PrefabUtility.ApplyPrefabInstance(background.gameObject, InteractionMode.AutomatedAction);
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
    }

}
