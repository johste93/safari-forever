using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CountryWindow))]
public class CountryWindowEditor : Editor {

	private CountryWindow current
	{
		get
		{
			return (CountryWindow) target;
		}
	}

	public override void OnInspectorGUI()
    {
       	DrawDefaultInspector();
		
        if(GUILayout.Button("Spawn Countries"))
        {
            current.SpawnCountryButtons();
        }

		if(GUI.changed)
		{
			EditorUtility.SetDirty( current );
		}
    }
}
