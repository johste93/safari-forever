using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FeatureTab : MonoBehaviour
{
    public RectTransform discordAd;
    public RectTransform discordAdContainer;
    public RectTransform scrollRectTransform;

    private void Awake()
    {
        if(!SaveManager.discordAdHidden)
        {
            ShowDiscordAd();
        }
        else
        {
            HideDiscordAd();
        }
    }

    public void ShowDiscordAd()
    {
        discordAd.gameObject.SetActive(true);
        scrollRectTransform.SetTop(107.5f);
    }

    public void HideDiscordAd()
    {
        discordAd.gameObject.SetActive(false);
        scrollRectTransform.SetTop(0);
        SaveManager.discordAdHidden = true;
    }

    public void OnClick()
    {
		discordAdContainer.DOComplete();
        discordAdContainer.DOPunchScale(Vector3.one * 0.1f, 0.3f).OnComplete(()=>{
			Application.OpenURL("https://discord.gg/bgykYaq");
            SaveManager.discordAdHidden = true;
		});
    }

    private void OnDisable()
    {
        KillAllTweens();

        if(SaveManager.discordAdHidden)
            HideDiscordAd();
    }

	private void OnDestroy()
    {
		KillAllTweens();
    }

	private void KillAllTweens()
	{
		discordAdContainer.DOKill();
	}
}
