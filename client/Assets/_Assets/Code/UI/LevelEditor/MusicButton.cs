using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using TMPro;

public class MusicButton : MonoBehaviour
{
    public GameObject cross;
    public RectTransform rectTransform;
    public TextMeshProUGUI indexTextMesh;

    private int index;
    
    private void Start()
    {
        index = LevelBuilder.instance.GetMusicIndex();
        //Debug.Log(index);
        UpdateTextMesh();
    }


    private void UpdateTextMesh()
    {
        if (index > 0)
            indexTextMesh.text = (index - 1).ToString();
        else
            indexTextMesh.text = "-";

        cross.SetActive(index == 0);
    }

    public void OnClick()
    {
        Audio.Play(SFX.instance.ui.flatButton, Channel.UI).SetPitch(UnityEngine.Random.Range(0.9f, 1.1f));
        rectTransform.DOComplete();
        rectTransform.DOPunchScale(Vector3.one * 0.1f, 0.3f);

        index++;
        if(index >= Enum.GetValues(typeof(Music)).Length)
            index = 0;

        if (index == 1)
            index++;

        Audio.Play(SFX.instance.ui.musicSwitch, Channel.UI);
        LevelBuilder.instance.SetMusic((Music)index);
        LevelBuilder.instance.PlayMusic(false);

        UpdateTextMesh();
    }

    private void On_LevelLoaded()
    {
        index = LevelBuilder.instance.GetMusicIndex();
        UpdateTextMesh();
    }

    private void OnEnable()
    {
        LevelSerializer.On_LevelLoaded += On_LevelLoaded;
    }

    private void Unsubscribe()
    {
        LevelSerializer.On_LevelLoaded -= On_LevelLoaded;
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();
		KillAllTweens();
    }

	private void KillAllTweens()
	{
		rectTransform.DOKill();
	}
}
