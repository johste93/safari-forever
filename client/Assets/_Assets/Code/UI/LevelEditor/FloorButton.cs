using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class FloorButton : MonoBehaviour
{
    public Image icon;

    public TextMeshProUGUI indexTextMesh;
    public RectTransform rectTransform;  

    private int index = 0;

    private void Start()
    {
        index = LevelBuilder.instance.GetFloorSpriteIndex();
        UpdateButton();
    }

    public void OnClick()
    {
        Audio.Play(SFX.instance.ui.flatButton, Channel.UI).SetPitch(Random.Range(0.9f, 1.1f));
        index++;
        if(index >= LevelBuilder.instance.floorSprites.Length)
            index = 0;

        UpdateButton();

        rectTransform.DOComplete();
        rectTransform.DOPunchScale(Vector3.one * 0.1f, 0.3f);

        LevelBuilder.instance.SetFloorSprite(index);
    }

    private void UpdateButton()
    {
        icon.sprite = LevelBuilder.instance.floorSprites[index];
        indexTextMesh.text = (index+1).ToString();
    }

    private void On_LevelLoaded()
    {
        index = LevelBuilder.instance.GetFloorSpriteIndex();
        UpdateButton();
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
