using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Chumpware.Tools
{
#if UNITY_EDITOR
    public class ScreenshotTool : EditorWindow
    {
        private const string RESOURCE_PATH = "";
        private const string FILENAME = "ScreenshotToolSettings";

        private static string path { get { return RESOURCE_PATH + FILENAME; } }

        private static ScreenshotToolSettings _settings;
        public static ScreenshotToolSettings settings
        {
            get
            {
                if (!_settings)
                {
                    _settings = Resources.Load<ScreenshotToolSettings> (path);

                    if (!_settings)
                    {
                        _settings = CreateInstance<ScreenshotToolSettings> ();

                        Debug.LogWarning ("ScreenshotToolSettings ScriptableObject not found in Resources! See: " + path);
                    }
                }

                return _settings;
            }
        }

        private SerializedObject serializedObject {
            get {
                return new SerializedObject(this);
            }
        }

        private static Queue<ScreenshotResolution> screenshotQueue;
        private DateTime startTime;
        private bool isPaused;

        private void OnGUI()
	    {
            GUILayout.Space(10);
            DrawList();

            settings.disableUI = EditorGUILayout.Toggle(settings.disableUI);

            GUILayout.Space(20);
            if(GUILayout.Button("Snap!", GUILayout.Height(40)))
            {
                CaptureScreenshots();
            }
        }

        private void CaptureScreenshots()
        {
            DisableUI();

            isPaused = EditorApplication.isPaused;
            EditorApplication.isPaused = true;
            screenshotQueue = new Queue<ScreenshotResolution>();
            startTime = DateTime.Now;

            cooldown = 1;

			AddCustomSizes();
            for(int i = 0; i < settings.resolutions.Count; i++)
            {
                screenshotQueue.Enqueue(settings.resolutions[i]);
            }
        }

        private void CaptureScreenshot(int index)
        {
            DisableUI();

            isPaused = EditorApplication.isPaused;
            EditorApplication.isPaused = true;
            screenshotQueue = new Queue<ScreenshotResolution>();
            startTime = DateTime.Now;

            cooldown = 1;

            AddCustomSizes();

            screenshotQueue.Enqueue(settings.resolutions[index]);
        }

        private Dictionary<Canvas, bool> canvases;
        private void DisableUI()
        {
            if(!settings.disableUI)
                return;

            canvases = new Dictionary<Canvas, bool>();
            foreach( Canvas c in (Canvas[]) GameObject.FindObjectsOfType(typeof(Canvas)))
            {
                canvases.Add(c, c.enabled);
                c.enabled = false;
            }
        }

        private void EnableUI()
        {
            if(!settings.disableUI)
                return;
                
            if(canvases == null)
                return;

            foreach(KeyValuePair<Canvas, bool> kVP in canvases)
            {
                kVP.Key.enabled = kVP.Value;
            }
        }

        private int cooldown;
        private void Update()
        {
            if(screenshotQueue == null || screenshotQueue.Count == 0)
                return;
            
            if(cooldown > 0)
            {
                cooldown--;
                return;
            }

            cooldown = 1;

            ScreenshotResolution res = screenshotQueue.Dequeue();
            int index = SetResolution(res.width, res.height);
            CaptureScreenshot(res.width, res.height, res.name);
            //GameViewUtility.RemoveCustomSize(GetGameViewSizeGroupType(), index);
            
            if(screenshotQueue.Count == 0)
            {
				//Done
                EnableUI();
				EditorApplication.isPaused = isPaused;
				RemoveCustomSizes();
			}
        }

		private void AddCustomSizes()
		{
			for(int i = 0; i < settings.resolutions.Count; i++)
			{
				int width = settings.resolutions[i].width;
				int height = settings.resolutions[i].height;

				int index = GameViewUtility.FindSize(GameViewUtility.GameViewSizeType.FixedResolution, GetGameViewSizeGroupType(), width, height);
				if(index == -1)
					GameViewUtility.AddCustomSize(GameViewUtility.GameViewSizeType.FixedResolution, GetGameViewSizeGroupType(), width, height, $"{width}x{height}");
			}
		}

		private void RemoveCustomSizes()
		{
			for(int i = 0; i < settings.resolutions.Count; i++)
			{
				int width = settings.resolutions[i].width;
				int height = settings.resolutions[i].height;
				
				int index = GameViewUtility.FindSize(GameViewUtility.GameViewSizeType.FixedResolution, GetGameViewSizeGroupType(), width, height);
				if(index > 27)
					GameViewUtility.RemoveCustomSize(GetGameViewSizeGroupType(), index);
			}
		}

        private void CaptureScreenshot(int width, int height, string name)
        {
            string folder = System.IO.Path.Combine("Poio_ScreenShots", startTime.ToString("yyyy_MM_dd"));
            string time = startTime.ToString("H.mm.ss");
			string filename = $"{time}_({width}x{height})_{name}.png";
			FileOperations.CreateDirectory(System.IO.Path.Combine(path, folder));

			ScreenCapture.CaptureScreenshot(System.IO.Path.Combine(System.IO.Path.Combine(path, folder), filename), 1);
        }

        private int SetResolution(int width, int height)
        {	
            int index = GameViewUtility.FindSize(GameViewUtility.GameViewSizeType.FixedResolution, GetGameViewSizeGroupType(), width, height);
            /*
			if(index == -1)
                GameViewUtility.AddCustomSize(GameViewUtility.GameViewSizeType.FixedResolution, GetGameViewSizeGroupType(), width, height, $"{width}x{height}");

            index = GameViewUtility.FindSize(GameViewUtility.GameViewSizeType.FixedResolution, GetGameViewSizeGroupType(), width, height);
			*/
            GameViewUtility.SetSize(index);
            return index;
        }

        private void DrawList()
        {
            EditorGUI.BeginChangeCheck();
            GUILayout.Label("Resolutions:");
            for(int i = 0; i < settings.resolutions.Count; i++)
            {
                GUILayout.BeginHorizontal();
                if(GUILayout.Button("Snap!", GUILayout.Width(50)))
                {
                    CaptureScreenshot(i);
                }

                GUILayout.Label($"{i}:", GUILayout.Width(20));
                
                GUILayout.Label("Width:");
                settings.resolutions[i].width = EditorGUILayout.IntField(settings.resolutions[i].width);

                GUILayout.Label("Height:");
                settings.resolutions[i].height = EditorGUILayout.IntField(settings.resolutions[i].height);


                GUILayout.Label("Name:");
                settings.resolutions[i].name = EditorGUILayout.TextField(settings.resolutions[i].name);

                GUI.color = Color.red;
                if(GUILayout.Button("X", GUILayout.Width(20)))
                {
                    settings.resolutions.RemoveAt(i);
                }
                GUI.color = Color.white;
                GUILayout.EndHorizontal();
            }

            GUI.color = Color.green;
            if(GUILayout.Button("Add", GUILayout.Width(50)))
            {
                settings.resolutions.Add(new ScreenshotResolution());
            }
            GUI.color = Color.white;

            if (EditorGUI.EndChangeCheck())
			{
                EditorUtility.SetDirty(settings);
                serializedObject.ApplyModifiedProperties();
            }
        }

        private GameViewSizeGroupType GetGameViewSizeGroupType()
        {
            switch(EditorUserBuildSettings.activeBuildTarget)
            {
                case BuildTarget.iOS:
                    return GameViewSizeGroupType.iOS;
                case BuildTarget.Android:
                    return GameViewSizeGroupType.Android;
                default:
                    return GameViewSizeGroupType.Standalone;
            }
        }

        [MenuItem("Chumpware/ScreenShot Tool")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(ScreenshotTool));
        }
    }
#endif
}