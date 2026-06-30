using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Linq;
using TMPro;
using System;

[CustomEditor(typeof(LocalizedText))]
public class LocalizedTextEditor : Editor
{
    private LocalizedText localizedText { get { return target as LocalizedText; } }

    void OnEnable () {
        localizedText.Awake();
        UnityEditorInternal.ComponentUtility.MoveComponentUp(localizedText);

        if(localizedText.enabled)
            localizedText.On_LanguageChanged(Application.isPlaying ? SaveManager.currentSave.language : Globals.localizationConstants.defaultLanguage);
    }

    public override void OnInspectorGUI()
    {
        //localizedText._key = (TranslationKey) EditorGUILayout.Popup((int) localizedText._key, Localization.TextKeys.Select(k => k.Replace("_", "/")).ToArray());
        localizedText._key =  (TranslationKey)EditorGUILayout.Popup((int) localizedText._key, Enum.GetNames(typeof(TranslationKey)).Select(k => k.Replace("_", "/")).ToArray());


        if(GUILayout.Button("Reload Dictionary"))
        {
            Localization.ReloadDictionary();
        }

        if(GUI.changed)
        {
            Localization.ReloadDictionary();
            
            if(localizedText.enabled)
                localizedText.On_LanguageChanged(Application.isPlaying ? SaveManager.currentSave.language : Globals.localizationConstants.defaultLanguage);

            EditorUtility.SetDirty(target);
            EditorSceneManager.MarkAllScenesDirty();
        }

        DrawDefaultInspector();
    }
}
