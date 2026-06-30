using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugManagerToggleLogTypeButton : MonoBehaviour {

	public LogType type;
	public Color disabledColor;

	private Color defaultColor;
	
	private Image image;

	private void Awake()
	{
		image = transform.GetChild(0).GetComponent<Image>();
		defaultColor = image.color;
	}

	public void OnClick()
	{
		switch(type)
		{
			case LogType.Log:
				((DebugManager)DebugManager.instance).ToggleLogs();
			break;
			case LogType.Warning:
				((DebugManager)DebugManager.instance).ToggleWarnings();
			break;
			case LogType.Error:
				((DebugManager)DebugManager.instance).ToggleErrors();
			break;
		}
	}

	private void On_ToggleLogType(LogType type, bool show)
	{
		if(this.type == type)
		{
			Color newColor = image.color;
			if(show)
				image.color = defaultColor;
			else
				image.color = disabledColor;
			
		}
	}

	private void OnEnable()
	{
		DebugManager.On_ToggleLogType += On_ToggleLogType;
	}

	private void Unsubscribe()
	{
		DebugManager.On_ToggleLogType -= On_ToggleLogType;
	}

	private void OnDestroy()
	{
		Unsubscribe();
	}

	private void OnDisable()
	{
		Unsubscribe();
	}
}
