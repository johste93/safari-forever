using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RoomViewer : MonoBehaviour
{
    public CanvasGroup canvasGroup;

    public RectTransform border;
    public RectTransform[] rooms;
    public Image[] borders;
    public Image[] icons;
    public Color deselectedColor;

    private Tween delay;

    private void On_ChangedRoom()
    {
        canvasGroup.DOKill();
        canvasGroup.DOFade(1f, 0.3f);

        if(delay != null)
            delay.Kill(true);
            
        delay = DOVirtual.DelayedCall(1.25f, ()=>
        {
            canvasGroup.DOKill();
            canvasGroup.DOFade(0f, 0.2f);
            delay = null;
        }, true);

        int index = LevelBuilder.instance.GetCurrentRoomIndex();
        //border.DOKill();
        //border.DOMove(rooms[index].position, 0.3f).SetEase(Ease.OutQuad);

        for(int i = 0; i < borders.Length; i++)
        {
            rooms[i].DOComplete();

            if(i == index)
                rooms[i].DOPunchScale(Vector3.one * 0.1f, 0.3f).SetEase(Ease.OutQuad);

            borders[i].color = i == index ? Color.white : deselectedColor;
            icons[i].color = i == index ? Color.white : Color.white.SetAlpha(0.5f);
        }
    }

    private void On_EnterPlayMode()
    {
        if(delay != null)
            delay.Kill(true);

        canvasGroup.DOKill();
        canvasGroup.alpha = 0f;
        delay = null;
    }

    private void OnEnable()
    {
        LevelBuilder.On_ChangedRoom += On_ChangedRoom;
        GameMaster.On_EnterPlayMode += On_EnterPlayMode;
    }

    private void Unsubscribe()
    {
        LevelBuilder.On_ChangedRoom -= On_ChangedRoom;
        GameMaster.On_EnterPlayMode -= On_EnterPlayMode;
    }
    
    private void OnDestroy()
    {
        Unsubscribe();
    }

    private void OnDisable()
    {
        Unsubscribe();
		KillAllTweens();
    }

	private void KillAllTweens()
	{
		if(delay != null)
            delay.Kill(true);

        canvasGroup.DOKill();

		for(int i = 0; i < borders.Length; i++)
            rooms[i].DOKill();
	}
}
