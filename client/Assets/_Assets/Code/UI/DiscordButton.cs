using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DiscordButton : MonoBehaviour
{
	public RectTransform child;
    public void OnClick()
    {
		child.DOComplete();
        child.DOPunchScale(Vector3.one * 0.1f, 0.3f).OnComplete(()=>{
			Application.OpenURL("https://discord.gg/AVwnQgM");
		});
    }

	private void OnDestroy()
    {
		KillAllTweens();
    }

	private void KillAllTweens()
	{
		child.DOKill();
	}
	
}
