using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySpy : MonoBehaviour
{
    public delegate void MovingEntities();
    public static MovingEntities OnSomeEntityStartedMoving;
    public static MovingEntities OnAllEntitiesStoppedMoving;

    private List<LevelEntity> movingEntities = new List<LevelEntity>();

    private void OnEntityMoved(LevelEntity entity)
    {
        if(movingEntities.Contains(entity))
            return;

        movingEntities.Add(entity);

        if(OnSomeEntityStartedMoving != null)
            OnSomeEntityStartedMoving();
    }

    private void OnEntityStoppedMoving(LevelEntity entity)
    {
        movingEntities.Remove(entity);

        if(movingEntities.Count == 0)
            if(OnAllEntitiesStoppedMoving != null)
                OnAllEntitiesStoppedMoving();
    }

    private void OnEnable()
    {
        LevelEntity.On_EntityMoved += OnEntityMoved;
        LevelEntity.On_EntityStoppedMoving += OnEntityStoppedMoving;
    }

    private void Unsubscribe()
    {
        LevelEntity.On_EntityMoved -= OnEntityMoved;
        LevelEntity.On_EntityStoppedMoving -= OnEntityStoppedMoving;
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
