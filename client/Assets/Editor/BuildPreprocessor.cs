using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Analytics;

public class BuildPreprocessor
{
    public int callbackOrder { get { return 0; } }
    public void OnPreprocessBuild(BuildTarget target, string path)
    {
        Debug.Log("OnPreprocessBuild: " + path);
    }
}
