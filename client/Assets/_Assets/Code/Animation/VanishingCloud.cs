using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class VanishingCloud : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    private const float lifetime = 0.3f;

    public void OnEnable()
    {
        transform.eulerAngles = new Vector3(0, 0, Random.Range(0,360));
        transform.DOScale(transform.localScale * 1.1f, lifetime).SetEase(Ease.OutQuad);
        transform.DORotate(transform.eulerAngles + new Vector3(0, 0, 30f), lifetime).SetEase(Ease.OutQuad);        
        spriteRenderer.DOFade(0f, lifetime).SetEase(Ease.OutQuad);

        Destroy(gameObject, lifetime);
    }	

	private void KillAllTweens()
	{
		transform.DOKill();
		spriteRenderer.DOKill();
	}

	private void OnDestroy()
	{
		KillAllTweens();
	}	
}
