using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM_CharacterController2D;
using DG.Tweening;

public class PowerSupplyBox : FunctionBox
{
    public Sprite onSprite;
    public Sprite offSprite;

    public SpriteRenderer iconRenderer;
    public SpriteRenderer shadowRenderer;

    public Color onColor;
    public Color offColor;

    public bool isOn = true;
    private bool isPowered = false;

    protected override void OnHit(FSM_CharacterController characterController)
    {
        base.OnHit(characterController);
        SetIsOn(!isOn);
        Audio.Play( SFX.instance.level.logicSwitch.beep.clips[isOn ? 0 : 1], Channel.Game ).SetVolume(SFX.instance.level.logicSwitch.beep.defaultVolume).SetPitch(SFX.instance.level.logicSwitch.beep.defaultPitch);
    }


    private int lastFrameOfPaused;
    private void Update()
    {
        if(!GameMaster.instance.IsPlaying())
        {
            lastFrameOfPaused = Time.frameCount;
            return;
        }

        entity.outputNode.EmitPower(isOn);
    }
        
    private void LateUpdate()
    {
        if(entity.inputNode.IsPowered())
        {
            if(!isPowered)
            {
                isPowered = true;
                Jump();
                SetIsOn(!isOn);
                Audio.Play( SFX.instance.level.logicSwitch.beep.clips[isOn ? 0 : 1], Channel.Game ).SetVolume(SFX.instance.level.logicSwitch.beep.defaultVolume).SetPitch(SFX.instance.level.logicSwitch.beep.defaultPitch);
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

    private void UpdateBox()
    {
        iconRenderer.color = isOn ? onColor : offColor;
    }

    public void ResetSwitch()
    {
        SetIsOn(((OptionGizmo)entity.gizmo).index == 1);
    }

    public void SetIsOn(bool isOn)
    {
        this.isOn = isOn;
        UpdateBox();
    }

    public bool EmitsPower()
    {
        return this.isOn;
    }

    public override void Reset()
	{
        isPowered = false;

        ResetSwitch();

		foreach(SpriteRenderer sR in spriteRenderers)
		{
			sR.color = sR.color.SetAlpha(1f);
		}

		child.localPosition = Vector3.zero;
	}

    private void On_EnterPlayMode()
    {
        ResetSwitch();
        entity.outputNode.EmitPower(isOn);
    }

    private void On_LevelReset(bool manual) => ResetSwitch();

    protected override void OnEnable()
    {
        base.OnEnable();

        GameMaster.On_EnterPlayMode += On_EnterPlayMode;
        GameMaster.On_LevelReset += On_LevelReset;
        GameMaster.On_ExitPlayMode += ResetSwitch;
        ResetSwitch();
    }

    protected override void Unsubscribe()
    {
        base.Unsubscribe();
        
        GameMaster.On_EnterPlayMode -= On_EnterPlayMode;
        GameMaster.On_LevelReset -= On_LevelReset;
        GameMaster.On_ExitPlayMode -= ResetSwitch;
    }
}
