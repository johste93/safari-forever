using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SafariForever.Toolbar;

public class EditorCanvas : Singleton<EditorCanvas>
{
    
    public CanvasGroup topMenuCanvasGroup;
    public CanvasGroup toolbarCanvasGroup;

    public Toolbar toolbar;
    
    private void On_EnterPlayMode()
    {
        toolbarCanvasGroup.alpha = 0;
        toolbarCanvasGroup.blocksRaycasts = false;

        topMenuCanvasGroup.alpha = 0;
        topMenuCanvasGroup.blocksRaycasts = false;
    }

    private void On_ExitPlayMode()
    {
        toolbarCanvasGroup.alpha = 1;
        toolbarCanvasGroup.blocksRaycasts = true;

        topMenuCanvasGroup.alpha = 1;
        topMenuCanvasGroup.blocksRaycasts = true;
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
