using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MandarinDuck.NativeShareDialog;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DG.Tweening;
using UnityEngine.UI;

public class LevelPublisher : PopupView
{
    public NameSelector nameSelector;    
    public ShareCodeButton shareCodeButton;

    public Button publishButton;
    public GameObject shareButton;
    public GameObject cancelButton;
    public GameObject backButton;

    public LevelPreview levelPreview;

    public void Initalize()
    {
        levelPreview.Reset();
        levelPreview.SetName("");
        publishButton.interactable = true;
    }

    public override void Show(System.Action onExit, bool instant = false, System.Action onComplete = null)
    {
        PopupCanvas.instance.background.DOKill();
        PopupCanvas.instance.background.DOColor(LevelBuilder.instance.GetCurrentColors().main, 0.3f);
        base.Show(onExit, instant, onComplete);
    }
   
    public void OnClickCancel()
    {
        base.Exit();
    }

    public void OnClickBack()
    {
        DialogCanvas.instance.ShowReviewWindow(null);

        base.Exit();
    }

    public void OnClickPublish()
    {
        if(Mathf.Approximately((float)Stopwatch.instance.GetTotal(), 0f))
        {
            Debug.LogError("Level is to short!");
            return;
        }

        if(LevelBuilder.instance.GetCost() > Globals.gameConstants.blockBudget)
        {
            Debug.LogError("Level is above block limit!");
            return;
        }

        if(!publishButton.interactable)
            return;

        publishButton.interactable = false;

        string json = LevelSerializer.Save(true);
        JObject levelJObject = JObject.Parse(json);
        byte[] largeThumbnail = ThumbnailCamera.instance.GetLargeThumbnail();
        byte[] smallThumbnail = ThumbnailCamera.instance.GetSmallThumbnail();

        LevelAPI.UploadLevel(nameSelector.levelName, levelJObject, largeThumbnail, smallThumbnail,
        ()=>{
            nameSelector.Editable(false);
            DialogCanvas.instance.ShowLoading();
        },
        (uploadSuccess, levelId)=>
        {
            if(!uploadSuccess)
            {
                DialogCanvas.instance.HideLoading();
                nameSelector.Editable(true);
                return;
            }

            Garage.DeleteWorkInProgressLevel();
            LevelBuilder.instance.CreateNewLevel();

            LevelAPI.VerifyUpload(levelId, (verificationSuccess, shareCode)=>
            {
                DialogCanvas.instance.HideLoading();
                if(!verificationSuccess)
                {
                    Debug.LogError("Unable to verify Level!");
                    base.Exit();
                    return;
                }

                shareCodeButton.shareUrl = shareCode;

                if(Application.isEditor)
                    shareCode.CopyToClipboard();

                SocialManager.Share( shareCode );

                publishButton.gameObject.SetActive(false);
                cancelButton.SetActive(false);

                shareButton.SetActive(true);
                backButton.SetActive(true);    
            });
        });
    }

    private void OnEnable()
    {
        shareButton.SetActive(false);
        backButton.SetActive(false);

        publishButton.gameObject.SetActive(true);
        cancelButton.SetActive(true);

        nameSelector.Editable(true);
    }
}
