using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;
using System.Diagnostics;
using System;
using System.IO;
using Chumpware;
using System.Reflection;
using System.Linq;

public class FastlaneBuild
{
    static string[] EnabledScenes = FindEnabledEditorScenes();

    // ------------------------------------------------------------------------
    // called from Fastlane
    // ------------------------------------------------------------------------
    public static void Build()
    {    
        string buildTarget = "";
        string jobName = "";
        string buildVersion = "";
        string workspace = "";
        bool build_aab = false;

        // find: -executeMethod
        //   +1: FastlaneBuild.Build
        //   +2: BuildTarget e.g. "iOS" / "Android"
        //   +3: Job name "Safari_Forever_IOS"
        //   +4: buildVersion / jobNumber
        //   +5: workspace e.g. ~/Unity/SafariForever/
        string[] args = System.Environment.GetCommandLineArgs();

        for (int i=0; i<args.Length; i++)
        {
            if (args[i] == "-executeMethod")
            {
                if (i+3 < args.Length)
                {
                    // Build method is args[i+1]
                    buildTarget = args[i+2];
                    jobName = args[i+3];  
                    buildVersion = args[i+4]; 
                    workspace = args[i+5];
                    i += 5;
                }
                else 
                {
                    System.Console.WriteLine("[FastlaneBuild] Incorrect Parameters for -executeMethod Format: -executeMethod Build <build target> <platform> <job name> <output dir>");
                    return;
                }
            }

            if (args[i] == "-build_aab")
                build_aab = true;
        }

        string buildPath = Path.Combine(workspace, "Builds", buildTarget.ToLower(), buildVersion);

        BuildTarget BuildTarget;
        BuildTargetGroup BuildTargetGroup;
        if(buildTarget.ToLower() == "ios")
        {
            BuildTarget = BuildTarget.iOS;
            BuildTargetGroup = BuildTargetGroup.iOS;
        }
        else
        {
            BuildTarget = BuildTarget.Android;
            BuildTargetGroup = BuildTargetGroup.Android;

            buildPath = Path.Combine(buildPath, jobName + (build_aab ? ".aab" : ".apk"));
        }
        
        BuildProject(EnabledScenes, buildPath, BuildTargetGroup, BuildTarget, BuildOptions.None, buildVersion, build_aab);
    }

    // ------------------------------------------------------------------------
    // ------------------------------------------------------------------------
    private static string[] FindEnabledEditorScenes()
    {
        List<string> EditorScenes = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
            {
                EditorScenes.Add(scene.path);
            }
        }
        return EditorScenes.ToArray();
    }
    
    // ------------------------------------------------------------------------
    // e.g. BuildTargetGroup.Standalone, BuildTarget.StandaloneOSX
    // ------------------------------------------------------------------------
    private static void BuildProject(string[] scenes, string targetDir, BuildTargetGroup buildTargetGroup, BuildTarget buildTarget, BuildOptions buildOptions, string buildVersion, bool build_aab)
    {
        System.Console.WriteLine("[FastlaneBuild] Building:" + targetDir + " buildTargetGroup:" + buildTargetGroup.ToString() + " buildTarget:" + buildTarget.ToString());
    
        // https://docs.unity3d.com/ScriptReference/EditorUserBuildSettings.SwitchActiveBuildTarget.html
        bool switchResult = EditorUserBuildSettings.SwitchActiveBuildTarget(buildTargetGroup, buildTarget);
        if (switchResult)
        {
            System.Console.WriteLine("[FastlaneBuild] Successfully changed Build Target to: " + buildTarget.ToString());
        }
        else 
        {
            System.Console.WriteLine("[FastlaneBuild] Unable to change Build Target to: " + buildTarget.ToString() + " Exiting...");
            return;
        }

        if(buildTarget == BuildTarget.iOS)
        {
            PlayerSettings.iOS.buildNumber = $"{PlayerSettings.bundleVersion}.{buildVersion}";

            if (PlayerSettings.iOS.appleEnableAutomaticSigning)
            {
                System.Console.WriteLine("Disabling iOS automatic app signing");
                PlayerSettings.iOS.appleEnableAutomaticSigning = false;
            }
        }

        if(buildTarget == BuildTarget.Android)
        {
            PlayerSettings.Android.keystoreName = GetEnv("KEYSTORE_NAME");
            PlayerSettings.Android.keystorePass = GetEnv("KEYSTORE_PASS");
            PlayerSettings.Android.keyaliasName = GetEnv("KEYALIAS_NAME");
            PlayerSettings.Android.keyaliasPass = GetEnv("KEYALIAS_PASS");

            System.Console.WriteLine("[FastlaneBuild] keystore set: " + PlayerSettings.Android.keystoreName);

            EditorUserBuildSettings.buildAppBundle = build_aab; //Set to true to build aab.
            EditorUserBuildSettings.exportAsGoogleAndroidProject = false;

            if(int.TryParse(buildVersion, out int bundleVersionCode))
                PlayerSettings.Android.bundleVersionCode = bundleVersionCode;
            else
                System.Console.WriteLine("[FastlaneBuild][ERROR] Unable to parse bundleVersionCode!");
        }
        
        //We need todo this to avoid using old values for our build.
        ForceSavePlayerSettings();
    
        // https://docs.unity3d.com/ScriptReference/BuildPipeline.BuildPlayer.html
        BuildReport buildReport = BuildPipeline.BuildPlayer(scenes, targetDir, buildTarget, buildOptions);
        BuildSummary buildSummary = buildReport.summary;
        if (buildSummary.result == BuildResult.Succeeded)
        {
            System.Console.WriteLine("[FastlaneBuild] Build Success: Time:" + buildSummary.totalTime + " Size:" + buildSummary.totalSize + " bytes");
        }
        else 
        {
            System.Console.WriteLine("[FastlaneBuild] Build Failed: Time:" + buildSummary.totalTime + " Total Errors:" + buildSummary.totalErrors);
        }
    }

    private static void ForceSavePlayerSettings()
    {
      PlayerSettings playerSettings = Resources.FindObjectsOfTypeAll<PlayerSettings>().FirstOrDefault();
      EditorUtility.SetDirty(playerSettings);
      AssetDatabase.SaveAssets();
    }


    [PostProcessBuildAttribute(1)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject) 
    {
        if(target == BuildTarget.iOS)
        {
            System.Console.WriteLine("[FastlaneBuild] pathToBuiltProject: " + pathToBuiltProject);
            SystemUtil.Execute("Assets/Editor/SET_USYM_UPLOAD_AUTH_TOKEN.sh", new string[]{$"{pathToBuiltProject}/Unity-IPhone.xcodeproj/project.pbxproj"});
        }
    }

    private static string GetProjectRoot()
    {
        string codeBase = Assembly.GetExecutingAssembly().CodeBase;
        UriBuilder uri = new UriBuilder(codeBase);
        string path = Uri.UnescapeDataString(uri.Path);
        return Path.GetFullPath(new Uri(Path.Combine(Path.GetDirectoryName(path), "../../")).AbsolutePath);
    }

    private static string GetEnv(string _Key)
    {
      return Environment.GetEnvironmentVariable(_Key);
    }

    private static string GetRequiredEnv(string _Key)
    {
      string Value = Environment.GetEnvironmentVariable(_Key);
      if (String.IsNullOrEmpty(Value))
        throw new Exception("Missing or empty environment Variable for: " + _Key);
      return Value;
    }
}