using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;

public class BuildTargetProcessor : IActiveBuildTargetChanged
{
    public int callbackOrder { get { return 0; } }
    public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
    {
        Debug.Log("Switched build target to " + newTarget);

        EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;

        scenes[0].enabled = newTarget != BuildTarget.WebGL;
        scenes[1].enabled = newTarget != BuildTarget.WebGL;

        EditorBuildSettings.scenes = scenes;
    }
}
