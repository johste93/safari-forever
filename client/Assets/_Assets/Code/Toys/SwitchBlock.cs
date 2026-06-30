using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SF.LogicSystem.v2;

public class SwitchBlock : Block, ISuspendable
{
    public BoxCollider2D boxCollider2D;   
    private LevelEntity entity;

    private int lastFrameOfOn;
    private bool justBecamePowered;
    private bool isSuspended;

    private int framesOfPower;

    private void Awake()
    {
        entity = GetComponentInParent<LevelEntity>();
    }

    private void UpdateBlock()
    {
        bool enabled = framesOfPower >= 2 || !entity.inputNode.HasConnections();
        boxCollider2D.enabled = enabled;

        foreach(SpriteRenderer sR in spriteRenderers)
            sR.color = sR.color.SetAlpha(enabled ? 1f : 0.25f);

        //Check if player is inside block. If yes then kill
        if(justBecamePowered)
        {
            justBecamePowered = false;
            Collider2D playerCollider = Physics2D.OverlapBox(boxCollider2D.transform.position + (Vector3)boxCollider2D.offset, boxCollider2D.size, 0f, Globals.gameConstants.whatIsPlayer);
            if(playerCollider != null && boxCollider2D.Overlaps((BoxCollider2D) playerCollider))
            {
                if(playerCollider.enabled)
                    playerCollider.GetComponent<Character>().Hurt();
            }

            Collider2D[] enemies = Physics2D.OverlapBoxAll(boxCollider2D.transform.position + (Vector3)boxCollider2D.offset, boxCollider2D.size, 0f, Globals.gameConstants.whatIsEnemy);
            foreach(Collider2D enemy in enemies)
            {
                if(boxCollider2D.Overlaps((BoxCollider2D) enemy))
                    enemy.GetComponent<Spikeball>().Die();
            }
        }
    }

    private void LateUpdate()
    {
        if(!GameMaster.instance.IsPlaying())
            return;

        if(isSuspended)
			return;

        if(entity.inputNode.HasConnections() && entity.inputNode.IsPowered())
        {
            if(framesOfPower == 0)
                justBecamePowered = true;

            framesOfPower++;
        }
        else
            framesOfPower = 0;

        UpdateBlock();
    }

    private void Reset()
    {
        justBecamePowered = false;
        framesOfPower = 2;
        
        boxCollider2D.enabled = false;

        foreach(SpriteRenderer sR in spriteRenderers)
            sR.color = sR.color.SetAlpha(1f);
    }

    private void On_LevelReset(bool manual) => Reset();

    protected override void OnEnable()
    {
        base.OnEnable();
        GameMaster.On_EnterPlayMode += Reset;
        GameMaster.On_LevelReset += On_LevelReset;
        GameMaster.On_ExitPlayMode += Reset;
        

        //entity.inputNode.On_BecamePowered += LogicUpdate;
        //entity.inputNode.On_LostPower += LogicUpdate;

        SuspensionManager.On_SuspensionEvent += On_SuspensionEvent;

        //UpdateBlock();
    }

    protected override void Unsubscribe()
    {
        base.Unsubscribe();
        GameMaster.On_EnterPlayMode -= Reset;
        GameMaster.On_LevelReset -= On_LevelReset;
        GameMaster.On_ExitPlayMode -= Reset;

        //entity.inputNode.On_BecamePowered -= LogicUpdate;
        //entity.inputNode.On_LostPower -= LogicUpdate;

        SuspensionManager.On_SuspensionEvent -= On_SuspensionEvent;
    }

    public void On_SuspensionEvent(bool isSuspended)
	{
		Suspend(isSuspended);
	}

	public void Suspend(bool suspend)
	{
		this.isSuspended = suspend;
	}
}
