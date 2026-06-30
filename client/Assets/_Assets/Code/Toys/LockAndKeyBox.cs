using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM_CharacterController2D;

public class LockAndKeyBox : FunctionBox
{
    public SpriteRenderer icon;
    public Sprite activeSprite;
    public Sprite inactiveSprite;
    public Sprite lockedSprite;

    public Rotate rotator;
    public Color offColor;



    private bool locked = true;
    private bool waitingLogicEvent;

    protected override void OnHit(FSM_CharacterController characterController)
    {
        if(locked)
        {
            if(characterController.character.keys.Count == 0)
                return;

            Unlock(characterController);
        }

        base.OnHit(characterController);

        Audio.Play( SFX.instance.level.logicSwitch.beep, Channel.Game );
        waitingLogicEvent = true;
    }

    protected void Unlock(FSM_CharacterController characterController)
    {
        characterController.character.keys[0].Unlock();

        locked = false;
        rotator.enabled = !locked;
        icon.color = Color.white;

        base.Jump();
        Audio.Play( SFX.instance.level.lockAndKey.unlock, Channel.Game );
    }

    protected override void Jump()
    {
        if(locked)
            return;

        base.Jump();
        icon.sprite = activeSprite;
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

    public override void Revert()
    {
        base.Revert();
        icon.sprite = inactiveSprite;
        waitingLogicEvent = false;
    }

    public override void Reset()
    {
        base.Reset();
        waitingLogicEvent = false;
        locked = true;
        icon.color = offColor;
        icon.sprite = lockedSprite;
        rotator.enabled = !locked;
    }
}
