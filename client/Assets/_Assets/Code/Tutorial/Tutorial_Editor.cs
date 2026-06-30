using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_Editor : MonoBehaviour {
	
	/*
	public CanvasGroup[] canvasGroups;

	private Coroutine dragAndDropRoutine;
	private float timePassed;
	private const float graceDuration = 2f;

	private bool ignoreUnregistedLevelEntities;

	private IEnumerator Start()
	{
		yield return 0;

		if(LevelBuilder.instance.IsRoomEmpty())
		{
			SetFirstSiblingInteractable();

			if(dragAndDropRoutine == null)
				dragAndDropRoutine = StartCoroutine(DragAndDropTutorial());
		}
	}

	private IEnumerator DragAndDropTutorial()
	{
		timePassed = 0f;

		while(timePassed < graceDuration)
		{
			if(!SuspensionManager.IsSuspended())
				timePassed += Time.deltaTime;

			yield return 0;
		}

		//GenericTutorialCanvas.ShowDragAnimationWithTracking(Toolbar.instance.baseGroup.GetObjectGroupByIndex(1).GetButtonByIndex(0).icon.transform, Camera.main, LevelBuilder.instance.transform, Camera.main);
	}

	private void On_LevelEntityRegistered()
	{	
		On_LevelEntityUnregistered();
	}

	private void On_LevelEntityUnregistered()
	{
		if(ignoreUnregistedLevelEntities) //This is here to avoid running this when changing rooms.
			return;

		UpdateToolbar();
	}

	private void UpdateToolbar()
	{
		bool interactable = !LevelBuilder.instance.IsRoomEmpty();

		if(interactable)
		{
			if(dragAndDropRoutine != null)
			{
				StopCoroutine(dragAndDropRoutine);
				dragAndDropRoutine = null;
			}

			//GenericTutorialCanvas.Stop();
			SetAllInteractable();
		}
		else
		{
			if(dragAndDropRoutine == null)
				dragAndDropRoutine = StartCoroutine(DragAndDropTutorial());

			SetFirstSiblingInteractable();

			if(LevelBuilder.instance.IsRoomEmpty())
				Toolbar.instance.SetTabAndSubTab(0,0);
		}
	}

    private void SetFirstSiblingInteractable()
    {
		foreach(CanvasGroup cG in canvasGroups)
		{
			cG.interactable = false;
			cG.alpha = 0.25f;
		}

		for(int i = 0; i < Toolbar.instance.baseGroup.GetButtonCount(); i++)
		{
			Toolbar.instance.baseGroup.GetButtonByIndex(i).SetInteractable(false);
		}

		for(int i = 0; i < Toolbar.instance.baseGroup.GetObjectGroupByIndex(1).GetButtonCount(); i++)
		{
			Toolbar.instance.baseGroup.GetObjectGroupByIndex(1).GetButtonByIndex(i).SetInteractable(false);
		}

		Toolbar.instance.baseGroup.GetObjectGroupByIndex(1).GetButtonByIndex(0).SetInteractable(true);
    }
	
	private void SetAllInteractable()
	{
		foreach(CanvasGroup cG in canvasGroups)
		{
			cG.interactable = true;
			cG.alpha = 1f;
		}

		for(int i = 0; i < Toolbar.instance.baseGroup.GetButtonCount(); i++)
		{
			Toolbar.instance.baseGroup.GetButtonByIndex(i).SetInteractable(true);
		}

		for(int i = 0; i < Toolbar.instance.baseGroup.GetObjectGroupByIndex(1).GetButtonCount(); i++)
		{
			Toolbar.instance.baseGroup.GetObjectGroupByIndex(1).GetButtonByIndex(i).SetInteractable(true);
		}
	}

	private void On_BeforeSceneLoad()
	{
		Unsubscribe();
	}

	private void On_ChangedRoom()
	{
		if(GameMaster.instance.GetCurrentMode() == GameMode.Play)
			return;

		if(LevelBuilder.instance.IsRoomEmpty())
			Toolbar.instance.SetTabAndSubTab(0,0);

		ignoreUnregistedLevelEntities = false;

		UpdateToolbar();
	}

	private void Before_ChangedRoom()
	{
		ignoreUnregistedLevelEntities = true;
	}

	private void OnEnable()
	{
		Subscribe();
	}

	private void Subscribe()
	{
		LevelBuilder.On_LevelEntityRegistered += On_LevelEntityRegistered;
		LevelBuilder.On_LevelEntityUnregistered += On_LevelEntityUnregistered;
		SceneLoader.On_BeforeSceneLoad += On_BeforeSceneLoad;
		LevelBuilder.On_ChangedRoom += On_ChangedRoom;
		LevelBuilder.Before_ChangedRoom += Before_ChangedRoom;
	}

	private void Unsubscribe()
	{
		if(dragAndDropRoutine != null)
			StopCoroutine(dragAndDropRoutine);
			
		LevelBuilder.On_LevelEntityRegistered -= On_LevelEntityRegistered;
		LevelBuilder.On_LevelEntityUnregistered -= On_LevelEntityUnregistered;
		SceneLoader.On_BeforeSceneLoad -= On_BeforeSceneLoad;
		LevelBuilder.On_ChangedRoom -= On_ChangedRoom;
		LevelBuilder.Before_ChangedRoom -= Before_ChangedRoom;
	}

	private void OnDisable()
	{
		Unsubscribe();
	}

	private void OnDestroy()
	{
		Unsubscribe();
	}
	*/
}
