using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FSM_CharacterController2D;

public class RestartSwipe : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public RectTransform icon;

    private float opacity = 0f;

    private const float minimumDistance = 0.05f;
    private const float sensitivity = 10f;
    private const float defaultRotation = -40f;
    private const float rotationAmount = 40f;

    private FSM_CharacterController characterController;

    private void Update()
    {
        if(!GameMaster.instance.IsPlaying())
        {
            Reset();
            return;
        }

        if(SuspensionManager.IsSuspended())
        {
            Reset();
            return;
        }

        if(DebugManager.instance.IsOpen())
        {
            Reset();
            return;
        }

        canvasGroup.alpha = opacity;
        canvasGroup.blocksRaycasts = opacity > 0f;
        icon.eulerAngles = new Vector3(0,0, defaultRotation + (EaseCurve.OutQuad(0f, 1f, opacity) * rotationAmount));
    }

    private void On_TouchMaintained(TouchInfo touchInfo)
	{
        if(!GameMaster.instance.IsPlaying())
			return;

        if(SuspensionManager.IsSuspended())
            return;

        if(DebugManager.instance.IsOpen())
            return;

        Vector2 swipeDir = touchInfo.viewportPosition - touchInfo.viewportStartPosition;
        
        if(swipeDir.y < 0f)
        {
            opacity = Mathf.Clamp01((Mathf.Abs(swipeDir.y) - minimumDistance) * sensitivity);
        }
        else
        {
            opacity = 0f;
        }
	}

	private void On_TouchEnd(TouchInfo touchInfo)
	{
        if(!GameMaster.instance.IsPlaying())
			return;

        if(SuspensionManager.IsSuspended())
            return;

        if(DebugManager.instance.IsOpen())
            return;

		if(opacity >= 1f - Mathf.Epsilon)
            GameMaster.instance.ResetLevel(true);
        else
        {
            if(!Mathf.Approximately(opacity, 0f))
                characterController?.inputController.ConsumeInput();
        }

        Reset();
	}

    private void Reset()
    {
        opacity = 0f;
        canvasGroup.alpha = opacity;
        canvasGroup.blocksRaycasts = false;
        icon.eulerAngles = new Vector3(0,0,defaultRotation);
    }

    private void On_EnterPlayMode()
	{
		Reset();
	}

	private void On_ExitPlayMode()
	{
        Reset();
	}

    private void On_LevelReset(bool manual)
    {
        Reset();
    }

    private void On_PlayerStartedRunning(FSM_CharacterController characterController)
    {
        this.characterController = characterController;
    }

    private void OnEnable()
    {
        GameMaster.On_EnterPlayMode += On_EnterPlayMode;
		GameMaster.On_ExitPlayMode += On_ExitPlayMode;
        GameMaster.On_LevelReset += On_LevelReset;
        GameMaster.On_PlayerStartedRunning += On_PlayerStartedRunning;

		TouchInput.On_TouchEnd += On_TouchEnd;
		TouchInput.On_TouchMaintained += On_TouchMaintained;
    }

    private void Unsubscribe()
    {
        GameMaster.On_EnterPlayMode -= On_EnterPlayMode;
		GameMaster.On_ExitPlayMode -= On_ExitPlayMode;
        GameMaster.On_LevelReset -= On_LevelReset;
        GameMaster.On_PlayerStartedRunning -= On_PlayerStartedRunning;

		TouchInput.On_TouchEnd -= On_TouchEnd;
		TouchInput.On_TouchMaintained -= On_TouchMaintained;
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
