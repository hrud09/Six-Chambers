using System.Collections.Generic;
using System.IO;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using UnityEngine;

#if UNITY_EDITOR
namespace BigBangStudio.SceneViewer
{
    public class SceneViewer : EditorWindow
{
        private List<string> scenePaths = new List<string>();
    private List<string> allDirectories = new List<string>();
    private List<string> activeDirectories = new List<string>();
    private Vector2 scrollPosition;
    private Vector2 sceneScrollPos;

    private const string ActiveDirectoriesKey = "SceneViewer.ActiveDirectories";
    private const string ActiveDirectoriesFoldoutKey = "SceneViewer.ActiveDirectoriesFoldout";

    private bool activeDirectoriesFoldout = true;

    [MenuItem("GameDevProdigy/Scene Viewer")]
    public static void ShowWindow()
    {
        SceneViewer window = EditorWindow.GetWindow<SceneViewer>();
        window.titleContent = new GUIContent("Scene Viewer");
        window.Show();
    }

    private void OnEnable()
    {
        FindScenes();
        FindAllDirectories();
        LoadActiveDirectories();
        GetActiveDirectoryState();
        FindScenes();
    }

    private void GetActiveDirectoryState()
    {
        activeDirectoriesFoldout = EditorPrefs.GetBool(ActiveDirectoriesFoldoutKey, true);
    }


    private void OnDisable()
    {
        SaveActiveDirectories();
    }

    private void OnGUI()
    {
        GUILayout.Label("Scene Viewer", EditorStyles.boldLabel);

        // Active selection list
        activeDirectoriesFoldout = EditorGUILayout.Foldout(activeDirectoriesFoldout, "Active Directories");
        if (activeDirectoriesFoldout)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(120));
            foreach (string directory in allDirectories)
            {
                bool isActive = activeDirectories.Contains(directory);
                bool newActive = EditorGUILayout.ToggleLeft(directory, isActive);
                if (newActive != isActive)
                {
                    if (newActive)
                    {
                        activeDirectories.Add(directory);
                    }
                    else
                    {
                        activeDirectories.Remove(directory);
                    }
                }
            }
            EditorGUILayout.EndScrollView();

            EditorGUILayout.EndVertical();
        }

        // Search scenes button
        if (GUILayout.Button("Search Scenes"))
        {
            FindScenes();
        }

        // Display scenes
        EditorGUILayout.LabelField("Scenes");
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        sceneScrollPos = EditorGUILayout.BeginScrollView(sceneScrollPos, GUILayout.Height(180));
        foreach (string scenePath in scenePaths)
        {
            string sceneName = Path.GetFileNameWithoutExtension(scenePath);
            if (GUILayout.Button(sceneName))
            {
                OpenScene(scenePath);
            }
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    private void FindScenes()
    {
        scenePaths.Clear();

        string[] sceneGuids = AssetDatabase.FindAssets("t:Scene");
        foreach (string guid in sceneGuids)
        {
            string scenePath = AssetDatabase.GUIDToAssetPath(guid);
            scenePaths.Add(scenePath);
        }

        // Filter scenes based on active directories
        scenePaths = scenePaths.Where(path => IsInActiveDirectory(path)).ToList();

        // Sort scenes based on name
        scenePaths = scenePaths.OrderBy(path => Path.GetFileNameWithoutExtension(path)).ToList();
    }

    private void FindAllDirectories()
    {
        allDirectories.Clear();

        string[] sceneGuids = AssetDatabase.FindAssets("t:Scene");
        foreach (string guid in sceneGuids)
        {
            string scenePath = AssetDatabase.GUIDToAssetPath(guid);
            string sceneDirectory = Path.GetDirectoryName(scenePath);
            if (!allDirectories.Contains(sceneDirectory))
            {
                allDirectories.Add(sceneDirectory);
            }
        }
    }

    private bool IsInActiveDirectory(string scenePath)
    {
        string sceneDirectory = Path.GetDirectoryName(scenePath);
        return activeDirectories.Contains(sceneDirectory);
    }

    private void LoadActiveDirectories()
    {
        if (EditorPrefs.HasKey(ActiveDirectoriesKey))
        {
            string serializedDirectories = EditorPrefs.GetString(ActiveDirectoriesKey);
            activeDirectories = serializedDirectories.Split(';').ToList();
        }
        else
        {
            activeDirectories.Clear();
        }
    }

    private void SaveActiveDirectories()
    {
        string serializedDirectories = string.Join(";", activeDirectories.ToArray());
        EditorPrefs.SetString(ActiveDirectoriesKey, serializedDirectories);
        EditorPrefs.SetBool(ActiveDirectoriesFoldoutKey, activeDirectoriesFoldout);

    }

    private void OpenScene(string scenePath)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(scenePath);
        }
    }
    }
}
#endif
