using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FSM_CharacterController2D;
using System;
using System.IO;
using TMPro;

public class GifRecorder : Singleton<GifRecorder>
{   
    public Camera recorderCamera;
    public TextMeshProUGUI nicknameTextMesh;

    public FixedAspectRatio watermarkAspectRatio;
    public CanvasScaler watermarkCanvasScaler;
    public RectTransform watermarkRectTransform;

    public delegate void StateChanged(RecordingState state);
    public static StateChanged On_StateChanged;

    public delegate void ProgressUpdate(float progress);
    public static ProgressUpdate On_ProgressUpdate;

    private RecordingState state = RecordingState.Disabled;

    public void SetState(RecordingState state)
    {
        if(this.state == state)
            return;
        
        this.state = state;

        //Debug.Log("State Changed: " + state);

        switch(this.state)
        {
            case RecordingState.Recording:
                StartRecording();
                break;
            case RecordingState.Ready:
                UpdateCameraSize(recorderCamera.aspect > 1f);
                UpdateCameraPosition();
                break;
        }

        if(this.state == RecordingState.Processing)
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        else
            Screen.sleepTimeout = SleepTimeout.SystemSetting;

        On_StateChanged?.Invoke(this.state);
    }

    public RecordingState GetState()
    {
        return state;
    }

    public bool HasRecorded()
    {
        return ProGifManager.Instance.HasRecorded();
    }

    private void UpdateCameraSize(bool horizontalMode)
	{
		Vector4 realLevelSize = LevelBuilder.instance.GetRealLevelSize();
		float levelWidth = realLevelSize.x;
		float levelHeight = realLevelSize.y;

        float heightBasedSize = (levelHeight / 2f);
        float widthBasedSize = (levelWidth / 2);

        watermarkAspectRatio.enabled = false;

        if( horizontalMode )
        {
            //Widescreen
            recorderCamera.orthographicSize = (Mathf.Max(widthBasedSize, heightBasedSize) + 3f);
            watermarkCanvasScaler.matchWidthOrHeight = 1f;

            
            watermarkAspectRatio.scaleByHeight = true;

            watermarkRectTransform.anchorMin = new Vector2(0.5f, 0f);
            watermarkRectTransform.anchorMax = new Vector2(0.5f, 1f);

            watermarkRectTransform.SetTop(0);
            watermarkRectTransform.SetBottom(0);
        }
        else
        {
            //Portait
            recorderCamera.orthographicSize = (Mathf.Max(widthBasedSize, heightBasedSize) + 3f) / recorderCamera.aspect;
            watermarkCanvasScaler.matchWidthOrHeight = 0;
            watermarkAspectRatio.scaleByHeight = false;

            watermarkRectTransform.anchorMin = new Vector2(0f, 0.5f);
            watermarkRectTransform.anchorMax = new Vector2(1f, 0.5f);

            watermarkRectTransform.SetLeft(0);
            watermarkRectTransform.SetRight(0);
        }

        watermarkAspectRatio.enabled = true;
	}

	private void UpdateCameraPosition()
	{
		Vector4 realLevelSize = LevelBuilder.instance.GetRealLevelSize();
		float x = realLevelSize.z;
		float y = realLevelSize.w;
		recorderCamera.transform.position = new Vector3(x,y,recorderCamera.transform.position.z); 
	}

    private void On_PlayerStartedRunning(FSM_CharacterController controller)
    {
        //Debug.Log("On_PlayerStartedRunning");

        if(state == RecordingState.DoneRecording)
            SetState(RecordingState.Ready);

        if(state == RecordingState.Ready)
            SetState(RecordingState.Recording);
    }

    private void On_PlayerDied(FSM_CharacterController controller)
    {
        if(state == RecordingState.Recording)
        {
            EndRecording();
            SetState(RecordingState.DoneRecording);
        }
    }

    private void StartRecording()
    {
        string nickname = string.Empty;
        if(SaveManager.currentSave.HasOnlineProfile())
        {
            SaveManager.currentSave.FetchOnlineProfile((profile)=>{
                nickname = $"{profile.nickname}#{profile.identifier}";
            });
        }

        nicknameTextMesh.text = nickname;

        ProGifManager.Instance.ClearRecorder();
        GifQualitySettings settings = Globals.gifConstants.GetQualitySettings((GifQuality.Best));
        ProGifManager.Instance.SetRecordSettings(new Vector2(Globals.gameConstants.levelHeight, Globals.gameConstants.levelHeight), settings.width, settings.height, settings.duration, settings.fps, Globals.gifConstants.repeats ? 0 : 1, settings.processingQuality);

        UpdateCameraSize(recorderCamera.aspect > 1f);
        UpdateCameraPosition();
        
        ProGifManager.Instance.StartRecord(recorderCamera,
			(progress)=>{
				//Debug.Log("[SimpleStartDemo] On record progress: " + progress);
                On_ProgressUpdate?.Invoke(progress);
			},
			()=>{
				//Debug.Log("[SimpleStartDemo] On recorder buffer max.");
                //DoneRecording();
			});
    }

    public void DoneRecording()
    {
        if(state == RecordingState.Recording)
        {
            EndRecording();
            SetState(RecordingState.DoneRecording);
        }
    }

    public void EndRecording()
    {
        //Debug.Log("EndRecording");
        ProGifManager.Instance.StopRecord();
        SetState(RecordingState.Stopped);
    }

    public void SaveRecording(System.Action<bool, string> onComplete = null)
    {
        SetState(RecordingState.Processing);

        string fileName = $"Replay_{DateTime.Now.ToString().Replace("\\","_").Replace("/", "_").Replace(" ", "_").Replace(":", "_")}";

            DialogCanvas.instance.ShowLoading();
        ProGifManager.Instance.StopAndSaveRecord(()=>{
            //Debug.Log("[SimpleStartDemo] On pre-processing done.");
            DialogCanvas.instance.HideLoading();
            DialogCanvas.instance.ShowProgress(true);
        }, (id, progress)=>{
            //Debug.Log("[SimpleStartDemo] On save progress: " + progress);
            DialogCanvas.instance.SetProgress(progress);
        }, (id, path)=>{
            //Debug.Log("[SimpleStartDemo] On saved, origin save path: " + path);
            DialogCanvas.instance.SetProgress(1f);
            ProGifManager.Instance.ClearRecorder();
            DialogCanvas.instance.HideProgress();
            SetState(RecordingState.Disabled);
        
            RequestPlayersPermission((granted)=>{
                if(!granted)
                {
                    File.Delete(path);
                    onComplete?.Invoke(false, "");
                }
                else
                {
                    SaveGifToMedia(path, fileName, onComplete);
                }
            }, NativeGallery.CheckPermission(NativeGallery.PermissionType.Write, NativeGallery.MediaType.Image) == NativeGallery.Permission.Denied? 1 : 0);
            
        }
        ,fileName);
    }

    private void SaveGifToMedia(string path, string fileName, System.Action<bool, string> onComplete)
    {
        if(!Application.isMobilePlatform)
        {   
            onComplete(true, path);
            return;
        }

        byte[] bytes = File.ReadAllBytes(path);
        NativeGallery.SaveImageToGallery(bytes, "SafariForever", fileName + ".gif", (success, _) => {
            onComplete(success, path);
        });
    }

    public void SuccessDialog(string path, System.Action<bool, string> onComplete)
    {
        TranslationKey bodyKey = Application.platform == RuntimePlatform.Android ? TranslationKey.Photos_Saved_Body_Android : TranslationKey.Photos_Saved_Body_Ios;

        if(DialogCanvas.instance.recordMenuWindow.gameObject.activeInHierarchy)
            DialogCanvas.instance.recordMenuWindow.gameObject.SetActive(false);

        this.Delay1Frame(()=>{
            new Dialog(TranslationKey.Photos_Saved_Title, bodyKey)
            .AddNeutralButton(TranslationKey.Generic_Ok, ()=>{
                DialogCanvas.instance.recordMenuWindow.gameObject.SetActive(false);
                onComplete?.Invoke(true, path);
            })
            .Show();
        });
    }

    private void RequestPlayersPermission(System.Action onCompletePositive, System.Action onCompleteNegative)
    {
        TranslationKey bodyKey = Application.platform == RuntimePlatform.Android ? TranslationKey.Photos_PermissionPromt_Body_Android : TranslationKey.Photos_PermissionPromt_Body_Ios;

        if(DialogCanvas.instance.recordMenuWindow.gameObject.activeInHierarchy)
            DialogCanvas.instance.recordMenuWindow.gameObject.SetActive(false);

        this.Delay1Frame(()=>{
            new Dialog(TranslationKey.Photos_PermissionPromt_Header, bodyKey)
            .AddNegativeButton(TranslationKey.Generic_Later, ()=>{
                onCompleteNegative?.Invoke();
            })
            .AddPositiveButton(TranslationKey.Generic_Ok, ()=>{
                onCompletePositive?.Invoke();
            })
            .Show();
        });
    }

    private void RequestPlayersPermission(System.Action<bool> onComplete, int attempt = 0)
    {
        if(DialogCanvas.instance.recordMenuWindow.gameObject.activeInHierarchy)
            DialogCanvas.instance.recordMenuWindow.gameObject.SetActive(false);

        this.Delay1Frame(()=>
        {
            if(!Application.isMobilePlatform)
            {
                onComplete?.Invoke(true);
                return;
            }

            
            NativeGallery.Permission permission = NativeGallery.CheckPermission(NativeGallery.PermissionType.Write, NativeGallery.MediaType.Image);
            if(permission != NativeGallery.Permission.Granted)
            {
                TranslationKey bodyKey = Application.platform == RuntimePlatform.Android ? TranslationKey.Photos_PermissionPromt_Body_Android : TranslationKey.Photos_PermissionPromt_Body_Ios;
                Dialog permissionDialog = new Dialog(TranslationKey.Photos_PermissionPromt_Header, bodyKey);

                if(Application.platform != RuntimePlatform.IPhonePlayer) {
                    permissionDialog.AddNegativeButton(TranslationKey.Generic_Later, ()=>{
                        onComplete.Invoke(false);
                    });
                }

                if(permission == NativeGallery.Permission.Denied)
                {
                    permissionDialog.AddPositiveButton(TranslationKey.Generic_OpenAppSettings, ()=>{
                        OpenSettingsMenu();
                        attempt++;
                        RequestPlayersPermission(onComplete, attempt);
                    }, true);
                }

                permissionDialog.AddPositiveButton(attempt == 0 ? TranslationKey.Generic_Continue : TranslationKey.Generic_TryAgain, 
                    ()=>{
                        NativeGallery.RequestPermissionAsync(NativeGallery.PermissionType.Write, NativeGallery.MediaType.Image).ContinueWith((x) => {
                            attempt++;
                            RequestPlayersPermission(onComplete, attempt);
                        });
                    }, attempt == 0);

                permissionDialog.Show();
            }
            else
            {
                onComplete.Invoke(true);
            }
        });
    }

    public void Reset()
    {
        if(state == RecordingState.Recording)
        {
            EndRecording();
        }
        
        ProGifManager.Instance.ClearRecorder();
        SetState(RecordingState.Disabled);
    }

    public void Prepare()
    {
        if(state == RecordingState.Recording)
        {
            EndRecording();
            SetState(RecordingState.Ready);
        }
    }

    private void On_EnterPlayMode()
    {
        //Debug.Log("On_EnterPlayMode");
        Reset();
    }

    private void On_ExitPlayMode()
    {
        //Debug.Log("On_ExitPlayMode");
        Reset();
    }

    private void On_LevelReset(bool manual)
    {
        //Debug.Log("On_LevelReset");
        Prepare(); //Dont delete the recording incase player like to save it after.
    }

    private void On_ReachedGoalBase()
    {
        //Debug.Log("On_ReachedGoalBase");

        if(state == RecordingState.Recording)
        {
            EndRecording();
            SetState(RecordingState.ReadyToProcess);
        }
    }

    private void On_SuspensionEvent(bool isSuspended)
    {
        if(state == RecordingState.Recording)
        {
            if(isSuspended)
            {
                ProGifManager.Instance.PauseRecord();
            }   
            else
            {
                ProGifManager.Instance.ResumeRecord();
            }
        }
    }

    private void On_OrientationChanged(DeviceOrientation orientation)
    {
        if(GifRecorder.instance.HasRecorded())
        {
            GifRecorder.instance.EndRecording();
            DialogCanvas.instance.ShowRecordMenuWindow();
        }

        GifRecorder.instance.SetState(RecordingState.Disabled);
        
        UpdateCameraSize( orientation != DeviceOrientation.Portrait );
        UpdateCameraPosition();
    }

    private void OnEnable()
    {
        GameMaster.On_PlayerStartedRunning += On_PlayerStartedRunning;
        //GameMaster.On_PlayerWon += On_PlayerWon;
        Goal.On_ReachedGoalBase += On_ReachedGoalBase;
        GameMaster.On_PlayerDied += On_PlayerDied;

        SuspensionManager.On_SuspensionEvent += On_SuspensionEvent;

        GameMaster.On_EnterPlayMode += On_EnterPlayMode;
        GameMaster.On_ExitPlayMode += On_ExitPlayMode;
        GameMaster.On_LevelReset += On_LevelReset;

        ScreenOrientationManager.On_OrientationChanged += On_OrientationChanged;
    }

    private void Unsubscribe()
    {
        GameMaster.On_PlayerStartedRunning -= On_PlayerStartedRunning;
        //GameMaster.On_PlayerWon -= On_PlayerWon;
        Goal.On_ReachedGoalBase -= On_ReachedGoalBase;
        GameMaster.On_PlayerDied -= On_PlayerDied;

        SuspensionManager.On_SuspensionEvent -= On_SuspensionEvent;

        GameMaster.On_EnterPlayMode -= On_EnterPlayMode;
        GameMaster.On_ExitPlayMode -= On_ExitPlayMode;
        GameMaster.On_LevelReset -= On_LevelReset;

        ScreenOrientationManager.On_OrientationChanged -= On_OrientationChanged;
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

/*
    private void OpenPhotos(string androidPath)
    {
#if UNITY_IPHONE
        string url = "photos-redirect://";
        Application.OpenURL(url);
#endif
#if UNITY_ANDROID
        Intent intent = new Intent(Intent.ACTION_VIEW);
        intent.setDataAndType(Uri.fromFile(new File(androidPath)), mediaFile.getExtension());
        intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
        //startActivity(intent);


        using (var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		using (AndroidJavaObject currentActivityObject = unityClass.GetStatic<AndroidJavaObject>("currentActivity"))
		{
			string packageName = currentActivityObject.Call<string>("getPackageName");
	
			using (var uriClass = new AndroidJavaClass("android.net.Uri"))
			using (AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("fromParts", "package", packageName, null))
			using (var intentObject = new AndroidJavaObject("android.content.Intent", "android.settings.APPLICATION_DETAILS_SETTINGS", androidPath))
			{
				intentObject.Call<AndroidJavaObject>("addCategory", "android.intent.category.CATEGORY_OPENABLE");
				//intentObject.Call<AndroidJavaObject>("setFlags", 0x10000000);
                intentObject.Call<AndroidJavaObject>("setAction", "android.intent.action.OPEN_DOCUMENT");
                intentObject.Call<AndroidJavaObject>("setType", "image/gif");
				currentActivityObject.Call("startActivity", intentObject);
			}
		}
#endif
    }
*/

    private void OpenSettingsMenu() 
	{
		#if UNITY_IPHONE
            string url = MyNativeBindings.GetSettingsURL();
            Application.OpenURL(url);
        #endif

		#if UNITY_ANDROID
		using (var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		using (AndroidJavaObject currentActivityObject = unityClass.GetStatic<AndroidJavaObject>("currentActivity"))
		{
			string packageName = currentActivityObject.Call<string>("getPackageName");
	
			using (var uriClass = new AndroidJavaClass("android.net.Uri"))
			using (AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("fromParts", "package", packageName, null))
			using (var intentObject = new AndroidJavaObject("android.content.Intent", "android.settings.APPLICATION_DETAILS_SETTINGS", uriObject))
			{
				intentObject.Call<AndroidJavaObject>("addCategory", "android.intent.category.DEFAULT");
				intentObject.Call<AndroidJavaObject>("setFlags", 0x10000000);
				currentActivityObject.Call("startActivity", intentObject);
			}
		}
		#endif
    }
}
