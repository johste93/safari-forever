using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListLoadAnimation : MonoBehaviour
{
	public RectTransform rectTransform;
	public Image radialImage;	

    public void SetValue(float value)
	{
		value = Mathf.Clamp01(value);
		radialImage.color = radialImage.color.SetAlpha(value);
		radialImage.fillAmount = value;
	}
}
