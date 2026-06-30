using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chumpware.Networking;
using Newtonsoft.Json;
using System.IO;

namespace Chumpware.POEditor
{
	public class POEditorAPI
	{
		private const string fetchAvailableLanguaguesEndpoint = "https://api.poeditor.com/v2/languages/list";
		private const string fetchExportUrlEndpoint = "https://api.poeditor.com/v2/projects/export";

		public static void FetchAvailableLanguagues(string apiToken, string projectId, System.Action<bool, POEditorFetchLanguagesResult> onComplete)
		{
			Dictionary<string,object> formData = new Dictionary<string, object>();
			formData.Add("api_token", apiToken);
			formData.Add("id", projectId);
			
			EditorRestClient.PostRequest(fetchAvailableLanguaguesEndpoint, new Dictionary<string, string>(), formData, (success, response, responseHeaders)=>
			{
				if(!success)
				{
					Debug.LogError(response);
					onComplete?.Invoke(false, null);
					return;
				}	

				Debug.Log(response);

				POEditorFetchLanguagesResult fetchLanguagesResult = JsonConvert.DeserializeObject<POEditorFetchLanguagesResult>(response);

				if(fetchLanguagesResult == null)
				{
					Debug.LogError("Unable to parse POEditor Response");
					onComplete?.Invoke(false, null);
					return;
				}

				onComplete?.Invoke(true, fetchLanguagesResult);
			});
		}

		public static void FetchExportUrl(string apiToken, string projectId, string languageCode, System.Action<bool, POEditorFetchExportUrlResult> onComplete)
		{
			Dictionary<string,object> formData = new Dictionary<string, object>();
			formData.Add("api_token", apiToken);
			formData.Add("id", projectId);
			formData.Add("language", languageCode);
			formData.Add("type", "key_value_json");
			
			EditorRestClient.PostRequest(fetchExportUrlEndpoint, new Dictionary<string, string>(), formData, (success, response, responseHeaders)=>
			{
				if(!success)
				{
					Debug.LogError(response);
					onComplete?.Invoke(false, null);
					return;
				}	

				Debug.Log(response);

				POEditorFetchExportUrlResult fetchExportUrlResult = JsonConvert.DeserializeObject<POEditorFetchExportUrlResult>(response);

				if(fetchExportUrlResult == null)
				{
					Debug.LogError("Unable to parse POEditor Response");
					onComplete?.Invoke(false, null);
					return;
				}

				onComplete?.Invoke(true, fetchExportUrlResult);
			});
		}

		public static void ExportLanguage(string downloadUrl, System.Action<bool, string, byte[]> onComplete)
		{
			EditorRestClient.GetRequest(downloadUrl, (success, response, data, responseHeaders)=>
			{
				if(!success)
				{
					onComplete?.Invoke(false, null, null);
					return;
				}

				string fileName = "";

				if(responseHeaders.ContainsKey("Content-Disposition"))
				{	
					string contentDisposition = responseHeaders["Content-Disposition"];

					const string filename="filename=";
					int index = contentDisposition.LastIndexOf(filename,System.StringComparison.OrdinalIgnoreCase);
					if (index > -1)
					{
						fileName = contentDisposition.Substring(index+filename.Length).Replace("\"", "");
					}
				}

				onComplete?.Invoke(true, fileName, data);
			});
		}
	}
}