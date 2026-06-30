using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class CurrencyButton : Singleton<CurrencyButton>
{
    public TextMeshProUGUI amountTextMesh;

    public CanvasGroup canvasGroup;

    private Tween tween;
    private Tween punchTween;

    public void OnClick()
    {
        Audio.Play(SFX.instance.ui.flatButton, Channel.UI).SetPitch(Random.Range(0.9f, 1.1f));
        punchTween?.Complete();
        punchTween = transform.DOPunchScale(new Vector2(0.1f, 0.2f), 0.3f, 1).OnComplete(()=>
        {
            new Dialog(TranslationKey.Menu_Currency_Button_Title, TranslationKey.Menu_Currency_Button_Body)
                .AddNeutralButton(TranslationKey.Generic_Ok, null, true)
                .Show();
        });
    }

    public void TryShow(int coins)
    {
        UpdateCoins(coins);
        tween = canvasGroup.DOFade(1f, 0.5f);
        canvasGroup.interactable = true;
    }

    public void UpdateCoins(int coins)
    {
        amountTextMesh.text = coins.ToString();
    }

    private void OnEnable()
    {
        if(canvasGroup.interactable)
        {
            SaveManager.currentSave.FetchOnlineProfile((profile)=>
            {
                if(profile == null)
                    return;  
                
                UpdateCoins(profile.coins);
            });
        }
    }
    
    private void OnDestroy()
    {
		KillAllTweens();
    }

	private void KillAllTweens()
	{	
        tween?.Kill();
        punchTween?.Kill();
	}
}
