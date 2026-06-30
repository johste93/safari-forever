using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM_CharacterController2D;

public class NoteBox : FunctionBox
{
    private AudioPlayer currentPlayer;

    private bool isPowered;

    public Octave octave;
    public enum Octave
    {
        low,
        medium,
        high,
        sfx
    }

    protected override void OnHit(FSM_CharacterController characterController)
    {
        base.OnHit(characterController);
        PlaySound();
    }

    private void PlaySound()
    {
        int index = ((OptionGizmo)entity.gizmo).index;

        currentPlayer?.Kill();

        switch(octave)
        {
            case Octave.low:
                currentPlayer = Audio.Play(SFX.instance.level.noteBox.octave_low.clips[index], Channel.Game).SetVolume(SFX.instance.level.noteBox.octave_low.defaultVolume).SetPitch(SFX.instance.level.noteBox.octave_low.defaultPitch);
                break;
            case Octave.medium:
                currentPlayer = Audio.Play(SFX.instance.level.noteBox.octave_medium.clips[index], Channel.Game).SetVolume(SFX.instance.level.noteBox.octave_medium.defaultVolume).SetPitch(SFX.instance.level.noteBox.octave_medium.defaultPitch);
                break;
            case Octave.high:
                currentPlayer = Audio.Play(SFX.instance.level.noteBox.octave_high.clips[index], Channel.Game).SetVolume(SFX.instance.level.noteBox.octave_high.defaultVolume).SetPitch(SFX.instance.level.noteBox.octave_high.defaultPitch);
                break;
            case Octave.sfx:
                currentPlayer = Audio.Play(SFX.instance.level.noteBox.sfx.clips[index], Channel.Game).SetVolume(SFX.instance.level.noteBox.sfx.defaultVolume).SetPitch(SFX.instance.level.noteBox.sfx.defaultPitch);
                break;
        }
    }

    private void LateUpdate()
    {
        if(entity.inputNode.IsPowered())
        {
            if(!isPowered)
            {
                isPowered = true;
                Jump();
                PlaySound();
            }
        }
        else
        {
            if(isPowered)
            {
                isPowered = false;
            }
        }
    }

    public override void Reset()
	{
        currentPlayer?.Kill();
        currentPlayer = null;
        isPowered = false;
		base.Reset();
	}
}
