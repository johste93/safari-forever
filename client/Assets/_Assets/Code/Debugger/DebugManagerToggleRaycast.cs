using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugManagerToggleRaycast : MonoBehaviour {

	public Image image;
	public Sprite blockRaycastSprite;
	public Sprite clickThroughRaycastSprite;

	public GameObject header;
	public GameObject footer;
	public Image sliderBackground;
	public GameObject slidingArea;

	public void OnClick()
	{
		bool visible = ((DebugManager) DebugManager.instance).ToggleRaycastBlocking();
		if(!visible)
		{
			image.sprite = blockRaycastSprite;
		}
		else
		{
			image.sprite = clickThroughRaycastSprite;
		}

		header.SetActive(visible);
		footer.SetActive(visible);
		sliderBackground.enabled = visible;
		slidingArea.SetActive(visible);
	}
}
