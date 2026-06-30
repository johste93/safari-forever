using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBlockLevelEntity : MonoBehaviour
{
    public LevelEntity entity;

    public Direction4Gizmo directionGizmo;

    private void Start()
    {
        UpdateGizmo();
    }

    private void UpdateGizmo()
    {   
        Vector3 centerPos = entity.GetSerializableData().centerPosition;
        centerPos.z = entity.GetDragHandle().transform.position.z;
        entity.GetDragHandle().transform.position = centerPos;

        bool sizeOne = entity.GetWidth() * entity.GetHeight() == 1;
        directionGizmo.up = directionGizmo.down = sizeOne || entity.GetWidth() > entity.GetHeight();
        directionGizmo.left = directionGizmo.right = sizeOne || entity.GetWidth() < entity.GetHeight();

        if(
            (directionGizmo.direction == Direction4.Up && !directionGizmo.up) ||
            (directionGizmo.direction == Direction4.Right && !directionGizmo.right) ||
			(directionGizmo.direction == Direction4.Down && !directionGizmo.down) ||
            (directionGizmo.direction == Direction4.Left && !directionGizmo.left))
        {   
            directionGizmo.direction = directionGizmo.GetFirstAvailableDirection();
            directionGizmo.face.eulerAngles = new Vector3(0,0, directionGizmo.direction.ToDegree());
            directionGizmo.shadow.eulerAngles = new Vector3(0,0, directionGizmo.direction.ToDegree());
        }
    }

    private void On_EntityChangedSize(LevelEntity entity)
    {
        if(this.entity != entity)
            return;

        UpdateGizmo();
    }

    private void OnEnable()
	{
		LevelEntity.On_EntityChangedSize += On_EntityChangedSize;
	}

	private void Unsubscribe()
	{
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
