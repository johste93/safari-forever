using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BrowserButton : MonoBehaviour
{
	public MenuCanvas menuCanvas;

    public void OnClick()
    {
		SaveManager.currentSave.FetchOnlineProfile((profile)=>
		{
			if(profile == null)
				return;

			BrowserView browserView = (BrowserView)PopupCanvas.instance.browserView;

			browserView.browserCategory.SetCategory(BrowserTab.Featured, true);
			browserView.LoadLevelsForTab(BrowserTab.Featured);

			PopupCanvas.instance.background.DOColor(Camera.main.backgroundColor, 0.3f);
			PopupCanvas.instance.browserView.Show(()=>{
				PopupCanvas.instance.CloseCanvas();
			});
		});
    }
}
