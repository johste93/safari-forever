using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LevelEditorMenuButton : MonoBehaviour
{
    public RectTransform anchorTransform;

    public void OnClick()
    {
        if (!anchorTransform.gameObject.activeInHierarchy)
            return;

        TouchInput.CancelTouch();

        Audio.Play(SFX.instance.ui.flatButton, Channel.UI).SetPitch(Random.Range(0.9f, 1.1f));
        anchorTransform.DOComplete();
        anchorTransform.DOPunchScale(Vector3.one * 0.1f, 0.3f);

        DialogCanvas.instance.ShowEditorMenu();
    }

    private void On_EnterPlayMode()
    {
        anchorTransform.gameObject.SetActive(false);
    }

    private void On_ExitPlayMode()
    {
        anchorTransform.gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        GameMaster.On_EnterPlayMode += On_EnterPlayMode;
        GameMaster.On_ExitPlayMode += On_ExitPlayMode;
    }

    private void Unsubscribe()
    {
        GameMaster.On_EnterPlayMode -= On_EnterPlayMode;
        GameMaster.On_ExitPlayMode -= On_ExitPlayMode;
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

}
