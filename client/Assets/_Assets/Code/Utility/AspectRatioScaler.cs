using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AspectRatioScaler : MonoBehaviour {

	private RectTransform rectTransform;

	public float maxAspect = 9f / 16f;

	public float scaleheight
	{
		get{
			return Mathf.Clamp01(canvasAspect / maxAspect);
		}
	}

	public float canvasAspect{
		get{
			return  parentCanvas.GetWidth() / parentCanvas.GetHeight();
		}
	}

	private Canvas parentCanvas;
	private void Awake()
	{
		parentCanvas = transform.root.GetComponent<Canvas>();		
	}
	
	private void Start()
	{		
		rectTransform = GetComponent<RectTransform>();
		rectTransform.sizeDelta = new Vector2(GetComponentInParent<Canvas>().GetWidth(), scaleheight * parentCanvas.GetHeight());	
	}

	private void On_OrientationChanged(DeviceOrientation orientation) => Start();

	private void OnEnable()
    {
		ScreenOrientationManager.On_OrientationChanged += On_OrientationChanged;
	}

    private void OnDisable()
    {
		ScreenOrientationManager.On_OrientationChanged -= On_OrientationChanged;
	}
}
