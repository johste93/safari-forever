using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class EditorMenuWindow : MonoBehaviour
{   
    public Button garageButton;
    public RectTransform saveMessageRect;
    public TextMeshProUGUI savedMessage;
    public RectTransform window;
    private Tween tween;
    private Tween saveTween;

    public void Show()
    {
        garageButton.interactable = false;
        saveMessageRect.gameObject.SetActive(false);

        window.DOKill();
        window.anchoredPosition = new Vector3(0, -400f, 0);

        Canvas.ForceUpdateCanvases();

        gameObject.SetActive(true);
        window.gameObject.SetActive(true);
        tween = window.DOAnchorPosY(Screen.width <= Screen.height ? 100f : (800f - window.sizeDelta.y) / 2f, 0.3f).OnComplete(()=>
        {
            if(GlobalSingleton.mode == GameMode.Create)
            {
                if(!LevelBuilder.instance.IsLevelEmpty())
                {
                    GameMaster.instance.ResetLevel(true);
                   
                    saveMessageRect.gameObject.SetActive(true);
                    saveTween?.Kill();
                    saveTween = saveMessageRect.DOAnchorPosY(-10f, 0.3f).SetEase(Ease.OutBack).OnComplete(()=>
                    {
                        Garage.SaveWorkInProgressLevel();
                        garageButton.interactable = true;
                        //enable garage button.
                        
                    }).SetDelay(0.3f);
                }
                else
                {
                    garageButton.interactable = true;
                }
            }
        });

        //gameObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(window);
        LayoutRebuilder.ForceRebuildLayoutImmediate(window);
    }

    public void Close()
    {
        saveMessageRect.anchoredPosition = new Vector2(saveMessageRect.anchoredPosition.x, 60);
        saveMessageRect.gameObject.SetActive(false);

        gameObject.SetActive(false);
        DialogCanvas.instance.FadeOutbackground();
    }

    private void OnDestroy()
    {
        KillAllTweens();
    }

    private void KillAllTweens()
    {
        tween?.Kill();
        tween = null;

        saveTween?.Kill();
        saveTween = null;
    }
}
