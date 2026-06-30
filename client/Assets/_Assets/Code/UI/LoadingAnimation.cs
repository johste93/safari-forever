using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LoadingAnimation : MonoBehaviour
{
    public List<Transform> blocks;
    public List<Tween> tweens = new List<Tween>();

    private const float speed = 0.15f;
	private Tween delay;

    private void OnEnable()
    {
        Reset();
        PlayAnimation();
    }

    private void PlayAnimation()
    {
        for(int i = 0; i < blocks.Count; i++)
        {
            tweens.Add(blocks[i].DOScale(1f, speed).SetEase(Ease.OutBack).SetDelay(i * speed));
            tweens.Add(blocks[i].DOScale(0f, speed).SetDelay((blocks.Count * speed) + (i * speed)));
        }

        delay = DOVirtual.DelayedCall(blocks.Count * speed * 2, ()=>{
            PlayAnimation();
        });
    }

    private void Reset()
    {
        foreach(Transform block in blocks)
        {
			if(block != null)
           		block.localScale = Vector3.zero;
        }
    }

    private void OnDisable()
    {
        KillAllTweens();
    }

	private void OnDestroy()
	{
		KillAllTweens();
	}

	private void KillAllTweens()
	{
		delay?.Kill();

		foreach(Tween t in tweens)
            t?.Kill();

        tweens = new List<Tween>();
	}
}
