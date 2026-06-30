using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearDebuggerButton : MonoBehaviour
{
    public void OnClick()
	{
		((DebugManager) DebugManager.instance).Clear();
	}
}
