using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideDebuggerButton : MonoBehaviour {

	public void OnClick()
	{
		((DebugManager) DebugManager.instance).ShowDebugger(false);
	}
}
