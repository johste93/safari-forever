using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SearchButton : MonoBehaviour
{
	public RectTransform child;
    public void OnClick()
    {
		child.DOComplete();
        child.DOPunchScale(Vector3.one * 0.1f, 0.3f).OnComplete(()=>{
			
			PopupView currentView = PopupCanvas.instance.GetActivePopup();
			currentView.Close(false, ()=>
			{
				PopupCanvas.instance.searchView.Show(()=>{
					currentView.Show(null);
				});
			});
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
