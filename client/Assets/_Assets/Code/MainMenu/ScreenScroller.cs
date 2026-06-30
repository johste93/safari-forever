using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;

public class ScreenScroller : MonoBehaviour
{
    public int startIndex = 0;
    private int current = 0;

    public CustomScrollRect horizontalScrollRect;
    public RectTransform[] screens;
	public RectTransform[] containers;

    public delegate void MenuEvent();
    public static MenuEvent OnScreenChange;

	private List<Tween> tweens = new List<Tween>();
    
    private float stepLength
	{
		get
		{
			return 1f / (float)(screens.Length-1);
		}
	}

    private Canvas rootCanvas;

    private void Awake()
    {     
        rootCanvas = transform.root.GetComponent<Canvas>();
		current = startIndex;
	}

    public void Next()
    {
		int previous = current;
        if(current < screens.Length-1)
            current++;

        GoToIndex(previous, current);

        OnScreenChange?.Invoke();
    }

    public void Previous()
    {
		int previous = current;

        if(current > 0)
            current--;

        GoToIndex(previous,current);

        OnScreenChange?.Invoke();
    }

    public void GoToIndex(int previous, int index, bool instant = false)
    {
		containers[index].gameObject.SetActive(true);
        SnapToNearest(stepLength * index, instant, ()=>{
			if(previous != current)
				containers[previous].gameObject.SetActive(false);
		});
    }

    private int SnapToNearest(float normalizedPos, bool instant, System.Action callback = null)
	{
		float nearestNormalizedPos = FindNearest(normalizedPos) ;
		horizontalScrollRect.DOKill();
		tweens.Add(DOTween.To(()=>horizontalScrollRect.horizontalNormalizedPosition, x=> horizontalScrollRect.horizontalNormalizedPosition = x, nearestNormalizedPos, instant ? 0f : 0.3f).SetEase(Ease.OutQuad).OnComplete(()=>{
			if(callback != null)
				callback();
		}));
		return Mathf.RoundToInt(nearestNormalizedPos / stepLength);
	}

    private float FindNearest(float t)
	{
		List<float> targetNormalizedPositions = new List<float>();
		for(float f = 0f; f < 1f; f += stepLength)
		{
			targetNormalizedPositions.Add(f);
		}
		targetNormalizedPositions.Add(1f);

		return targetNormalizedPositions.OrderBy(v => Mathf.Abs(v - t)).First();
	}

	private void OnDestroy()
	{
		KillAllTweens();
	}

	private void KillAllTweens()
	{
		horizontalScrollRect.DOKill();
		foreach(Tween t in tweens)
		{
			t.Kill();
		}
	}

	private void On_OrientationChanged(DeviceOrientation orientation)
    {
		UpdateSize(current);
	}

	private void UpdateSize(int target)
    {
		current = target;
		GoToIndex(0, current, true);

		for (int i = 0; i < screens.Length; i++)
		{
			screens[i].sizeDelta = new Vector2(rootCanvas.GetWidth(), screens[i].transform.parent.GetComponent<RectTransform>().rect.height);
			screens[i].anchoredPosition = new Vector3(rootCanvas.GetWidth() * i, 0, 0);
			LayoutRebuilder.ForceRebuildLayoutImmediate(screens[i]);
		}

		LayoutRebuilder.ForceRebuildLayoutImmediate(horizontalScrollRect.GetComponent<RectTransform>());
	}

	private void OnEnable()
	{
		UpdateSize(current);

		ScreenOrientationManager.On_OrientationChanged += On_OrientationChanged;
	}

	private void OnDisable()
	{
		ScreenOrientationManager.On_OrientationChanged -= On_OrientationChanged;
	}
}
