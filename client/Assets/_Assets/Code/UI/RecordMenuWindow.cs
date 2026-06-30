using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RecordMenuWindow : MonoBehaviour
{
    public RectTransform window;
    private Tween tween;
    private System.Action onComplete;

    public void Show(System.Action onComplete = null)
    {
        //SuspensionManager.Suspend(true);

        this.onComplete = onComplete;

        window.DOKill();
        window.anchoredPosition = new Vector3(0, -400f, 0);

        Canvas.ForceUpdateCanvases();

        window.gameObject.SetActive(true);
        tween = window.DOAnchorPosY(Screen.width <= Screen.height ? 100f : (800f - window.sizeDelta.y) / 2f, 0.3f);
    }

    public void Save(int quality)
    {
        GifQualitySettings settings = Globals.gifConstants.GetQualitySettings((GifQuality) quality);
        ProGifManager.Instance.SetEncoderQuality(settings.processingQuality);

        GifRecorder.instance.SaveRecording(On_GifSaved);
    }

    private void On_GifSaved(bool saveSuccess, string path)
    {
        if(!saveSuccess)
        {
            Close();
            
            if(Application.isMobilePlatform)
            {
                if(!string.IsNullOrWhiteSpace(path))
                {  
                    if(System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                        Debug.Log("Deleting gif from internal memory");
                    }
                }
            }

            return;
        }

        GifRecorder.instance.SuccessDialog(path, (s, p)=>{
            DialogCanvas.instance.shareRecordingMenuWindow.pathToSavedGif = path;
            DialogCanvas.instance.ShowShareRecordingMenuWindow(()=>{
                
                

                onComplete?.Invoke();
            });
        });
    }

    public void OnClickCloseButton()
    {
        //Delete recording.
        GifRecorder.instance.Reset();

        Close();
    }

    public void Close()
    {
        gameObject.SetActive(false);
        DialogCanvas.instance.FadeOutbackground();

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
