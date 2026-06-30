using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DeathEffect : MonoBehaviour
{	
	public static DeathEffect instance;

	public Image flash;
	public Image vignette;
	public Material grayscaleMaterial;

	//private Dictionary<SpriteRenderer, Color> spriteRenderers;
	private bool effectEnabled;

    private void Awake()
	{
		instance = this;
		
		//Make sure we dont start with a gray level if exiting during death effect.
		grayscaleMaterial.SetFloat("_EffectAmount", 0f);
	}
	
	public Tween DoEffect(System.Action callback = null)
	{
		MusicManager.DoFade(Globals.musicConstants.defaultVolume*0.25f, 0.1f);

		effectEnabled = true;

		

		if(!SaveManager.currentSave.noBrightFlashesMode)
		{
			vignette.gameObject.SetActive(true);
			Camera.main.backgroundColor = new Color(Camera.main.backgroundColor.grayscale, Camera.main.backgroundColor.grayscale, Camera.main.backgroundColor.grayscale);

			grayscaleMaterial.SetFloat("_EffectAmount", 1f);

			flash.color = Color.white;
			flash.gameObject.SetActive(true);
			return flash.DOFade(0f, 0.3f).OnComplete(()=>
			{
				MusicManager.DoFade(Globals.musicConstants.defaultVolume, 0.5f);

				if(callback != null)
					callback();
			});
		}
		else
		{
			return DOVirtual.DelayedCall(0.3f, ()=>
			{
				MusicManager.DoFade(Globals.musicConstants.defaultVolume, 0.5f);

				if(callback != null)
					callback();
			});
		}
	}

	public void UndoEffect()
	{
		grayscaleMaterial.SetFloat("_EffectAmount", 0f);

		MusicManager.DoFade(Globals.musicConstants.defaultVolume, 0f);

		flash.gameObject.SetActive(false);
		vignette.gameObject.SetActive(false);
		Camera.main.backgroundColor = LevelBuilder.instance.GetCurrentColors().main;

		effectEnabled = false;
	}

	private void On_SuspensionEvent(bool isSuspended)
    {
		if(!effectEnabled)
			return;
			
        flash.gameObject.SetActive(!isSuspended);
		vignette.gameObject.SetActive(!isSuspended);
    }

	private void OnEnable()
	{
		SuspensionManager.On_SuspensionEvent += On_SuspensionEvent;
	}

	private void Unsubscribe()
	{
		SuspensionManager.On_SuspensionEvent -= On_SuspensionEvent;
	}

	private void OnDisable()
	{
		Unsubscribe();
	}

	private void OnDestroy()
	{
		Unsubscribe();
		DestroyAllTweens();
	}

	private void DestroyAllTweens()
	{
		flash.DOKill();
	}
}
