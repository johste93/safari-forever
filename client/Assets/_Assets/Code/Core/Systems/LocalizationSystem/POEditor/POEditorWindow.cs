using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;

#if UNITY_EDITOR
namespace Chumpware.POEditor
{
	public class POEditorWindow : EditorWindow
	{
		private string apiToken {
			get {
				return EditorPrefs.GetString("POEditorApiToken");
			}
			set {
				EditorPrefs.SetString("POEditorApiToken", value);
			}
		}
		private string projectId {
			get {
				return EditorPrefs.GetString("POEditorProjectId");
			}
			set {
				EditorPrefs.SetString("POEditorProjectId", value);
			}
		}

		private string relativePathToLanguageFiles = "Resources/LocalizedText";

		private POEditorLanguage[] availableLanguages;

		private bool apiTokenAndProjectIdSet {
			get {
				return !string.IsNullOrWhiteSpace(apiToken) && !string.IsNullOrWhiteSpace(projectId);
			}
		}

		private void OnEnable()
		{
			availableLanguages = new POEditorLanguage[0];

			if(apiTokenAndProjectIdSet)
				FetchAvailableLanguages();
		}

		public void OnGUI()
		{
			apiToken = EditorGUILayout.TextField("api token", apiToken);
			projectId = EditorGUILayout.TextField("project id", projectId);
			relativePathToLanguageFiles = EditorGUILayout.TextField("path", relativePathToLanguageFiles);

			GUI.enabled = apiTokenAndProjectIdSet;
			if(GUILayout.Button("Fetch available languages"))
			{
				FetchAvailableLanguages();
			}
			GUI.enabled = true;

			GUILayout.Space(20f);
			foreach(POEditorLanguage language in availableLanguages)
			{
				GUI.enabled = apiTokenAndProjectIdSet;
				if(GUILayout.Button($"Download: {language.name}"))
				{
					DownloadLanguage(language);
				}
				GUI.enabled = true;
			}

			GUILayout.Space(10f);
			GUI.enabled = availableLanguages != null && availableLanguages.Length > 0 && apiTokenAndProjectIdSet;
			if(GUILayout.Button("Download all"))
			{
				DownloadAll();
			}
			GUI.enabled = true;
		}

		[MenuItem("Chumpware/POEditor")]
		static void Init()
		{
			// Get existing open window or if none, make a new one:
			POEditorWindow window = (POEditorWindow)EditorWindow.GetWindow(typeof(POEditorWindow));
			window.Show();
		}

		private void FetchAvailableLanguages()
		{
			POEditorAPI.FetchAvailableLanguagues(apiToken, projectId, (fetchLanguagesSuccess, fetchLanguagesResponse)=>
			{
				if(!fetchLanguagesSuccess)
					return;

				availableLanguages = fetchLanguagesResponse.result.languages;
			});
		}

		private void DownloadLanguage(POEditorLanguage language)
		{
			POEditorAPI.FetchExportUrl(apiToken, projectId, language.code, (fetchUrlSuccess, fetchUrlResponse)=>
			{
				if(!fetchUrlSuccess)
					return;

				POEditorAPI.ExportLanguage(fetchUrlResponse.result.url, (downloadSuccess, filename, data)=>
				{
					if(!downloadSuccess)
						return;

					if(string.IsNullOrWhiteSpace(filename))
						filename = language.name + ".json";

					string path = Path.Combine(Application.dataPath, relativePathToLanguageFiles);
					path = Path.Combine(path, filename);

					if(File.Exists(path))
					{
						Debug.Log("Deleted old file");
						File.Delete(path);
					}

					File.WriteAllBytes(path, data);

					Debug.Log($"<color=green>Successully written to file to path: {path}</color>");
				});
			});
		}

		private void DownloadAll()
		{
			foreach(POEditorLanguage language in availableLanguages)
			{
				DownloadLanguage(language);
			}
		}
	}
}
#endif