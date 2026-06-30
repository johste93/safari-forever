using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ColorButton : MonoBehaviour
{   
    public RectTransform rectTransform;

    public Image[] paletteImages;

    private void Start()
    {
        if(SaveManager.currentSave.monoChromatic)
            gameObject.SetActive(false);

        UpdateButton();
    }

    private void UpdateButton()
    {
        ColorPalette colors = LevelBuilder.instance.GetCurrentColors();
        paletteImages[0].color = colors.main;
        paletteImages[1].color = colors.floor;
        paletteImages[2].color = colors.wall;
        paletteImages[3].color = colors.pattern;
    }

    public void OnPointerDown()
    {
        Audio.Play(SFX.instance.ui.flatButton, Channel.UI).SetPitch(Random.Range(0.9f, 1.1f));
        rectTransform.DOComplete();
        rectTransform.DOPunchScale(Vector3.one * 0.1f, 0.3f);
    }

    public void OnClick()
    {
        LevelBuilder.instance.SetColorPalette(ColorGenerator.GetRandomColorPalette(false));   
    }

    public void OnLongPress()
    {
        ColorPalette colorPalette = LevelBuilder.instance.GetCurrentColors();

        ColorPicker.Constraints constraint = new ColorPicker.Constraints(){
            saturation = new Vector2(0.3f, 0.9f)
        };
        
        ColorPicker colorPicker = DialogCanvas.instance.ShowColorPicker(true, true, false,
        new Color[]{colorPalette.main, colorPalette.floor, colorPalette.wall, colorPalette.pattern}, new ColorPicker.Constraints[]{constraint,constraint,constraint,constraint},  (confirmed, pickedColors)=>{
            
            if(!confirmed)
                return;
            
            colorPalette.main = pickedColors[0];
            colorPalette.floor = pickedColors[1];
            colorPalette.wall = pickedColors[2];
            colorPalette.pattern = pickedColors[3];
            
            LevelBuilder.instance.SetColorPalette(colorPalette);
        });
    }

    private void On_LevelLoaded() => UpdateButton();

    private void On_ColorPaletteChanged(ColorPalette newPalette) => UpdateButton();

    private void OnEnable()
    {
        LevelSerializer.On_LevelLoaded += On_LevelLoaded;
        LevelBuilder.On_ColorPaletteChanged += On_ColorPaletteChanged;
    }

    private void Unsubscribe()
    {
        LevelSerializer.On_LevelLoaded -= On_LevelLoaded;
        LevelBuilder.On_ColorPaletteChanged -= On_ColorPaletteChanged;
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
