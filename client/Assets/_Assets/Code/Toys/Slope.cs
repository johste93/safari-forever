using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slope : MonoBehaviour
{
    public SpriteRenderer[] spriteRenderers;
    public LayerMask whatIsBlocking;

    private LevelEntity entity;

    public int slopeDirection{
        get{
            return (Direction4)entity.GetSerializableData().gizmoDirection == Direction4.Left ? -1 : 1;
        }
    }

    public GameObject slope;
    public GameObject block;
    public GameObject fakeSlope;

    public EdgeCollider2D bottomCollider;
    public BoxCollider2D backCollider;

    private bool isSlope
    {
        get{return offset.x == offset.y;}
    }
    private Vector2Int offset;

    private void Awake()
    {
        entity = GetComponentInParent<LevelEntity>();
        Direction4 direction = entity.GetSerializableData().gizmoDirection.HasValue ? (Direction4)entity.GetSerializableData().gizmoDirection : Direction4.Right;
        slope.transform.localScale = new Vector3(direction == Direction4.Left ? -1 : 1, 1, 1);
        fakeSlope.transform.localScale = slope.transform.localScale;

        float width = entity.GetWidth();
        float height = entity.GetHeight();

        
        Vector2 offsetFromCorner = Vector2.zero;
        if(direction == Direction4.Right)
            offsetFromCorner = entity.GetSerializableData().bottomLeft - (Vector2)transform.position + new Vector2(0.5f,0.5f);
        else
            offsetFromCorner = new Vector2(entity.GetSerializableData().topRight.x, entity.GetSerializableData().bottomLeft.y) - (Vector2)transform.position;
        
        offset = new Vector2Int((int)Mathf.Abs(offsetFromCorner.x) , (int)Mathf.Abs(offsetFromCorner.y));

        slope.SetActive(isSlope);
        block.SetActive(offset.y < offset.x);

        if(isSlope)
        {
            bottomCollider.enabled = offset.y == 0;
            backCollider.enabled = offset.y == height-1;
        }
    }

    private void Start()
    {
        On_ColorPaletteChanged(LevelBuilder.instance.GetCurrentColors());
        UpdateSlope();      
    }

    private void UpdateSlope()
    {
        if(!isSlope)
            return;

        Collider2D collider = Physics2D.OverlapBox(transform.position + Vector3.up, Vector2.one*0.45f, 0, whatIsBlocking);

        slope.SetActive(collider == null);
        fakeSlope.SetActive(collider != null);
    }
    
    private void On_EntityMoved(LevelEntity entity)
    {
        if(this.entity == entity)
            return;

        UpdateSlope(); 
    }
    
    private void On_EntityChangedSize(LevelEntity entity)
    {
        if(this.entity == entity)
            return;

        UpdateSlope(); 
    }

    private void On_ColorPaletteChanged(ColorPalette colorPallete)
    {
        spriteRenderers[0].color = colorPallete.floor.SetVibrance(colorPallete.floor.GetVibrance()+0.10f); //Top
        spriteRenderers[1].color = colorPallete.floor.SetVibrance(colorPallete.floor.GetVibrance()-0.10f); //Wall
        spriteRenderers[2].color = colorPallete.floor;

        spriteRenderers[3].color = spriteRenderers[0].color; //Top
        spriteRenderers[4].color = spriteRenderers[1].color; //Wall
        spriteRenderers[5].color = spriteRenderers[1].color;
        spriteRenderers[6].color = colorPallete.floor;

        spriteRenderers[7].color = spriteRenderers[0].color;//Top
        spriteRenderers[8].color = spriteRenderers[1].color;//Wall
        spriteRenderers[9].color = spriteRenderers[2].color;
    }

    private void OnEnable()
    {
        LevelBuilder.On_ColorPaletteChanged += On_ColorPaletteChanged;

        LevelEntity.On_EntityMoved += On_EntityMoved;
        LevelEntity.On_EntityChangedSize += On_EntityChangedSize;
    }

    private void Unsubscribe()
    {
        LevelBuilder.On_ColorPaletteChanged -= On_ColorPaletteChanged;

        LevelEntity.On_EntityMoved -= On_EntityMoved;
        LevelEntity.On_EntityChangedSize -= On_EntityChangedSize;
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
