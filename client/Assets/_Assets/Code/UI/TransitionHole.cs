using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TransitionHole : Singleton<TransitionHole>
{   
    public Camera transitionCamera;
    public Transform childTransform;
    public MeshRenderer meshRenderer;
    private Material holeMaterial;

    private Tween currentTween;
    private Transform target;

    private const float duration = 0.5f;

    private void Awake()
    {
        holeMaterial = meshRenderer.material;
    }

    public void Close(Transform target)
    {
        this.target = target;

        Close();
    }

    public void Close()
    {
        if(currentTween != null)
            currentTween.Kill();

        float t = 1f;        
        currentTween = DOTween.To(()=> t, x=> t = x, -0.05f, duration).SetUpdate(true).SetEase(Ease.OutQuad).OnUpdate(()=>
        {
            holeMaterial.SetFloat("_Cutoff", t);

            if(target != null)
            {
                Vector3 viewportPosition = Camera.main.WorldToViewportPoint(target.position);
                childTransform.position = transitionCamera.ViewportToWorldPoint(viewportPosition);
            }
        });
    }

    public void Open(Transform target, System.Action onComplete = null)
    {
        this.target = target;

        Open(onComplete);
    }

    public void Open(System.Action onComplete = null)
    {
        if(currentTween != null)
            currentTween.Kill();

        float t = -0.05f;
        currentTween = DOTween.To(()=> t, x=> t = x, 2f, duration*2).SetUpdate(true).SetEase(Ease.InQuad).OnUpdate(()=>
        {
            holeMaterial.SetFloat("_Cutoff", t);

            if(target != null)
            {
                Vector3 viewportPosition = Camera.main.WorldToViewportPoint(target.position);
                childTransform.position = transitionCamera.ViewportToWorldPoint(viewportPosition);
            }

			if(onComplete != null)
			{
				if(!IsClosed())
				{
					onComplete();
					onComplete = null;
				}
			}
        });
    }

    public bool IsClosed()
    {
        return holeMaterial.GetFloat("_Cutoff") < 0.3f;
    }

	private void OnDestroy()
	{
		KillAllTweens();
	}

	private void KillAllTweens()
	{
		if(currentTween != null)
            currentTween.Kill();
	}
}
