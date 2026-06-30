using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public SpriteRenderer[] spriteRenderers;

	public SpriteRenderer[] topSpriteRenderers;
	public SpriteRenderer[] wallSpriteRenderers;
	public SpriteRenderer[] faceSpriteRenderers;
	public SpriteRenderer[] IconSpriteRenderers;

    protected virtual void Start()
    {
        On_ColorPaletteChanged(LevelBuilder.instance.GetCurrentColors());
    }

    private void On_ColorPaletteChanged(ColorPalette colorPallete)
    {
		Color top = colorPallete.floor.SetVibrance(colorPallete.floor.GetVibrance()+0.10f);
		Color wall = colorPallete.floor.SetVibrance(colorPallete.floor.GetVibrance()-0.10f);
		Color face = colorPallete.floor;
		
        spriteRenderers[0].color = top; //Top
        spriteRenderers[1].color = wall; //Wall
        spriteRenderers[2].color = wall;
        spriteRenderers[3].color = face;

		foreach(SpriteRenderer sR in topSpriteRenderers)
			sR.color = top;
			
		foreach(SpriteRenderer sR in wallSpriteRenderers)
			sR.color = wall;
	
		foreach(SpriteRenderer sR in faceSpriteRenderers)
			sR.color = face;
    }

    protected virtual void OnEnable()
    {
        LevelBuilder.On_ColorPaletteChanged += On_ColorPaletteChanged;
    }

    protected virtual void Unsubscribe()
    {
        LevelBuilder.On_ColorPaletteChanged -= On_ColorPaletteChanged;
    }

    protected virtual void OnDisable()
    {
        Unsubscribe();
    }

    protected virtual void OnDestroy()
    {
        Unsubscribe();
    }
}
