using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShadow : MonoBehaviour
{
    public Image background;

    private void On_ColorPaletteChanged(ColorPalette newPalette)
	{
		float vibrance = newPalette.main.GetVibrance();
        vibrance *= 0.75f;
        Color c = newPalette.main.SetVibrance(vibrance);
		background.color = c;
	}	

	private void OnEnable()
	{        
        if(LevelBuilder.instance.GetCurrentColors() != null)
            On_ColorPaletteChanged(LevelBuilder.instance.GetCurrentColors());
            
		LevelBuilder.On_ColorPaletteChanged += On_ColorPaletteChanged;
	}

    private void Unsubscribe()
	{
		LevelBuilder.On_ColorPaletteChanged -= On_ColorPaletteChanged;
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
