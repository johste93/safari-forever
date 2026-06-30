using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;

#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

public class BuildPostprocessor
{
    [PostProcessBuildAttribute(200)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
#if UNITY_WEBGL

        //rename index.html -> PlayWebGL.cshtml
        string indexPath = Path.Combine(pathToBuiltProject, "index.html");
        string directoryPath = Path.Combine(pathToBuiltProject, "wwwroot", "Templates");

        Directory.CreateDirectory(directoryPath);

        File.Copy(indexPath, Path.Combine(directoryPath, "PlayWebGL.cshtml"));
        File.Move(indexPath, Path.Combine(pathToBuiltProject, "wwwroot", "Templates", "index.html"));

        //move Build -> wwwroot
        string originalPath = Path.Combine(pathToBuiltProject, "Build");
        string destinationBuildPath = Path.Combine(pathToBuiltProject, "wwwroot", "Build");
        Directory.Move(originalPath, destinationBuildPath);
#endif
#if UNITY_IOS
        if (target == BuildTarget.iOS)
        {
            // Get plist
            string plistPath = pathToBuiltProject + "/Info.plist";
            PlistDocument plist = new PlistDocument();
            plist.ReadFromString(File.ReadAllText(plistPath));

            // Get root
            PlistElementDict rootDict = plist.root;

            // Change value of NSCameraUsageDescription in Xcode plist
            var buildKey = "NSPhotoLibraryUsageDescription ";
            string destription = "Safari Forever requires access to Photo Library to save gifs.";
            rootDict.SetString(buildKey, destription);
            System.Console.WriteLine("NSPhotoLibraryUsageDescription  changed.");
            
            // Write to file
            File.WriteAllText(plistPath, plist.WriteToString());
        }
#endif
    }
}
