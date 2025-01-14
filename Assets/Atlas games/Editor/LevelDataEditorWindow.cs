using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System;
using System.Reflection;
using System.Linq;
using UnityEditor.Experimental.SceneManagement;

public class LevelDataEditorWindow : ExtendedEditorWindow
{
    string levelname;
    LevelWave Level;
    Vector2 scrollPos;
    static LevelDataEditorWindow window;
    string AtlasScenePath = "Assets/GeneratedLevels/Playing atlas Test.unity";

    public static void Open(LevelData level)
    {
        window = GetWindow<LevelDataEditorWindow>("Level editor");
        window.serializedObject = new SerializedObject(level);
    }
    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true);
        serializedProperty = serializedObject.FindProperty("levels");
        EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(1000));
        DrawProperties(serializedProperty, true);
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
        EditorGUILayout.LabelField("\n\n\n\t\t\t\t Edits and saves \n\n\n");
        LevelData levelData = serializedObject.targetObject as LevelData;
        int ExpMin = 0, ExpMax = 0;
        foreach (EnemyWave waves in levelData.levels[0].Waves)
        {
            foreach (EnemySpawn spawn in waves.enemySpawns)
            {
                if (spawn.enemy == null) continue;
                int expMin = spawn.enemy.GetComponent<GiveExpWhenDie>().expMin;
                int expMax = spawn.enemy.GetComponent<GiveExpWhenDie>().expMax;
                if (spawn.boosType != EnemySpawn.isBoss.NONE)
                {
                    expMin = spawn.BossMinExp;
                    expMax = spawn.BossMaxExp;
                }
                ExpMin += expMin;
                ExpMax += expMax;
            }
        }
        EditorGUILayout.LabelField($"Total Exp: {(int)ExpMin}-{(int)ExpMax}");

        EditorGUILayout.BeginHorizontal("box");
        Level = (LevelWave)EditorGUILayout.ObjectField(Level, typeof(LevelWave), true);
        if (GUILayout.Button("Load level data"))
        {
            if (Level != null)
                DrawProperties(Level, serializedProperty, true);
        }
        EditorGUILayout.EndHorizontal();

        levelname = EditorGUILayout.TextField("Level name, exp: 'Level 1'", levelname);

        EditorGUILayout.BeginHorizontal("box");
        Apply();
        if (GUILayout.Button("save and export"))
        {
            Apply();
            if (levelname != null)
            {
                createInstance(levelname);
                Level = null;
                Debug.LogError("Saved and exported the data");
            }
            else
            {
                EditorUtility.DisplayDialog("save and export", "the lavel name can't be Empty", "Ok", "Exit");
            }
        }
        if (GUILayout.Button("Edit Level"))
        {
            if (Level == null)
            {
                EditorUtility.DisplayDialog("Editing level data", "Select a level first !", "Ok", "Exit");
                return;
            }
            bool is_ok = EditorUtility.DisplayDialog("Edit level data", "Are you sure you want to edit the data?", "Edit", "Exit");
            if (is_ok)
            {
                Debug.LogError("Edited the data");
                Apply();
                Edit(Level, levelname);
                Level = null;
            }
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal("box");
        if (GUILayout.Button("Ready this level for test play"))
        {
            var obj = GameObject.FindGameObjectWithTag("EditorLEM");
            if (!obj)
            {
                bool res = EditorUtility.DisplayDialog("Playing Status", "Open 'Playing atlas test' scene first!", "Ok", "Exit");
                if (res)
                {
                    EditorSceneManager.OpenScene(AtlasScenePath);
                    return;
                }
                else
                    return;
            }
            LevelEnemyManager LEM = obj.GetComponent<LevelEnemyManager>();
            Debug.LogError("Ready for play");
            RunTestScene(LEM);
            EditorUtility.DisplayDialog("Playing Status", "Scene is ready for test", "Ok", "Exit");
            window.Close();
        }
        if (GUILayout.Button("Open Game level setup"))
        {
            Type t = typeof(PrefabStageUtility);
            MethodInfo mi = t.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static).Single(m => m.Name == "OpenPrefab" && m.GetParameters().Length == 1
                                                && m.GetParameters()[0].ParameterType == typeof(string));
            PrefabStage gamelevel = (PrefabStage)mi.Invoke(null, new object[] { gameLevelSetupPath });
            gameLevelSetup = gamelevel.FindComponentOfType<GameLevelSetup>();

        }
        EditorGUILayout.EndHorizontal();

    }
}
