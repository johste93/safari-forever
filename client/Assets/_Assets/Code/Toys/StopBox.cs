using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM_CharacterController2D;

public class StopBox : FunctionBox
{   
    private bool waitingLogicEvent;

    protected override void OnHit(FSM_CharacterController characterController)
    {
        base.OnHit(characterController);
        characterController.stateController.OverideState(State.Stopped);
        Audio.Play( SFX.instance.level.logicSwitch.beep.randomClip, Channel.Game ).SetPitch(SFX.instance.level.logicSwitch.beep.defaultPitch).SetVolume(0.1f);
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

    public override void Revert()
    {
        base.Revert();
        waitingLogicEvent = false;
    }
}
