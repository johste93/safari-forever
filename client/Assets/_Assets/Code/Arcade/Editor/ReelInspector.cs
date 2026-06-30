using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Reel))]
public class ReelInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Reel currentReel = (Reel) target;

        if(GUILayout.Button("Spinn"))
        {
            currentReel.Spinn();
        }

        if(GUILayout.Button("Stop"))
        {
            currentReel.Stop();
        }
    }
}
