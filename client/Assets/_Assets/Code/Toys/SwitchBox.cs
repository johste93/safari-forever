using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM_CharacterController2D;

public class SwitchBox : FunctionBox
{
    public Sprite activeSprite;
    public Sprite inactiveSprite;

    public SpriteRenderer icon;

    private bool waitingLogicEvent;

    protected override void OnHit(FSM_CharacterController characterController)
    {
        base.OnHit(characterController);

        Audio.Play( SFX.instance.level.logicSwitch.beep, Channel.Game );
        waitingLogicEvent = true;
    }

    private void Update()
	{
        if(!GameMaster.instance.IsPlaying())
            return;

        DoLogicUpdate();
    }

    private void DoLogicUpdate()
    {
        if(waitingLogicEvent)
        {
            entity.outputNode.EmitPower(true);
            waitingLogicEvent = false;
        }
        else
        {
            entity.outputNode.EmitPower(false);
        } 
    }

    protected override void Jump()
    {
        base.Jump();
        icon.sprite = activeSprite;
    }

    public override void Revert()
    {
        base.Revert();
        icon.sprite = inactiveSprite;
        waitingLogicEvent = false;
    }
}
