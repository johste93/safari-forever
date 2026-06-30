using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM_CharacterController2D;

public class HelpBox : FunctionBox
{
    protected override void OnHit(FSM_CharacterController characterController)
    {
        base.OnHit(characterController);

        characterController.stateController.OverideState(State.Idle);
        Audio.Play(SFX.instance.ui.dialogBoxOpen, Channel.Game);

        new Dialog(TranslationKey.Tips_Header, TranslationKey.Tips_HoldToJumpHeigher)
            .AddPositiveButton(TranslationKey.Generic_Ok, ()=>{
                Audio.Play(SFX.instance.ui.dialogBoxClose, Channel.Game);
            })
            .Show();
    }
}