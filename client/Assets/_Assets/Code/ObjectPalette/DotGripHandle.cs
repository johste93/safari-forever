using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class DotGripHandle : MonoBehaviour {

	private Image[] dots;

	private void Awake()
	{
		dots = GetComponentsInChildren<Image>(true);
	}

	public void TweenAlpha(float alpha, float duration)
	{
		foreach(Image dot in dots)
		{
			dot.DOFade(alpha, duration);
		}
	}

	private void OnDestroy()
	{
		KillAllTweens();
	}

	private void KillAllTweens()
	{
		foreach(Image dot in dots)
		{
			dot.DOKill();
		}
	}
}
