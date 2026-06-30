using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEditor;

public class SceneBrowser : EditorWindow
{

    [MenuItem("Chumpware/SceneBrowser")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(SceneBrowser));
    }
    
    public void OnGUI()
    {
        if(GUILayout.Button("Boot"))
        {
            EditorSceneManager.OpenScene("Assets/_Assets/Scenes/BOOT.unity", OpenSceneMode.Single);
        }

        if(GUILayout.Button("Menu"))
        {
            EditorSceneManager.OpenScene("Assets/_Assets/Scenes/MENU.unity", OpenSceneMode.Single);
        }

        if(GUILayout.Button("Game"))
        {
            EditorSceneManager.OpenScene("Assets/_Assets/Scenes/GAME.unity", OpenSceneMode.Single);
        }
    }
}
