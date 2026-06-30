using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BrokenBlock : MonoBehaviour, ISuspendable
{
	public Transform topLeft, topRight, bottomLeft, bottomRight;

	private List<Tween> tweens = new List<Tween>();

	private float jumpHeight = 3;
	private float animationDuration = 0.3f;
    public void PlayAnimation()
	{
		tweens = new List<Tween>();

		topLeft.DOKill();
		tweens.Add(topLeft.DOLocalJump(new Vector3(-1 + Random.Range(-3, 0f), -2 + Random.Range(-2, 0f), 0), jumpHeight + Random.Range(-2, 2f), 1, animationDuration).SetEase(Ease.Linear).OnComplete(()=>{
			topLeft.gameObject.SetActive(false);
		}));
		tweens.Add(topLeft.DOLocalRotate(Vector3.forward * -45f, animationDuration));
		
		topRight.DOKill();
		tweens.Add(topRight.DOLocalJump(new Vector3(1 + Random.Range(0, 3f), -2 + Random.Range(-2, 0f), 0), jumpHeight + Random.Range(-2, 2f), 1, animationDuration).SetEase(Ease.Linear).OnComplete(()=>{
			topRight.gameObject.SetActive(false);
		}));
		tweens.Add(topRight.DOLocalRotate(Vector3.forward * 45f, animationDuration));

		bottomLeft.DOKill();
		tweens.Add(bottomLeft.DOLocalJump(new Vector3(-1 + Random.Range(-3, 0f), -3 + Random.Range(-2, 0f), 0), jumpHeight + Random.Range(-2, 2f), 1, animationDuration).SetEase(Ease.Linear).OnComplete(()=>{
			bottomLeft.gameObject.SetActive(false);
		}));
		tweens.Add(bottomLeft.DOLocalRotate(Vector3.forward * -45f, animationDuration));

		bottomRight.DOKill();
		tweens.Add(bottomRight.DOLocalJump(new Vector3(1 + Random.Range(0, 3f), -3 + Random.Range(-2, 0f), 0), jumpHeight + Random.Range(-2, 2f), 1, animationDuration).SetEase(Ease.Linear).OnComplete(()=>{
			bottomRight.gameObject.SetActive(false);
		}));
		tweens.Add(bottomRight.DOLocalRotate(Vector3.forward * 45f, animationDuration));
	}

	private void Reset()
	{
		topLeft.transform.localPosition = new Vector3(-0.25f, 0.25f, 0f);
		topRight.transform.localPosition = new Vector3(0.25f, 0.25f, 0f);
		bottomLeft.transform.localPosition = new Vector3(-0.25f, -0.25f, 0f);
		bottomRight.transform.localPosition = new Vector3(0.25f, -0.25f, 0f);

		topLeft.eulerAngles = topRight.eulerAngles = bottomLeft.eulerAngles = bottomRight.eulerAngles = Vector3.zero;

		topLeft.gameObject.SetActive(true);
		topRight.gameObject.SetActive(true);
		bottomLeft.gameObject.SetActive(true);
		bottomRight.gameObject.SetActive(true);
	
		foreach(Tween t in tweens)
		{
			t?.Kill();
		}
	}

	private void OnEnable()
	{
		PlayAnimation();

		SuspensionManager.On_SuspensionEvent += On_SuspensionEvent;
	}

	private void Unsubscribe()
	{
		SuspensionManager.On_SuspensionEvent -= On_SuspensionEvent;
	}

	private void OnDisable()
	{
		Reset();
		Unsubscribe();
	}

	private void OnDestroy()
	{
		KillAllTweens();
		Unsubscribe();
	}

	private void KillAllTweens()
	{
		topRight.DOKill();
		topLeft.DOKill();
		bottomRight.DOKill();
		bottomLeft.DOKill();
	}

	public void On_SuspensionEvent(bool suspend)
    {
        Suspend(suspend);
    }

	public void Suspend(bool suspend)
    {
        foreach(Tween t in tweens)
		{
			if(suspend)
				t?.Pause();
			else
				t?.Play();
		}
    }  
}
