using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Tile : MonoBehaviour
{
    public SpriteRenderer background;
    public SpriteRenderer pattern;
    public SpriteRenderer floor;

	public SpriteRenderer top_wall;
	public SpriteRenderer right_wall;
	public SpriteRenderer bottom_wall;
	public SpriteRenderer left_wall;

	public TileCorner topLeft_Corner;
	public TileCorner topRight_Corner;
	public TileCorner bottomLeft_Corner;
	public TileCorner bottomRight_Corner;

	public SpriteRenderer cross;

    public BoxCollider2D boxCollider2D;

	public int tileModifier;
	public bool isBottomRow;

	private bool topLeft;
	private bool top;
	private bool topRight;
	private bool left;
	private bool right;
	private bool bottomLeft;
	private bool bottom;
	private bool bottomRight;

	public string collisionMask = "";

	public void UpdateTile()
	{
		top = IsNeighbourColliderEnabled(new Vector2(0, 1));
		floor.enabled = boxCollider2D.enabled && !top;

		//cross.gameObject.SetActive(!isBottomRow);
	}

	public void UpdateMask()
	{
		topLeft = IsNeighbourColliderEnabled(new Vector2(-1, 1));
		top = IsNeighbourColliderEnabled(new Vector2(0, 1));
		topRight = IsNeighbourColliderEnabled(new Vector2(1, 1));
		left = IsNeighbourColliderEnabled(new Vector2(-1, 0));
		right = IsNeighbourColliderEnabled(new Vector2(1, 0));
		bottomLeft = IsNeighbourColliderEnabled(new Vector2(-1, -1));
		bottom = IsNeighbourColliderEnabled(new Vector2(0, -1));
		bottomRight = IsNeighbourColliderEnabled(new Vector2(1, -1));

		top_wall.enabled = top && background.enabled;
		right_wall.enabled = right && background.enabled;
		bottom_wall.enabled = bottom && background.enabled ;
		left_wall.enabled = left && background.enabled;

		topLeft_Corner.SetEnabled(((!bottom && !right && topLeft && top && left) || (topLeft && !top && !left) || (topLeft && top && left) || (!topLeft && top && left)) && background.enabled);
		topRight_Corner.SetEnabled(((!bottom && !left && topRight && top && right) || (topRight && !top && !right) || (topRight && top && right) || (!topRight && top && right)) && background.enabled);
		bottomLeft_Corner.SetEnabled(((!top && !right && bottomLeft && bottom && left) || (bottomLeft && !bottom && !left) || (bottomLeft && bottom && left) || (!bottomLeft && bottom && left)) && background.enabled);
		bottomRight_Corner.SetEnabled(((!top && !left && bottomRight && bottom && right) || (bottomRight && !bottom && !right) || (bottomRight && bottom && right) || (!bottomRight && bottom && right)) && background.enabled);

		topLeft_Corner.FlippRotation((topLeft && !top && !left));
		topRight_Corner.FlippRotation((topRight && !top && !right));
		bottomLeft_Corner.FlippRotation((bottomLeft && !bottom && !left));
		bottomRight_Corner.FlippRotation((bottomRight && !bottom && !right));
	}

	private bool IsNeighbourColliderEnabled(Vector2 offset)
	{
		Tile tile = null;
		tile = LevelBuilder.instance.GetTileAtPosition(transform.position + new Vector3(offset.x, offset.y, 0));
		return tile == null || tile.boxCollider2D.enabled;
	}

	public void On_ColorPaletteChanged(ColorPalette palette)
	{
		background.color = palette.wall;
        floor.color = palette.floor;
        pattern.color = palette.pattern;

		Color roofColor = palette.main.SetVibrance(palette.main.GetVibrance()-0.20f);
		Color wallColor = palette.main.SetVibrance(palette.main.GetVibrance()-0.10f);
		Color floorColor = palette.floor.SetVibrance(palette.floor.GetVibrance()+0.10f);
		
		top_wall.color = roofColor;
		right_wall.color = wallColor;
		bottom_wall.color = floorColor;
		left_wall.color = wallColor;

		topLeft_Corner.SetColor(wallColor, roofColor);
		topRight_Corner.SetColor(wallColor, roofColor);
		bottomLeft_Corner.SetColor(wallColor, floorColor);
		bottomRight_Corner.SetColor(wallColor, floorColor);
	}

	private void OnEnable()
	{
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
