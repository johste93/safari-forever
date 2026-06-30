using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ColorPalette))]
public class ColorPaletteEditor : Editor {

	private ColorPalette current
	{
		get
		{
			return (ColorPalette) target;
		}
	}

	public override void OnInspectorGUI()
    {
       	DrawDefaultInspector();
		current.main = EditorGUILayout.ColorField("Main", current.main);
		current.floor = EditorGUILayout.ColorField("Sub", current.floor);
		current.wall = EditorGUILayout.ColorField("Wall", current.wall);
		current.pattern = EditorGUILayout.ColorField("Pattern", current.pattern);

		if(GUI.changed)
		{
			EditorUtility.SetDirty( current );
		}
    }
}
