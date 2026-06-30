using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayMenuButton : MonoBehaviour
{
    public RectTransform anchorTransform;
    public Sprite homeSprite;
    public Sprite menuSprite;

    public Image iconImage;

    private bool showPlayMenu;

    public void OnClick()
    {
        if (!anchorTransform.gameObject.activeInHierarchy)
            return;

        if(TransitionHole.instance.IsClosed())
            return;

        Audio.Play(SFX.instance.ui.flatButton, Channel.UI).SetPitch(Random.Range(0.9f, 1.1f));
        anchorTransform.DOComplete();
        anchorTransform.DOPunchScale(Vector3.one * 0.1f, 0.3f);

        if(GameMaster.instance.GetCurrentMode() == GameMode.Campaign)
        {
            SceneLoader.Load(SafariScene.Menu);
        }
        else
        {
            showPlayMenu = true;
        }
    }

    private void FixedUpdate()
    {
        if (!showPlayMenu)
            return;

        showPlayMenu = false;
        DialogCanvas.instance.ShowPlayMenu();
    }

    private void OnEnable()
    {
#if UNITY_WEBGL
		gameObject.SetActive(false);
#else
        anchorTransform.gameObject.SetActive(GameMaster.instance.GetCurrentMode() != GameMode.Create);

        iconImage.sprite = GameMaster.instance.GetCurrentMode() == GameMode.Campaign ? homeSprite : menuSprite;
#endif
    }


    private void OnDestroy()
	{
		KillAllTweens();
	}

	private void KillAllTweens()
	{
		anchorTransform.DOKill();
	}
}
