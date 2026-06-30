using UnityEngine;
using DG.Tweening;
using SafariForever.Toolbar;

public class CameraScaler : Singleton<CameraScaler> {

	private new Camera camera;

	private float currentMinWidth;
	private float currentMinHeight;

	private Vector3 cameraDefaultPosition;

	private float currentMinOrthographicSize;

	private float editorMinWidth = 12.5f; //in blocks.
	private bool isChangingRoom;

	private void Awake()
	{
		camera = GetComponent<Camera>();
		cameraDefaultPosition = transform.position;

		editorMinWidth = (Globals.gameConstants.levelWidth/2f) + 2f;
		currentMinWidth = editorMinWidth;
		currentMinOrthographicSize = Mathf.Max(16f, currentMinWidth / camera.aspect);
		camera.orthographicSize = currentMinOrthographicSize;

		//ZoomOutOfLevel(true);
	}

	private float CurrentWidth()
	{
		return camera.orthographicSize * camera.aspect;
	}

	private float CurrentHeight()
	{
		return camera.orthographicSize*2f;
	}

	public void ZoomInOnLogic(bool instant)
	{
		Vector4 maxLevelSize = new Vector4(Globals.gameConstants.levelWidth, Globals.gameConstants.levelHeight, 0, 0);
		ZoomIn(instant, maxLevelSize);
	}

	public void ZoomInOnLevel(bool instant)
	{
		Vector4 realLevelSize = LevelBuilder.instance.GetRealLevelSize();
		realLevelSize += new Vector4(1, 1, 0, 0);
		ZoomIn(instant, realLevelSize);
	}

	private void ZoomIn(bool instant, Vector4 realLevelSize)
	{
		//Incase our level is empty we avoid zooming in!
		if(realLevelSize.x < 1 || realLevelSize.y < 1 || realLevelSize.x > 100 || realLevelSize.y > 100)
			return;

		float levelWidth = realLevelSize.x;
		float levelHeight = realLevelSize.y;

		currentMinHeight = (levelHeight/2f) + 6;
		currentMinWidth = (levelWidth/2f) + 2;

#if UNITY_WEBGL
		currentMinHeight -= 2;
#endif

		if((currentMinWidth / camera.aspect) > currentMinHeight)
		{
			currentMinOrthographicSize = currentMinWidth / camera.aspect;
		}
		else
		{
			currentMinOrthographicSize = currentMinHeight;
		}

		UpdateCamera(currentMinOrthographicSize, new Vector3(realLevelSize.z, realLevelSize.w, transform.position.z), instant);
	}

	public void ZoomOutOfLevel(bool instant)
	{
		currentMinWidth = editorMinWidth;

		currentMinOrthographicSize = Mathf.Max(18f, currentMinWidth / camera.aspect);

		UpdateCamera(currentMinOrthographicSize, cameraDefaultPosition, instant);
	}


	private void UpdateCamera(float newOrtographicSize, Vector3 newCameraPos, bool instant)
	{	
		camera.DOKill();
		transform.DOKill();

		//if(GlobalSingleton.mode == GameMode.Create && !GameMaster.instance.IsPlaying())
#if !UNITY_WEBGL
		newCameraPos += new Vector3(0f, -(1.5f/camera.aspect), 0f);
#endif
		if(instant)
		{
			camera.orthographicSize = newOrtographicSize;
			transform.position = newCameraPos;
		}
		else
		{
			camera.DOOrthoSize(newOrtographicSize, instant ? 0f : 0.5f ).SetEase(Ease.InOutQuad);
			transform.DOMove(newCameraPos, instant ? 0f : 0.5f).SetEase(Ease.InOutQuad);
		}
	}

	private void On_EnterPlayMode()
	{
		ZoomInOnLevel(false);
	}

	private void On_ExitPlayMode()
	{
		if(Toolbar.instance.GetCurrentButtonIndex() == 0 || LogicCanvas.LogicVisible() || RailCanvas.RailsVisible())
			ZoomOutOfLevel(false);
		else
			ZoomInOnLevel(false);
	}

	public void AdjustCamera(bool instant = false)
	{
		if(GameMaster.instance.IsPlaying() || GlobalSingleton.mode != GameMode.Create)
		{
			ZoomInOnLevel(true);
		}
		else
		{
			if(Toolbar.instance.GetCurrentButtonIndex() == 0 || LogicCanvas.LogicVisible() || RailCanvas.RailsVisible())
				ZoomOutOfLevel(instant);
			else
				ZoomInOnLevel(instant);
		}
	}

	private void On_TabChange(int tabIndex)
	{	
		AdjustCamera();
	}

	private void On_EntityStoppedMoving(LevelEntity entity)
	{
		this.Delay1Frame(()=>{
			AdjustCamera();
		});
	}

	private void On_ChangedRoom()
	{
		AdjustCamera(true);
		isChangingRoom = false;
	}

	private void Before_ChangedRoom()
	{
		isChangingRoom = true;
	}

	private void On_LevelEntityUnregistered()
	{
		if(isChangingRoom)
			return;

		if(GlobalSingleton.mode == GameMode.Create)
		{
			if(LevelBuilder.instance.IsRoomEmpty())
			{
				AdjustCamera();
			}
		}
	}

	private void On_EntityChangedSize(LevelEntity entity)
	{
		this.Delay1Frame(()=>{
			AdjustCamera();
		});
	}

	private void On_LogicCanvasUpdate(bool visible)
	{
		if(visible)
			ZoomInOnLogic(false);
		else
			AdjustCamera();
	}

	private void On_RailCanvasUpdate(bool visible)
	{
		if(visible)
			ZoomInOnLogic(false);
		else
			AdjustCamera();
	}

    public void ResetLinkZoom()
    {
        transform.DOKill();
        transform.DOLocalMove(cameraDefaultPosition, 0.5f).SetEase(Ease.InOutQuad);

        currentMinWidth = editorMinWidth;

        currentMinOrthographicSize = Mathf.Max(18f, currentMinWidth / camera.aspect);

        camera.DOKill();
        camera.DOOrthoSize(currentMinOrthographicSize, 0.5f).SetEase(Ease.InOutQuad);
    }

    public void ZoomInOnLink(Vector2 startPositon, Vector2 endPositon)
    {
        Vector2 centerPostion = Vector2.Lerp(startPositon, endPositon, 0.5f);
        float distance = Vector2.Distance(startPositon, endPositon);

        float t = (distance / 20f);
        float targetSize = Mathf.Lerp(10f, Mathf.Max(18f, currentMinWidth / camera.aspect), t);
        //Debug.Log(t);
        Vector3 unclampedTargetPos = new Vector3(centerPostion.x, centerPostion.y, transform.position.z); 

        Vector3 clampedTargetPos = unclampedTargetPos;
        clampedTargetPos.x = Mathf.Clamp(unclampedTargetPos.x, -6f, 6f);
        clampedTargetPos.y = Mathf.Clamp(unclampedTargetPos.y, -7f, 7f);


        transform.position = Vector3.Lerp(transform.position, clampedTargetPos, 0.2f);
        camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, targetSize, 0.2f);
    }

	private void On_OrientationChanged(DeviceOrientation orientation)
    {
		AdjustCamera();
	}

	private void OnEnable()
	{
		GameMaster.On_EnterPlayMode += On_EnterPlayMode;
		GameMaster.On_ExitPlayMode += On_ExitPlayMode;

		LevelBuilder.On_ChangedRoom += On_ChangedRoom;
		LevelBuilder.Before_ChangedRoom += Before_ChangedRoom;
		LevelBuilder.On_LevelEntityUnregistered += On_LevelEntityUnregistered;

        LevelEntity.On_EntityStoppedMoving += On_EntityStoppedMoving;
		LevelEntity.On_EntityChangedSize += On_EntityChangedSize;

		Toolbar.On_TabChange += On_TabChange;

		LogicCanvas.On_LogicCanvasUpdate += On_LogicCanvasUpdate;
		RailCanvas.On_RailCanvasUpdate += On_RailCanvasUpdate;

		ScreenOrientationManager.On_OrientationChanged += On_OrientationChanged;
	}

	private void Unsubscribe()
	{
		GameMaster.On_EnterPlayMode -= On_EnterPlayMode;
		GameMaster.On_ExitPlayMode -= On_ExitPlayMode;

		LevelBuilder.On_ChangedRoom -= On_ChangedRoom;
		LevelBuilder.Before_ChangedRoom -= Before_ChangedRoom;
		LevelBuilder.On_LevelEntityUnregistered -= On_LevelEntityUnregistered;
		LevelEntity.On_EntityChangedSize -= On_EntityChangedSize;

        LevelEntity.On_EntityStoppedMoving -= On_EntityStoppedMoving;

		Toolbar.On_TabChange -= On_TabChange;

		LogicCanvas.On_LogicCanvasUpdate -= On_LogicCanvasUpdate;
		RailCanvas.On_RailCanvasUpdate -= On_RailCanvasUpdate;

		ScreenOrientationManager.On_OrientationChanged -= On_OrientationChanged;
	}

	private void OnDisable()
	{
		Unsubscribe();
	}

	private void OnDestroy()
	{
		Unsubscribe();
		KillAllTweens();
	}

	private void KillAllTweens()
	{
		transform.DOKill();
		camera.DOKill();
	}
}
