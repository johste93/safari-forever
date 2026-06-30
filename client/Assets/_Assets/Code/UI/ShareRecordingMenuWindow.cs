using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using MandarinDuck.NativeShareDialog;

public class ShareRecordingMenuWindow : MonoBehaviour
{
    public RectTransform window;
    public GameObject giphyButton;
    public GameObject shareButton;

    [HideInInspector]public string pathToSavedGif;

    private Tween tween;
    private System.Action onComplete;

    private string shareUrl;

    public void Show(System.Action onComplete = null)
    {
        //SuspensionManager.Suspend(true);

        this.onComplete = onComplete;

        window.DOKill();
        window.anchoredPosition = new Vector3(0, -400f, 0);

        Canvas.ForceUpdateCanvases();

        ShowShareButton(false);
        ShowGiphyButton(true);

        window.gameObject.SetActive(true);
        tween = window.DOAnchorPosY(Screen.width <= Screen.height ? 100f : (800f - window.sizeDelta.y) / 2f, 0.3f);
    }

    public void UploadToGiphy()
    {
        byte[] bytes = System.IO.File.ReadAllBytes(pathToSavedGif);

        bool uploadComplete = false;

        DialogCanvas.instance.ShowLoading();
        GifAPI.Upload(bytes, (uploadSuccess, response)=>
        {    
            DialogCanvas.instance.HideLoading();
            if(!uploadSuccess)
            {
                Close();
                return;
            }

            shareUrl = response.Url;

            ChangeButtonsAnimation(()=>{
                Share();
            });

            ShowShareButton(true);
            ShowGiphyButton(false);
        });
    }

    private void ShowShareButton(bool show)
    {
        shareButton.SetActive(show);
    }

    private void ShowGiphyButton(bool show)
    {
        giphyButton.SetActive(show);
    }

    public void Share()
    {
        if(Application.isEditor)
            shareUrl.CopyToClipboard();

        SocialManager.Share( shareUrl );
    }

    public void OnClickCloseButton()
    {
        //Delete recording.
        GifRecorder.instance.Reset();

        Close();
    }

    private void ChangeButtonsAnimation(System.Action onComplete)
    {
        window.gameObject.SetActive(false);

        window.DOKill();
        window.anchoredPosition = new Vector3(0, -400f, 0);
        Canvas.ForceUpdateCanvases();

        window.gameObject.SetActive(true);
        tween = window.DOAnchorPosY(100f, 0.3f).OnComplete(()=>{
            onComplete?.Invoke();
        });
    }

    public void Close()
    {
        //SuspensionManager.Suspend(false);

        gameObject.SetActive(false);
        DialogCanvas.instance.FadeOutbackground();

        if(Application.isMobilePlatform)
        {
            if(!string.IsNullOrWhiteSpace(pathToSavedGif))
            {    
                if(System.IO.File.Exists(pathToSavedGif))
                {
                    System.IO.File.Delete(pathToSavedGif);
                    Debug.Log("Deleting gif from internal memory");
                }
            }
        }

        onComplete?.Invoke();
    }

    private void OnDestroy()
    {
        KillAllTweens();
    }

    private void KillAllTweens()
    {
        if(tween != null)
        {
            tween.Kill();
            tween = null;
        }
    }
}
