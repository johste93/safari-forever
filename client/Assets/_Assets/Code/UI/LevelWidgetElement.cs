using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LevelWidgetElement : MonoBehaviour
{
    public Image thumbnailImage;
    public GameObject loading;

    private LevelInfoDTO levelInfo;
    private Tween thisTween;
    private Tween backgroundColorTween;

    private string userId;

    public void OnClick()
    {
        if(thisTween != null)
                thisTween.Complete();
            
        thisTween = transform.DOPunchScale(new Vector3(-0.2f, -0.3f, 0), 0.3f, 5);

        LevelAPI.Download(levelInfo.LevelId, (level) =>
        {
            if (level == null)
                return;

            //LevelViewer.instance.InspectLevel(level);
            
            Color backgroundColor = PopupCanvas.instance.background.color;
            PopupView currentView = PopupCanvas.instance.GetActivePopup();
            currentView.Close(false, ()=>{
                ((LevelInspector)PopupCanvas.instance.inspectView).Initalize(level, MenuLocation.Profile);
                PopupCanvas.instance.inspectView.Show(()=>{
                    
                    backgroundColorTween = PopupCanvas.instance.background.DOColor(backgroundColor, 0.3f);

                    currentView.Show(null);
                });
            });
        });
    }

    public void Initalize(LevelInfoDTO levelInfo, string userId)
    {
        this.userId = userId;
        loading.gameObject.SetActive(false);
        this.levelInfo = levelInfo;
    
        Sprite preview;
        if(this.levelInfo.MiniThumbnail == null)
        {
            preview = this.levelInfo.Thumbnail.ToSpriteFromGIF();
        }
        else
        {
            preview = this.levelInfo.MiniThumbnail.ToSpriteFromGIF();
        }

		if(preview == null)
		{
			Debug.LogError("Unable to Parse Image");
			return;
		}

		SetImage(preview);
    }

    public void SetImage(Sprite thumbnail)
    {
        thumbnailImage.enabled = true;
        thumbnailImage.sprite = thumbnail;
    }

    private void KillAllTweens()
    {
        thisTween?.Kill();
        backgroundColorTween?.Kill();
    }

    private void OnDestroy()
    {
        KillAllTweens();
    }	
}
