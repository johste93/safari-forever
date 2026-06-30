using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Random = UnityEngine.Random;

public class RecordButton : MonoBehaviour
{
	public CanvasGroup canvasGroup;
	public RectTransform anchorTransform;

	public Color grayColor;

	public Image icon;
	public Image surface;
	public Image radialDial;
	public Image overwriteDial;

	public GameObject defaultIcon;
	public GameObject stopRecordingIcon;
	public GameObject readyIcon;

	public bool showRecordMenuWindow;

	public void OnClick()
	{
		if(GifRecorder.instance.GetState() == RecordingState.Disabled)
		{
			if(GameMaster.instance.HasStartedRunning())
				GifRecorder.instance.SetState(RecordingState.Recording);
			else
				GifRecorder.instance.SetState(RecordingState.Ready);
		}
		else
		{
			if(GifRecorder.instance.HasRecorded())
			{
				GifRecorder.instance.EndRecording();
				showRecordMenuWindow = true;
			}

			GifRecorder.instance.SetState(RecordingState.Disabled);
		}
	}

	private void FixedUpdate()
	{
		if (!showRecordMenuWindow)
			return;

		showRecordMenuWindow = false;
		DialogCanvas.instance.ShowRecordMenuWindow();
	}

	public void OnLongPress()
    {
    }

	public void OnPointerDown()
    {
        Audio.Play(SFX.instance.ui.flatButton, Channel.UI).SetPitch(Random.Range(0.9f, 1.1f));
		anchorTransform.DOComplete();
        anchorTransform.DOPunchScale(Vector3.one * 0.1f, 0.3f);
    }

	public void On_StateChanged(RecordingState state)
	{
		switch(state)
		{
			default:
			case RecordingState.Disabled:
				icon.color = grayColor;
				//surface.color = Color.white;
				surface.gameObject.SetActive(true);
				stopRecordingIcon.SetActive(false);
				radialDial.gameObject.SetActive(false);
				overwriteDial.gameObject.SetActive(false);
				readyIcon.SetActive(false);
				break;
			case RecordingState.Ready:
				icon.color = Color.white;
				//surface.color = grayColor;
				surface.gameObject.SetActive(false);
				stopRecordingIcon.SetActive(false);
				radialDial.gameObject.SetActive(false);
				overwriteDial.gameObject.SetActive(false);
				readyIcon.SetActive(true);
				break;
			case RecordingState.Recording:
				//surface.color = grayColor;
				surface.gameObject.SetActive(false);
				stopRecordingIcon.SetActive(true);
				readyIcon.SetActive(false);
				radialDial.fillAmount = 0f;
				overwriteDial.fillAmount = 0f;
				radialDial.gameObject.SetActive(true);
				overwriteDial.gameObject.SetActive(true);
				break;
			case RecordingState.DoneRecording:
				surface.gameObject.SetActive(false);
				stopRecordingIcon.SetActive(true);
				readyIcon.SetActive(false);
				//radialDial.fillAmount = 0f;
				//overwriteDial.fillAmount = 0f;
				radialDial.gameObject.SetActive(true);
				overwriteDial.gameObject.SetActive(true);
				break;
		}
	}

	private void On_ProgressUpdate(float progress)
	{
		float normalizedProgress = progress - Mathf.FloorToInt(progress);

		if(Mathf.FloorToInt(progress) % 2 == 0)
		{
			radialDial.transform.SetAsLastSibling();
			radialDial.fillAmount = normalizedProgress;
			overwriteDial.fillAmount = progress;
		}
		else
		{
			overwriteDial.transform.SetAsLastSibling();
			overwriteDial.fillAmount = normalizedProgress;
			radialDial.fillAmount = progress;
		}
	}

	private void On_EnterPlayMode()
	{
		if(GameMaster.instance.GetCurrentMode() == GameMode.Create)
			return;

		if(SystemInfo.systemMemorySize < Globals.gifConstants.minimumSystemMemorySize || SystemInfo.processorCount < Globals.gifConstants.minimumNumberOfProcessorCores)
			return;

		canvasGroup.blocksRaycasts = true;
		canvasGroup.alpha = 1;
	}

	private void On_ExitPlayMode()
	{
		if(GameMaster.instance.GetCurrentMode() == GameMode.Create)
			return;
			
		canvasGroup.blocksRaycasts = false;
		canvasGroup.alpha = 0;
	}

    private void OnEnable()
    {
        GameMaster.On_EnterPlayMode += On_EnterPlayMode;
		GameMaster.On_ExitPlayMode += On_ExitPlayMode;

		GifRecorder.On_StateChanged += On_StateChanged;
		GifRecorder.On_ProgressUpdate += On_ProgressUpdate;

		On_StateChanged(GifRecorder.instance.GetState());
    }

    private void Unsubscribe()
    {
        GameMaster.On_EnterPlayMode -= On_EnterPlayMode;
		GameMaster.On_ExitPlayMode -= On_ExitPlayMode;

		GifRecorder.On_StateChanged -= On_StateChanged;
		GifRecorder.On_ProgressUpdate -= On_ProgressUpdate;
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
		anchorTransform.DOKill();
	}
}
