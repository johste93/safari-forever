using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ProfileViewer : Singleton<ProfileViewer>
{
    public RectTransform rectTransform;
    public GameObject container;
    public Image background;
    
    public PopupView profileView;
    public PopupView notificationView; 
    public PopupView levelsView; 

    public Canvas canvas;

    private List<Tween> tweens = new List<Tween>();

    public void ShowViewer(bool instant = false, System.Action OnComplete = null)
    {
        if(container.activeInHierarchy)
            return;

        background.color = Camera.main.backgroundColor;
        container.SetActive(true);

        background.DOKill();
        background.color = background.color.SetAlpha(0f);
        tweens.Add(background.DOFade(1f, instant ? 0f : 0.3f ).OnComplete(()=>{
            OnComplete?.Invoke();
        }));
    }

    public void CloseViewer(bool instant = false)
    {
        background.DOKill();
        tweens.Add(background.DOFade(0f, instant ? 0f : 0.3f ).OnComplete(()=>{
            container.SetActive(false);
        }));
    }

    private void OnDestroy()
	{
		KillAllTweens();
	}

	private void KillAllTweens()
	{
		foreach (Tween t in tweens)
        {
            t.Kill();
        }
        tweens = new List<Tween>();
	}

    public static void SetSortingOrder(int index)
    {
        instance.canvas.sortingOrder = index;
        instance.canvas.worldCamera.depth = index;
    }
}
