
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ImaginationOverflow.UniversalDeepLinking;
using UnityEngine.SceneManagement;

public class DeepLinkSingleton : MonoBehaviour {

	public static DeepLinkSingleton instance;

	public string cachedShareCode;
	public string cachedUserId;

	private bool _isSubscribed;
	public bool isSubscribed{
		get{
			return _isSubscribed;	
		}
		private set {
			_isSubscribed = value;
		}
	}


	private Coroutine unsubscribeRoutine;

	private void Awake()
	{
		if(instance != null)
		{
			Destroy(gameObject);
			return;
		}

		instance = this;
		DontDestroyOnLoad(gameObject);

		Subscribe();
	}

	private void Instance_LinkActivated(LinkActivation linkActivation)
	{
		ParseDeeplink(linkActivation.RawQueryString);
		KillCoroutine();
		Unsubscribe();
	}

	public bool ParseDeeplink(string rawQueryString)
	{
		string[] segments = rawQueryString.Split('/');

		if(rawQueryString.ToLower().Contains("sf4.life"))
		{
			if(segments.Length == 5)
			{
				string levelId = segments[4];

				if(string.IsNullOrEmpty(levelId))
					return false;

				LoadLevel(levelId);
				return true;
			}
			else if(segments.Length == 6)
			{
				string nickname = segments[4];

				if(string.IsNullOrEmpty(nickname))
					return false;

				string identifier = segments[5];

				if(string.IsNullOrEmpty(identifier))
					return false;

				LoadProfile(nickname, identifier);
				return true;
			}
			return false;
		}
		else if(rawQueryString.ToLower().Contains("safariforever.com"))
		{
			if(segments.Length == 4)
			{
				string levelId = segments[3];

				if(string.IsNullOrEmpty(levelId))
					return false;

				LoadLevel(levelId);
				return true;

			}
			else if(segments.Length == 5)
			{
				string nickname = segments[3];

				if(string.IsNullOrEmpty(nickname))
					return false;

				string identifier = segments[4];

				if(string.IsNullOrEmpty(identifier))
					return false;

				LoadProfile(nickname, identifier);
				return true;
			}	
			return false;
		}
		return false;
	}

	private void LoadProfile(string nickname, string identifier)
	{
		if(PopupCanvas.instance == null)
		{
			cachedUserId = $"{nickname}#{identifier}";
			return;
		}

		DialogCanvas.instance?.CloseAllWindows();

		bool wasPausedByDeepLink = !SuspensionManager.IsSuspended();
		if(wasPausedByDeepLink)
			SuspensionManager.Suspend(true);

		DialogCanvas.instance.ShowLoading();
		UserAPI.FetchProfile(nickname, identifier, (success, profile)=>
		{
			DialogCanvas.instance.HideLoading();
			if(profile == null)
				return;

			PopupView currentView = PopupCanvas.instance.GetActivePopup();
			if(currentView == PopupCanvas.instance.profileView)
			{
				((ProfileView)PopupCanvas.instance.profileView).Initalize(profile);			
				PopupCanvas.instance.profileView.Show(()=>{
					PopupCanvas.instance.CloseCanvas();
					if(wasPausedByDeepLink)
						SuspensionManager.Suspend(false);
				});
			}
			else
			{
				PopupCanvas.instance.CloseCurrentView(false, ()=>
				{
					((ProfileView)PopupCanvas.instance.profileView).Initalize(profile);			
					PopupCanvas.instance.profileView.Show(()=>{
						PopupCanvas.instance.CloseCanvas();
						if(wasPausedByDeepLink)
							SuspensionManager.Suspend(false);
					});
				});
			}
		});
	}

	private void LoadLevel(string levelId)
	{
		if(PopupCanvas.instance == null)
		{
			cachedShareCode = levelId;
			return;
		}

		DialogCanvas.instance?.CloseAllWindows();

		bool wasPausedByDeepLink = !SuspensionManager.IsSuspended();
		if(wasPausedByDeepLink)
			SuspensionManager.Suspend(true);
	
		LevelAPI.Download(levelId, (level)=>
		{
            if(level == null)
                return;

			PopupView currentView = PopupCanvas.instance.GetActivePopup();
			if(currentView == PopupCanvas.instance.inspectView)
			{
				((LevelInspector)PopupCanvas.instance.inspectView).Initalize(level);
				PopupCanvas.instance.inspectView.Show(()=>{
					PopupCanvas.instance.CloseCanvas();
					if(wasPausedByDeepLink)
						SuspensionManager.Suspend(false);
				});
			}
			else
			{
				PopupCanvas.instance.CloseCurrentView(false, ()=>
				{
					((LevelInspector)PopupCanvas.instance.inspectView).Initalize(level);
					PopupCanvas.instance.inspectView.Show(()=>{
						PopupCanvas.instance.CloseCanvas();
						if(wasPausedByDeepLink)
							SuspensionManager.Suspend(false);
					});
				});
			}

        });
	}

	public void LoadCachedUserId(System.Action<bool> onComplete)
	{
		if(string.IsNullOrWhiteSpace(cachedUserId))
		{
			onComplete?.Invoke(false);
			return;
		}

		if(PopupCanvas.instance == null)
		{
			onComplete?.Invoke(false);
			return;
		}

		DialogCanvas.instance?.CloseAllWindows();

		string[] segments = cachedUserId.Split('#');
		if(segments.Length != 2)
		{
			onComplete?.Invoke(false);
			return;
		}

		bool wasPausedByDeepLink = !SuspensionManager.IsSuspended();
		if(wasPausedByDeepLink)
			SuspensionManager.Suspend(true);

		UserAPI.FetchProfile(segments[0], segments[1], (success, profile)=>
		{
			if(profile == null)
            {
				onComplete?.Invoke(false);
				return;
			}

            PopupView currentView = PopupCanvas.instance.GetActivePopup();
			if(currentView == PopupCanvas.instance.profileView)
			{
				((ProfileView)PopupCanvas.instance.profileView).Initalize(profile);			
				PopupCanvas.instance.profileView.Show(()=>{
					PopupCanvas.instance.CloseCanvas();
					if(wasPausedByDeepLink)
						SuspensionManager.Suspend(false);
				});
			}
			else
			{
				PopupCanvas.instance.CloseCurrentView(false, ()=>
				{
					((ProfileView)PopupCanvas.instance.profileView).Initalize(profile);			
					PopupCanvas.instance.profileView.Show(()=>{
						PopupCanvas.instance.CloseCanvas();
						if(wasPausedByDeepLink)
							SuspensionManager.Suspend(false);
					});
				});
			}

			onComplete?.Invoke(true);
		});

		cachedUserId = "";
	}

	public void LoadCachedShareCode(System.Action<bool> onComplete)
	{
		if(string.IsNullOrWhiteSpace(cachedShareCode))
		{
			onComplete?.Invoke(false);
			return;
		}

		if(PopupCanvas.instance == null)
		{
			onComplete?.Invoke(false);
			return;
		}

		DialogCanvas.instance?.CloseAllWindows();

		bool wasPausedByDeepLink = !SuspensionManager.IsSuspended();
		if(wasPausedByDeepLink)
			SuspensionManager.Suspend(true);
	
		LevelAPI.Download(cachedShareCode, (level)=>
		{
            if(level == null)
            {
				onComplete?.Invoke(false);
				return;
			}

			PopupView currentView = PopupCanvas.instance.GetActivePopup();
			if(currentView == PopupCanvas.instance.inspectView)
			{
				((LevelInspector)PopupCanvas.instance.inspectView).Initalize(level);
					PopupCanvas.instance.inspectView.Show(()=>{
						PopupCanvas.instance.CloseCanvas();
						if(wasPausedByDeepLink)
							SuspensionManager.Suspend(false);
					});
			}
			else
			{
				PopupCanvas.instance.CloseCurrentView(false, ()=>
				{
					((LevelInspector)PopupCanvas.instance.inspectView).Initalize(level);
					PopupCanvas.instance.inspectView.Show(()=>{
						PopupCanvas.instance.CloseCanvas();
						if(wasPausedByDeepLink)
							SuspensionManager.Suspend(false);
					});
				});
			}

			onComplete?.Invoke(true);
        });
		cachedShareCode = "";
	}

	private void OnApplicationPause(bool appIsPaused)
    {
		KillCoroutine();

        if(appIsPaused)
		{
			Subscribe();
		}
		else
		{
			unsubscribeRoutine = StartCoroutine(UnsubscribeRoutine());
		}
    }

	private IEnumerator UnsubscribeRoutine()
	{
		yield return new WaitForSeconds(3f);
		Unsubscribe();
	}

	public void KillCoroutine()
	{
		if(unsubscribeRoutine == null)
			return;

		StopCoroutine(unsubscribeRoutine);
		unsubscribeRoutine = null;
	}

	public void Subscribe()
	{
		if(isSubscribed)
			return;

		DeepLinkManager.Instance.LinkActivated  += Instance_LinkActivated; 
		isSubscribed = true;
	}

	public void Unsubscribe()
	{
		if(!isSubscribed)
			return;
	
		DeepLinkManager.Instance.LinkActivated -= Instance_LinkActivated;
		isSubscribed = false;
	}
	
	private void OnDisable()
	{
		Unsubscribe();
	}

	private void OnDestroy()
	{
		Unsubscribe();
	}
}
