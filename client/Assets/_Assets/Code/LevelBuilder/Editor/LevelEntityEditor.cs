using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelEntity))]
public class LevelEntityEditor : Editor 
{
    private LevelEntity entity{
        get{
            return (LevelEntity) target;
        }
    }

    public override void OnInspectorGUI()
    {
        GUILayout.BeginHorizontal();
        entity.id = EditorGUILayout.TextField("id",  entity.id);
        
        if(GUILayout.Button("Generate GUID", GUILayout.Width(30f)))
        {
            entity.id = System.Guid.NewGuid().ToString();
            EditorUtility.SetDirty(entity);
        }
        GUILayout.EndHorizontal();

        EditorGUILayout.LabelField($"Unique Id: {entity.uniqueId}");

        DrawDefaultInspector();
    }
}
