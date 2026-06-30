using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnRectTransformDimensionsChangeListener : MonoBehaviour
{
	public UIEvent onRectTransformDimensionsChange;

	private void OnRectTransformDimensionsChange()
	{
		if(onRectTransformDimensionsChange != null)
			onRectTransformDimensionsChange.Invoke();
	}
}
