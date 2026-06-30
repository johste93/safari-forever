using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

namespace Chumpware.Networking
{
	//Dont use this at runtime.
	public class EditorRestClient
	{
		public static void GetRequest(string url, System.Action<bool, string, byte[], Dictionary<string, string>> onComplete, string authToken = null)
		{
			Debug.Log($"<color=yellow>{Time.frameCount} GET: {url}</color>");
			DateTime timeOut = DateTime.Now;
			timeOut = timeOut.AddSeconds(5);

			using (UnityWebRequest request = UnityWebRequest.Get(url))
			{
				if (authToken != null)
				{
					request.SetRequestHeader("Authorization", "Bearer " + authToken);
				}

				request.SendWebRequest();

				while (!request.isDone && DateTime.Compare(DateTime.Now, timeOut) < 0f ) 
				{
					//lets do nothing for a while.
				}

				if(request.isNetworkError || request.isHttpError || request.responseCode != 200) 
				{
					Debug.Log(request.responseCode + ": " + request.error + " " + request.downloadHandler.text);
					if(onComplete != null) onComplete(false, request.responseCode + ": " + request.error, null, request.GetResponseHeaders());
				}
				else
				{
					onComplete?.Invoke(true, request.downloadHandler.text, request.downloadHandler.data, request.GetResponseHeaders());
				}
			}
		}

		public static void PutRequest(string url, string jsonContent, System.Action<bool, string, Dictionary<string, string>> onComplete, string authToken = null)
		{
			Debug.Log($"<color=yellow>{Time.frameCount} PUT: {url}</color>");
			DateTime timeOut = DateTime.Now;
			timeOut = timeOut.AddSeconds(5);

			byte[] bytes = System.Text.Encoding.UTF8.GetBytes(jsonContent);

			using (UnityWebRequest request = UnityWebRequest.Put(url, bytes))
			{	
				request.SetRequestHeader("Content-Type", "application/json");

				if (authToken != null)
				{
					request.SetRequestHeader("Authorization", "Bearer " + authToken);
				}

				request.SendWebRequest();

				while (!request.isDone && DateTime.Compare(DateTime.Now, timeOut) < 0f ) 
				{
					//lets do nothing for a while.
				}

				if(request.isNetworkError || request.isHttpError || request.responseCode != 200) 
				{
					Debug.Log(request.responseCode + ": " + request.error + " " + request.downloadHandler.text);
					if(onComplete != null) onComplete(false, request.responseCode + ": " + request.error, request.GetResponseHeaders());
				}
				else
				{
					if(onComplete != null) onComplete(true, request.downloadHandler.text, request.GetResponseHeaders());
				}
			}
		}
	
		public static void PostRequest(string url, Dictionary<string, string> headers, string jsonContent, System.Action<bool, string, Dictionary<string, string>> onComplete, string authToken = null) 
		{
			Debug.Log($"<color=yellow>{Time.frameCount} POST: {url}</color>");
			DateTime timeOut = DateTime.Now;
			timeOut = timeOut.AddSeconds(5);

			using (UnityWebRequest request = UnityWebRequest.Post(url, new WWWForm()))
			{
				foreach(KeyValuePair<string, string> kVP in headers)
					request.SetRequestHeader(kVP.Key, kVP.Value);

				if (jsonContent != null)
				{
					byte[] bytes = System.Text.Encoding.UTF8.GetBytes(jsonContent);
					UploadHandlerRaw uH = new UploadHandlerRaw(bytes);
					request.uploadHandler = uH;
					request.SetRequestHeader("Content-Type", "application/json");
				}

				if (authToken != null)
				{
					request.SetRequestHeader("Authorization", "Bearer " + authToken);
				}

				request.SendWebRequest();

				while (!request.isDone && DateTime.Compare(DateTime.Now, timeOut) < 0f ) 
				{
					//lets do nothing for a while.
				}

				if(request.isNetworkError || request.isHttpError || request.responseCode != 200) 
				{
					Debug.Log(request.responseCode + ": " + request.error + " " + request.downloadHandler.text);
					if(onComplete != null) onComplete(false, request.responseCode + ": " + request.error, request.GetResponseHeaders());
				}
				else
				{
					if(onComplete != null) onComplete(true, request.downloadHandler.text, request.GetResponseHeaders());
				}
			}
		}

		public static void PostRequest(string url, Dictionary<string, string> headers, Dictionary<string, object> formData, System.Action<bool, string, Dictionary<string, string>> onComplete, string authToken = null) 
		{
			Debug.Log($"<color=yellow>{Time.frameCount} POST: {url}</color>");
			DateTime timeOut = DateTime.Now;
			timeOut = timeOut.AddSeconds(5);

			WWWForm form = new WWWForm();
			foreach (KeyValuePair<string, object> post_arg in formData)
			{
				if(post_arg.Value.GetType() == typeof(byte[]))
				{
					form.AddBinaryData(post_arg.Key, (byte[])post_arg.Value);
				}
				else
				{
					form.AddField(post_arg.Key, post_arg.Value.ToString());
				}
			}
			
			using (UnityWebRequest request = UnityWebRequest.Post(url, form))
			{
				if (authToken != null)
					headers.Add("Authorization", "Bearer " + authToken);
				
				foreach(KeyValuePair<string, string> kVP in headers)
					request.SetRequestHeader(kVP.Key, kVP.Value);

				request.SendWebRequest();

				while (!request.isDone && DateTime.Compare(DateTime.Now, timeOut) < 0f ) 
				{
					//lets do nothing for a while.
				}

				if(request.isNetworkError || request.isHttpError || request.responseCode != 200) 
				{
					Debug.Log(request.responseCode + ": " + request.error + " " + request.downloadHandler.text);
					if(onComplete != null) onComplete(false, request.responseCode + ": " + request.error, request.GetResponseHeaders());
				}
				else
				{
					if(onComplete != null) onComplete(true, request.downloadHandler.text, request.GetResponseHeaders());
				}
			}
		}

		public static void DeleteRequest(string url, System.Action<bool, string, Dictionary<string, string>> onComplete, string authToken = null)
		{
			Debug.Log($"<color=yellow>{Time.frameCount} DELETE: {url}</color>");
			DateTime timeOut = DateTime.Now;
			timeOut = timeOut.AddSeconds(5);

			using (UnityWebRequest request = UnityWebRequest.Delete(url))
			{
				request.downloadHandler = new DownloadHandlerBuffer();

				if (authToken != null)
				{
					request.SetRequestHeader("Authorization", "Bearer " + authToken);
				}

				request.SendWebRequest();

				while (!request.isDone && DateTime.Compare(DateTime.Now, timeOut) < 0f ) 
				{
					//lets do nothing for a while.
				}

				if(request.isNetworkError || request.isHttpError || request.responseCode != 200) 
				{
					Debug.Log(request.responseCode + ": " + request.error + " " + request.downloadHandler.text);
					if(onComplete != null) onComplete(false, request.responseCode + ": " + request.error, request.GetResponseHeaders());
				}
				else
				{
					if(onComplete != null) onComplete(true, request.downloadHandler.text, request.GetResponseHeaders());
				}
			}
		}
	}
}