using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ReadySign : MonoBehaviour
{   
    public List<Tween> tweens = new List<Tween>();
	private Tween fadeOut;

    public void ShrinkSign()
    {
        transform.DOKill();
        tweens.Add(transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InQuad));
    }

    public void EnlargeSign()
    {
        transform.DOKill();
        tweens.Add(transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack));

		AudioPlayer aP = Audio.Play(SFX.instance.ui.ready, Channel.Game);
		fadeOut = aP.source.audioSource.DOFade(0f, 0.5f).SetEase(Ease.OutSine).SetDelay(0.6f);
    }

	private void KillAllTweens()
	{
		transform.DOKill();

        foreach(Tween t in tweens)
            t.Kill();

		tweens = null;

		fadeOut?.Kill();
	}

    private void OnDestroy()
    {
		KillAllTweens();
    }
}
