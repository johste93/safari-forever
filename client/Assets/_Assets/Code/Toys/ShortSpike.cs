using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM_CharacterController2D;

public class ShortSpike : Spike
{   
    public BoxCollider2D boxCollider2D;
    
    private LevelEntity entity;

    
    private bool hiddenInsideTile;

    private void Awake()
    {
        entity = GetComponentInParent<LevelEntity>(); 
    }

    private void Start()
    {
        UpdateSpike();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.enabled == false)
            return;
            
        if(!GameMaster.instance.IsPlaying())
            return;

        if(!Globals.gameConstants.whatIsPlayer.Contains(other.gameObject.layer))
            return;

        FSM_CharacterController controller = other.GetComponent<FSM_CharacterController>();

        HurtPlayer(controller.character);
    }

    private void HurtPlayer(Character character)
    {
        if(!character.IsDead())
        {
            Audio.Play( SFX.instance.level.spike.impale.randomClip, Channel.Game ).SetPitch(Random.Range(0.9f, 1.1f));
            character.Hurt();
        }
    }

    private void UpdateSpike()
    {
        Direction4 direction = ((Direction4Gizmo)entity.gizmo).direction;

		transform.eulerAngles = new Vector3(0,0, direction.ToDegree());
    }

    private void On_RoomLoaded()
    {
        UpdateSpike();
    }

    private void On_EnterPlayMode()
    {
        boxCollider2D.enabled = !hiddenInsideTile;
    }

    private void On_ExitPlayMode()
    {
        boxCollider2D.enabled = false;
    }

    private void OnEnable()
    {
        LevelBuilder.On_RoomLoaded += On_RoomLoaded;
        //LevelEntity.On_EntityStoppedMoving += On_EntityStoppedMoving;

        GameMaster.On_EnterPlayMode += On_EnterPlayMode;
        GameMaster.On_ExitPlayMode += On_ExitPlayMode;

		LevelBuilder.instance.RegisterSpike(this);
    }

    private void Unsubscribe()
    {
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
}
