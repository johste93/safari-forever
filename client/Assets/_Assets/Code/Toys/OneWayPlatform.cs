using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{
    public EdgeCollider2D edgeCollider2D;
    public LayerMask whatIsGround;
    public SpriteRenderer[] spriteRenderers;

    protected virtual void Start()
    {
        On_ColorPaletteChanged(LevelBuilder.instance.GetCurrentColors());
    }

    private void On_ColorPaletteChanged(ColorPalette colorPallete)
    {
        spriteRenderers[0].color = colorPallete.floor.SetVibrance(colorPallete.floor.GetVibrance()+0.10f); //Top
        spriteRenderers[1].color = colorPallete.floor.SetVibrance(colorPallete.floor.GetVibrance()-0.10f); //Wall
        spriteRenderers[2].color = colorPallete.floor; //Face
    }

    private void CheckIfBlocked()
    {
        bool enableCollider = true;

        RaycastHit2D[] allHits;
        allHits = Physics2D.RaycastAll(transform.position, Vector2.up, 0.75f, whatIsGround);
        foreach (var hit in allHits)
        {
            if (hit.collider.CompareTag("OneWayPlatform") || hit.collider.CompareTag("OneWayGateNorth"))
                continue;
            
            if(hit.distance == 0)
                continue;

            enableCollider = false;
        }

        edgeCollider2D.enabled = enableCollider;
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
