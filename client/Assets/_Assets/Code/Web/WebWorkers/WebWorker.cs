using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Chumpware.Security;
using System.Text;
using System.Linq;

public class WebWorker : MonoBehaviour {
	
	public MonoBehaviour employeer;
	private bool isFreelanceWorker;

	private const string heartbeatURL = "https://google.com";

	private string targetUrl;

	internal bool assumeConnectedToInternet = false;
	internal bool skipVersionVerification = false;

	private string color;

	private const int timeoutLimit = 20;

	public void Awake()
	{
		color = "#" + ColorUtility.ToHtmlStringRGB(ColorGenerator.GetRandomColor(0.9f));

		if((Application.isEditor || Debug.isDebugBuild) && Globals.debugConstants.verboseLogging) Debug.Log($"<color={color}>New WebWorker {gameObject.name} Hired!</color>");

		if(!Application.isPlaying)
		{
			Debug.LogError("Can only be used @Runtime");
			Seppuku();
			return;
		}

		//Dont destroy worker. Worker will kill itself when hard work is done.
		DontDestroyOnLoad(gameObject);
	}

	public void SetSupervisor(MonoBehaviour employeer)
	{
		this.employeer = employeer;
		isFreelanceWorker = this.employeer == null;
	}

	public void Get(string url, Dictionary<string, string> headers, System.Action<bool, string, byte[]> onComplete, string authToken = null, bool failSilently = false, int timeout = timeoutLimit)
	{
		targetUrl = url;
		CheckInternetConnection(failSilently, (isConnected, serverTime)=>
		{
			if(!isConnected)
			{
				onComplete?.Invoke(false, null, null);
				return;
			}

			VerifyClientVersion(failSilently, ()=>{
				StartCoroutine(GetRequest(url, headers, onComplete, failSilently, timeout, serverTime, authToken));
			});
		});
	}

	public void Put(string url, Dictionary<string, string> headers, SortedDictionary<string, object> body, System.Action<bool, string> onComplete, string authToken = null, bool failSilently = false, int timeout = timeoutLimit)
	{
		targetUrl = url;
		CheckInternetConnection(failSilently, (isConnected, serverTime)=>
		{
			if(!isConnected)
			{
				onComplete?.Invoke(false, null);
				return;
			}

			VerifyClientVersion(failSilently, ()=>{
				StartCoroutine(PutRequest(url, headers, body, onComplete, failSilently, timeout, serverTime, authToken));
			});
		});	
	}

	public void Post(string url, Dictionary<string, string> headers, SortedDictionary<string, object> body, System.Action<bool, string> onComplete, string authToken = null, bool failSilently = false, int timeout = timeoutLimit)
	{
		targetUrl = url;
		CheckInternetConnection(failSilently, (isConnected, serverTime)=>
		{
			if(!isConnected)
			{
				onComplete?.Invoke(false, null);
				return;
			}

			VerifyClientVersion(failSilently, ()=>{
				StartCoroutine(PostRequest(url, headers, body, onComplete, failSilently, timeout, serverTime, authToken));
			});
		});
	}

	/*
	public void Post(string url, Dictionary<string, string> headers, Dictionary<string, object> formData, System.Action<float> onProgressUpdate, System.Action<bool, string> onComplete, string authToken = null, bool failSilently = false, int timeout = timeoutLimit)
	{
		targetUrl = url;
		CheckInternetConnection(failSilently, (isConnected, serverTime)=>
		{
			if(!isConnected)
			{
				onComplete?.Invoke(false, null);
				return;
			}

			VerifyClientVersion(failSilently, ()=>{
				StartCoroutine(PostRequest(url, headers, formData, onProgressUpdate, onComplete, failSilently, timeout, serverTime, authToken));	
			});
		});
	}
	*/

	public void Delete(string url, Dictionary<string, string> headers, System.Action<bool, string> onComplete, string authToken = null, bool failSilently = false, int timeout = timeoutLimit)
	{
		targetUrl = url;
		CheckInternetConnection(failSilently, (isConnected, serverTime)=>
		{
			if(!isConnected)
			{
				onComplete?.Invoke(false, null);
				return;
			}

			VerifyClientVersion(failSilently, ()=>{
				StartCoroutine(DeleteRequest(url, headers, onComplete, failSilently, timeout, serverTime, authToken));
			});
		});	
	}

	public void Patch(string url, Dictionary<string, string> headers, SortedDictionary<string, object> body, System.Action<bool, string> onComplete, string authToken = null, bool failSilently = false, int timeout = timeoutLimit)
	{
		targetUrl = url;
		CheckInternetConnection(failSilently, (isConnected, serverTime)=>
		{
			if(!isConnected)
			{
				onComplete?.Invoke(false, null);
				return;
			}

			VerifyClientVersion(failSilently, ()=>{
				StartCoroutine(PatchRequest(url, headers, body, onComplete, failSilently, timeout, serverTime, authToken));
			});
		});	
	}

	private IEnumerator GetRequest(string url, Dictionary<string, string> headers, System.Action<bool, string, byte[]> onComplete, bool failSilently, int timeout, DateTimeOffset serverTime, string authToken = null)
	{
		if((Application.isEditor || Debug.isDebugBuild) && Globals.debugConstants.verboseLogging) Debug.Log($"<color={color}>{Time.frameCount}: {gameObject.name}: {DateTime.Now.ToShortTimeString()} GET: {url}</color>");

		using (UnityWebRequest request = UnityWebRequest.Get(url))
		{
			headers.Add("ClientVersion", Application.version);
			if (authToken != null)
				headers.Add("Authorization", "Bearer " + GetOneTimeToken(authToken, serverTime, null));
			
			foreach(KeyValuePair<string, string> kVP in headers)
				request.SetRequestHeader(kVP.Key, kVP.Value);

			request.timeout = timeout;
			yield return request.SendWebRequest();

			if(!isFreelanceWorker && employeer == null)
			{
				Debug.Log("Employeer no longer exsists, worker silently commited seppuku");
				request.Dispose();
				Seppuku();
				yield break;
			}

			if(request.isNetworkError || request.isHttpError || request.responseCode < 200 || request.responseCode > 299) 
			{
				WebErrorHandler.Handle(request, failSilently, ()=>{
					onComplete?.Invoke(false, null, null);
				});
			}
			else
			{	
				onComplete?.Invoke(true, request.downloadHandler.text, request.downloadHandler.data);
			}

			request.Dispose();
			Seppuku();
		}
	}

	private IEnumerator PutRequest(string url, Dictionary<string, string> headers, SortedDictionary<string, object> body, System.Action<bool, string> onComplete, bool failSilently, int timeout, DateTimeOffset serverTime, string authToken = null)
	{
		if(Application.isEditor || Debug.isDebugBuild) Debug.Log($"<color={color}>{Time.frameCount}: {gameObject.name}: {DateTime.Now.ToShortTimeString()} PUT: {url}</color>");
		
		string jsonBody = JsonConvert.SerializeObject(body);

		using (UnityWebRequest request = UnityWebRequest.Put(url, jsonBody))
		{
			headers.Add("ClientVersion", Application.version);
			headers.Add("Content-Type", "application/json");
			if (authToken != null)
				headers.Add("Authorization", "Bearer " + GetOneTimeToken(authToken, serverTime, body));
			
			foreach(KeyValuePair<string, string> kVP in headers)
				request.SetRequestHeader(kVP.Key, kVP.Value);

			request.timeout = timeout;
			yield return request.SendWebRequest();

			if(!isFreelanceWorker && employeer == null)
			{
				Debug.Log("Employeer no longer exsists, worker silently commited seppuku");
				request.Dispose();
				Seppuku();
				yield break;
			}

			if(request.isNetworkError || request.isHttpError || request.responseCode < 200 || request.responseCode > 299) 
			{
				WebErrorHandler.Handle(request, failSilently, ()=>{
					onComplete?.Invoke(false, null);
				});
			}
			else
			{
				onComplete?.Invoke(true, request.downloadHandler.text);
			}

			request.Dispose();
			Seppuku();
		}
	}

	private IEnumerator PostRequest(string url, Dictionary<string, string> headers, SortedDictionary<string, object> body, System.Action<bool, string> onComplete, bool failSilently, int timeout, DateTimeOffset serverTime, string authToken = null) 
	{
		if((Application.isEditor || Debug.isDebugBuild) && Globals.debugConstants.verboseLogging)  Debug.Log($"<color={color}>{Time.frameCount}: {gameObject.name}: {DateTime.Now.ToShortTimeString()} POST: {url}</color>");

		string jsonBody = JsonConvert.SerializeObject(body);

		using (UnityWebRequest request = UnityWebRequest.Post(url, new WWWForm()))
		{
			headers.Add("ClientVersion", Application.version);
			if (jsonBody != null)
			{
				byte[] bytes = System.Text.Encoding.UTF8.GetBytes(jsonBody);
				UploadHandlerRaw uH = new UploadHandlerRaw(bytes);
				request.uploadHandler = uH;
				headers.Add("Content-Type", "application/json");
			}

			if (authToken != null)
				headers.Add("Authorization", "Bearer " + GetOneTimeToken(authToken, serverTime, body));

			foreach(KeyValuePair<string, string> kVP in headers)
				request.SetRequestHeader(kVP.Key, kVP.Value);

			request.timeout = timeout;
			yield return request.SendWebRequest();
			
			if(!isFreelanceWorker && employeer == null)
			{
				Debug.Log("Employeer no longer exsists, worker silently commited seppuku");
				request.Dispose();
				Seppuku();
				yield break;
			}

			if(request.isNetworkError || request.isHttpError || request.responseCode < 200 || request.responseCode > 299) 
			{
				WebErrorHandler.Handle(request, failSilently, ()=>{
					onComplete?.Invoke(false, null);
				});
			}
			else
			{
				onComplete?.Invoke(true, request.downloadHandler.text);
			}

			request.Dispose();
			Seppuku();
		}
    }

	private IEnumerator PostRequest(string url, Dictionary<string, string> headers, Dictionary<string, object> formData, System.Action<float> onProgressUpdate, System.Action<bool, string> onComplete, bool failSilently, int timeout, DateTimeOffset serverTime, string authToken = null) 
	{
		if((Application.isEditor || Debug.isDebugBuild) && Globals.debugConstants.verboseLogging)  Debug.Log($"<color={color}>{Time.frameCount}: {gameObject.name}: {DateTime.Now.ToShortTimeString()} POST: {url}</color>");
		
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
			headers.Add("ClientVersion", Application.version);
			if (authToken != null)
				headers.Add("Authorization", "Bearer " + GetOneTimeToken(authToken, serverTime, null));
			
			foreach(KeyValuePair<string, string> kVP in headers)
				request.SetRequestHeader(kVP.Key, kVP.Value);

			request.timeout = timeout;
			request.SendWebRequest();

			while(!request.isDone)
			{
				onProgressUpdate?.Invoke(request.uploadProgress + request.downloadProgress);
				yield return 0;
			}

			onProgressUpdate?.Invoke(1f);


			if(!isFreelanceWorker && employeer == null)
			{
				Debug.Log("Employeer no longer exsists, worker silently commited seppuku");
				request.Dispose();
				Seppuku();
				yield break;
			}

			while (!request.isDone && (!request.isNetworkError || !request.isHttpError) ) 
			{
				yield return null;
			}

			if(request.isNetworkError || request.isHttpError || request.responseCode < 200 || request.responseCode > 299) 
			{
				WebErrorHandler.Handle(request, failSilently, ()=>{
					onComplete?.Invoke(false, null);
				});
			}
			else
			{
				onComplete?.Invoke(true, request.downloadHandler.text);
			}

			request.Dispose();
			Seppuku();
		}
    }

	private IEnumerator DeleteRequest(string url, Dictionary<string, string> headers, System.Action<bool, string> onComplete, bool failSilently, int timeout, DateTimeOffset serverTime, string authToken = null)
	{
		if((Application.isEditor || Debug.isDebugBuild) && Globals.debugConstants.verboseLogging)  Debug.Log($"<color={color}>{Time.frameCount}: {gameObject.name}: {DateTime.Now.ToShortTimeString()} DELETE: {url}</color>");

		using (UnityWebRequest request = UnityWebRequest.Delete(url))
		{
			headers.Add("ClientVersion", Application.version);
			if (authToken != null)
				headers.Add("Authorization", "Bearer " + GetOneTimeToken(authToken, serverTime, null));
			
			foreach(KeyValuePair<string, string> kVP in headers)
				request.SetRequestHeader(kVP.Key, kVP.Value);
			
			request.downloadHandler = new DownloadHandlerBuffer();
			request.timeout = timeout;
			yield return request.SendWebRequest();

			if(!isFreelanceWorker && employeer == null)
			{
				Debug.Log("Employeer no longer exsists, worker silently commited seppuku");
				request.Dispose();
				Seppuku();
				yield break;
			}

			if(request.isNetworkError || request.isHttpError || request.responseCode < 200 || request.responseCode > 299) 
			{
				WebErrorHandler.Handle(request, failSilently, ()=>{
					onComplete?.Invoke(false, null);
				});
			}
			else
			{
				onComplete?.Invoke(true, request.downloadHandler.text);
			}

			request.Dispose();
			Seppuku();
		}
	}

	private IEnumerator PatchRequest(string url, Dictionary<string, string> headers, SortedDictionary<string, object> body, System.Action<bool, string> onComplete, bool failSilently, int timeout, DateTimeOffset serverTime, string authToken = null)
	{
		if((Application.isEditor || Debug.isDebugBuild) && Globals.debugConstants.verboseLogging)  Debug.Log($"<color={color}>{gameObject.name}: {DateTime.Now.ToShortTimeString()} PATCH: {url}</color>");
		
		string jsonBody = JsonConvert.SerializeObject(body);

		using (UnityWebRequest request = UnityWebRequest.Put(url, jsonBody))
		{
			request.method = "PATCH";
			headers.Add("ClientVersion", Application.version);
			headers.Add("Content-Type", "application/json");

			if (authToken != null)
				headers.Add("Authorization", "Bearer " + GetOneTimeToken(authToken, serverTime, body));
			
			foreach(KeyValuePair<string, string> kVP in headers)
				request.SetRequestHeader(kVP.Key, kVP.Value);

			request.timeout = timeout;
			yield return request.SendWebRequest();

			if(!isFreelanceWorker && employeer == null)
			{
				Debug.Log("Employeer no longer exsists, worker silently commited seppuku");
				request.Dispose();
				Seppuku();
				yield break;
			}

			if(request.isNetworkError || request.isHttpError || request.responseCode < 200 || request.responseCode > 299) 
			{
				WebErrorHandler.Handle(request, failSilently, ()=>{
					onComplete?.Invoke(false, null);
				});
			}
			else
			{
				onComplete?.Invoke(true, request.downloadHandler.text);
			}

			request.Dispose();
			Seppuku();
		}
	}

	public void CheckInternetConnection(bool failSilently, System.Action<bool,DateTimeOffset> onComplete)
	{
		if(assumeConnectedToInternet)
		{
			onComplete(true, new DateTimeOffset());
			return;
		}

		StartCoroutine(TryConnect(failSilently, (isConnected, serverTime)=>
		{
			onComplete(isConnected, serverTime);
		}));
	}

	private IEnumerator TryConnect(bool failSilently, System.Action<bool, DateTimeOffset> onComplete)
	{
		string url = Globals.webConstants.GetHost() + "heartbeat";
		using (UnityWebRequest request = UnityWebRequest.Get(url))
		{
			//if((Application.isEditor || Debug.isDebugBuild) && Globals.debugConstants.verboseLogging) Debug.Log($"<color={color}>{Time.frameCount}: {gameObject.name}: {DateTime.Now.ToShortTimeString()} Test Internet: {url}</color>\nTargetURL: {targetUrl}");
			request.SetRequestHeader("ClientVersion", Application.version);
			request.timeout = timeoutLimit;
			yield return request.SendWebRequest();

			if(!isFreelanceWorker && employeer == null)
			{
				Debug.Log("Employeer no longer exsists, worker silently commited seppuku");
				request.Dispose();
				Seppuku();
				yield break;
			}

			if(request.isNetworkError || request.isHttpError)
			{
				WebErrorHandler.Handle(request, failSilently, ()=>{
					onComplete?.Invoke(false, new DateTimeOffset());
				});
			}
			else
			{
				if(DateTimeOffset.TryParse(request.downloadHandler.text, out DateTimeOffset serverTime))
				{
					onComplete?.Invoke(true, serverTime);
				}
				else
				{
					onComplete?.Invoke(false, new DateTimeOffset());
				}
			}
		}
 	} 

	private void VerifyClientVersion(bool failSilently, System.Action onSuccess)
	{
		if(skipVersionVerification)
		{
			onSuccess?.Invoke();
			return;
		}

		Client.VerifyVersion(failSilently, (success, isUpToDate)=>
		{
			if(!success || !isUpToDate)
				return;

			onSuccess?.Invoke();
		});	
	}

	private void Seppuku()
	{
		//if((Application.isEditor || Debug.isDebugBuild) && Globals.debugConstants.verboseLogging)  Debug.Log("<color=Red>Seppuku: " + gameObject.name + "</color>");
		Destroy(gameObject);
	}

	private string GetOneTimeToken(string token, DateTimeOffset serverTime, SortedDictionary<string, object> body)
	{
		string checksum = CreateMD5("");
		if(body != null)
		{
			string json = JsonConvert.SerializeObject(body);
			checksum = CreateMD5(json);
		}

		JObject jObject = new JObject();
		jObject.Add("tokenTime", serverTime);
		jObject.Add("token", token);
		jObject.Add("checksum", checksum);
		return StringCipher.Encrypt(jObject.ToString(), "P1*Vc9otb0%uJo%rMU2KWU3!tsI4CP0U@&*jlgsEoePnw*ej7GCI^Nb2a5L7To#h", 128);
	}

	private static string CreateMD5(string input)
	{
		// Use input string to calculate MD5 hash
		using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
		{
			byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
			byte[] hashBytes = md5.ComputeHash(inputBytes);

			// Convert the byte array to hexadecimal string
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < hashBytes.Length; i++)
			{
				sb.Append(hashBytes[i].ToString("X2"));
			}
			return sb.ToString();
		}
	}
/*
	public void CheckClientVersion(bool failSilently, System.Action<bool, bool> onComplete)
	{
		StartCoroutine(GetClientVersion( failSilently, onComplete));
	}

	private IEnumerator GetClientVersion(bool failSilently, System.Action<bool, bool> onComplete)
	{
		string url = Globals.webConstants.GetHost() + "client/version";
		using (UnityWebRequest request = UnityWebRequest.Get(url))
		{
			if((Application.isEditor || Debug.isDebugBuild) && Globals.debugConstants.verboseLogging)  Debug.Log($"<color={color}>{Time.frameCount}: {gameObject.name}: {DateTime.Now.ToShortTimeString()} GET: {url}</color>\nTargetURL: {targetUrl}");
			request.SetRequestHeader("ClientVersion", Application.version);
			request.timeout = timeoutLimit;
			yield return request.SendWebRequest();

			if(!isFreelanceWorker && employeer == null)
			{
				Debug.Log("Employeer no longer exsists, worker silently commited seppuku");
				request.Dispose();
				Seppuku();
				yield break;
			}

			if(request.isNetworkError || request.isHttpError)
			{
				WebErrorHandler.Handle(request, failSilently, ()=>{
					onComplete?.Invoke(false, false);
				});

				yield break;
			}

			if(!ClientVersion.Parse(request.downloadHandler.text, out ClientVersion earliestSupportedVersion))
            {
                Debug.LogError("Unable to parse client version from server");
				onComplete?.Invoke(false, false);
                yield break;
            }

            if(!ClientVersion.Parse(Application.version, out ClientVersion thisVersion))
            {
                Debug.LogError("'Application Version' bad SYNTAX");
                onComplete?.Invoke(false, false);
                yield break;
            }
                       
            if(!thisVersion.IsNewerThanOrEqual(earliestSupportedVersion))
            {
				Debug.LogError("Client Outdated!");

				if(failSilently)
				{
					onComplete?.Invoke(true, false);
					yield break;
				}
				
				new Dialog(Localization.GetTranslation("Error.OutdatedClient.Title",SaveManager.currentSave.language),
					Localization.GetTranslationFormat("Error.OutdatedClient.Body",SaveManager.currentSave.language,  thisVersion.ToString(), earliestSupportedVersion.ToString()),
					Localization.IsRightToLeftLanguage(SaveManager.currentSave.language))
					.AddNeutralButton("Generic.Ok", ()=>{
						onComplete?.Invoke(true, false);
					})
					.Show();

                yield break;
            }

			onComplete?.Invoke(true, true);
		}
 	}
*/
}
