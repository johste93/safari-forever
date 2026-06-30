using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : ImmortalSingleton<GameManager>
{
	private static Vector2Int? nativeResolution;
	
    private void Start()
    {    
        //Globals.gameConstants.gameSpeed = SaveManager.currentSave.gameSpeed;
		SetLowPowerMode();
    }

	private void SetLowPowerMode()
	{
		if(!nativeResolution.HasValue)
		{
			nativeResolution = new Vector2Int(Screen.currentResolution.width, Screen.currentResolution.height);
		}

		int width = nativeResolution.Value.x;
		int height =  nativeResolution.Value.y;

		if(SaveManager.currentSave.lowPowerMode)
		{
			width /= 2;
			height /= 2;
		}
#if !UNITY_WEBGL
		Application.targetFrameRate = SaveManager.currentSave.lowPowerMode ? 60 : 120;
#endif
		Screen.SetResolution(width, height, Screen.fullScreenMode);
	}

	private void OnEnable()
	{
		LowPowerButton.On_LowPowerButtonClicked += SetLowPowerMode;
	}

	private void Unsubscribe()
	{
		LowPowerButton.On_LowPowerButtonClicked -= SetLowPowerMode;
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
