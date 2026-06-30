using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM_CharacterController2D;

public class LongSpike : Spike, ICollidableTrigger
{   
    public Sprite[] sprites;
    public SpriteRenderer spriteRenderer;
    public BoxCollider2D boxCollider2D;
    
    private Mask collisionMask;
    
    private LevelEntity entity;
    private Direction8 pointingDirection;
    
    private bool hiddenInsideTile;

    private void Awake()
    {
        entity = GetComponentInParent<LevelEntity>(); 
        UpdateSpike();
    }

    public void OnCollisionTrigger(FSM_CharacterController controller, Direction4 direction)
    {
        if(!GameMaster.instance.IsPlaying())
            return;

        if(spriteRenderer.sprite == sprites[1])
        {
            HurtPlayer(controller.character);
            return;
        }

        switch(direction)
        {
            case Direction4.Up:

                if(pointingDirection == Direction8.Down || pointingDirection == Direction8.Left_Down || pointingDirection == Direction8.Right_Down)
                    HurtPlayer(controller.character);

                break;
            case Direction4.Right:

                if(pointingDirection == Direction8.Left || pointingDirection == Direction8.Left_Up || pointingDirection == Direction8.Left_Down)
                    HurtPlayer(controller.character);

                break;
            case Direction4.Down:

                if(pointingDirection == Direction8.Up || pointingDirection == Direction8.Right_Up || pointingDirection == Direction8.Left_Up)
                    HurtPlayer(controller.character);
                

                break;
            case Direction4.Left:
                
                

                if(pointingDirection == Direction8.Right || pointingDirection == Direction8.Right_Up || pointingDirection == Direction8.Right_Down)
                    HurtPlayer(controller.character);

                break;
        }
    }

    private void HurtPlayer(Character character)
    {
        if (character.controller.boxCollider2D.enabled == false)
            return;

        if(!character.IsDead())
        {
            Audio.Play( SFX.instance.level.spike.impale.randomClip, Channel.Game ).SetPitch(Random.Range(0.9f, 1.1f));
            character.Hurt();
        }
    }

    private void UpdateSpike()
    {
		this.DelayEndOfFrame(()=>
		{
			collisionMask = GetCollisionMask();
			collisionMask = collisionMask.Combine(GetSpikeCollisionMask());

			spriteRenderer.sprite = GetSpriteByCollisionMask();
			transform.eulerAngles = GetRotationByCollisionMask();

			hiddenInsideTile = IsNeighbourColliderEnabled(Vector2.zero);
			spriteRenderer.enabled = !hiddenInsideTile;

			//Update collider

			switch(collisionMask.ToString())
			{
				default:
					boxCollider2D.size = new Vector2(0.5f, 0.5f);
					boxCollider2D.offset = Vector2.zero;
					break;
				case "1000":
				case "0100":
				case "0010":
				case "0001":
				case "1110":
				case "0111":
				case "1011":
				case "1101":
					boxCollider2D.size = new Vector2(1,0.6f);
					boxCollider2D.offset = new Vector2(0, -0.2f);
					break;
				case "1100":
				case "0110":
				case "0011":
				case "1001":
					boxCollider2D.size = new Vector2(0.5f, 0.5f);
					boxCollider2D.offset = new Vector2(-0.2f, -0.2f);
					break;
			}
		});
    }

    private Sprite GetSpriteByCollisionMask()
    {
        switch(collisionMask.ToString())
        {
            default:
                return sprites[1];
            case "1000":
            case "0100":
            case "0010":
            case "0001":
            case "1110":
            case "0111":
            case "1011":
            case "1101":
                return sprites[0];
            case "1100":
            case "0110":
            case "0011":
            case "1001":
                return sprites[2];
        }
    }

    private Vector3 GetRotationByCollisionMask()
    {
        switch(collisionMask.ToString())
        {
            case "1000":
            case "1101":
                pointingDirection = Direction8.Down;
                return new Vector3(0,0,180);
            case "0100":
            case "1110":
                pointingDirection = Direction8.Left;
                return new Vector3(0,0,90);
            default:
            case "0010":
            case "0111":
                pointingDirection = Direction8.Up;
                return Vector3.zero;
            case "0001":
            case "1011":
                pointingDirection = Direction8.Right;
                return new Vector3(0,0,270);

            case "1100":    //TopRight
                pointingDirection = Direction8.Left_Down;
                return new Vector3(0,0,180);

            case "0110":    //BottomRight
                pointingDirection = Direction8.Left_Up;
                return new Vector3(0,0,90);

            case "0011":    //BottomLeft
                
                pointingDirection = Direction8.Right_Up;
                return Vector3.zero;
                
            case "1001":    //TopLeft
                pointingDirection = Direction8.Right_Down;
                return new Vector3(0,0,270);
        }
    }

	private Mask GetSpikeCollisionMask()
	{
		List<Vector2Int> positions = LevelBuilder.instance.GetSpikePositions();

		bool top = IsNeighbourSpike(positions, new Vector2Int(0, 1));
		bool left = IsNeighbourSpike(positions, new Vector2Int(-1, 0));
		bool right = IsNeighbourSpike(positions, new Vector2Int(1, 0));
		bool bottom = IsNeighbourSpike(positions, new Vector2Int(0, -1));

		return new Mask(){
			top = top,
			left = left,
			right = right,
			bottom = bottom
		};
	}

	private bool IsNeighbourSpike(List<Vector2Int> spikePositions, Vector2Int offset)
	{
		Vector2Int pos = postion + offset;
		return spikePositions.Contains(pos);
	}

    private Mask GetCollisionMask()
	{   
        //bool topLeft = IsNeighbourColliderEnabled(new Vector2(-1, 1));
		bool top = IsNeighbourColliderEnabled(new Vector2(0, 1));
        //bool topRight = IsNeighbourColliderEnabled(new Vector2(1, 1));
        bool left = IsNeighbourColliderEnabled(new Vector2(-1, 0));

        bool right = IsNeighbourColliderEnabled(new Vector2(1, 0));
        
        //bool bottomLeft = IsNeighbourColliderEnabled(new Vector2(-1, -1));
		bool bottom = IsNeighbourColliderEnabled(new Vector2(0, -1));
        //bool bottomRight = IsNeighbourColliderEnabled(new Vector2(1, -1));
        
        
        //Debug.Log($"{top.ToInt()}{topRight.ToInt()}{right.ToInt()}{bottomRight.ToInt()}{bottom.ToInt()}{bottomLeft.ToInt()}{left.ToInt()}{topLeft.ToInt()}");
        /*
        Debug.Log( 
            (topLeft?"x":"o") + (top?"x":"o") + (topRight?"x":"o") + "\n" +
            (left?"x":"o") + "x" + (right?"x":"o") + "\n" +
            (bottomLeft?"x":"o") + (bottom?"x":"o") + (bottomRight?"x":"o")
        );
        */

		return new Mask(){
			top = top,
			left = left,
			right = right,
			bottom = bottom
		};
	}

	private bool IsNeighbourColliderEnabled(Vector2 offset)
	{
		Tile tile = null;
		tile = LevelBuilder.instance.GetTileAtPosition(transform.position + new Vector3(offset.x, offset.y, 0));
		return tile == null || tile.boxCollider2D.enabled;
	}

    private void On_EntityChangedEvent(LevelEntity entity)
    {
        UpdateSpike();
    }

    private void On_EnterPlayMode()
    {
        boxCollider2D.enabled = !hiddenInsideTile;;
    }

    private void On_ExitPlayMode()
    {
        boxCollider2D.enabled = false;
    }

    private void On_RoomLoaded()
    {
        UpdateSpike();
    }

    private void OnEnable()
    {
        LevelEntity.On_EntityMoved += On_EntityChangedEvent;
        LevelEntity.On_EntityChangedSize += On_EntityChangedEvent;
        LevelBuilder.On_RoomLoaded += On_RoomLoaded;
        //LevelEntity.On_EntityStoppedMoving += On_EntityStoppedMoving;

        GameMaster.On_EnterPlayMode += On_EnterPlayMode;
        GameMaster.On_ExitPlayMode += On_ExitPlayMode;

		LevelBuilder.instance.RegisterSpike(this);
    }

    private void Unsubscribe()
    {
        LevelEntity.On_EntityMoved -= On_EntityChangedEvent;
        LevelEntity.On_EntityChangedSize -= On_EntityChangedEvent;
        LevelBuilder.On_RoomLoaded -= On_RoomLoaded;
        //LevelEntity.On_EntityStoppedMoving -= On_EntityStoppedMoving;

        GameMaster.On_EnterPlayMode -= On_EnterPlayMode;
        GameMaster.On_ExitPlayMode -= On_ExitPlayMode;

		LevelBuilder.instance?.UnRegisterSpike(this);
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

	private struct Mask{
		public bool top, left, right, bottom;

		public override string ToString()
		{
			return "" + top.ToInt() + right.ToInt() + bottom.ToInt() + left.ToInt();
		}

		public Mask Combine(Mask other)
		{
			return new Mask()
			{
				top = top || other.top,
				left = left || other.left,
				right = right || other.right,
				bottom = bottom || other.bottom
			};
		}
	}
}
