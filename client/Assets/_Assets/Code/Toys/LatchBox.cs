using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM_CharacterController2D;
using DG.Tweening;

public class LatchBox : FunctionBox
{
    public Sprite onSprite;
    public Sprite offSprite;

    public SpriteRenderer iconRenderer;
    public SpriteRenderer gradiantRenderer;

    public Rotate rotator;

    public Color onColor;
    public Color offColor;

    public bool isOn = true;
    private bool isPowered = false;

    private bool hasBeenToggled = false;

    protected override void OnHit(FSM_CharacterController characterController)
    {
        if(hasBeenToggled)
            return;

        hasBeenToggled = true;

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
        if(hasBeenToggled)
            return;

        if(entity.inputNode.IsPowered())
        {
            if(!isPowered)
            {
                isPowered = true;
                Jump();
                SetIsOn(!isOn);
                Audio.Play( SFX.instance.level.logicSwitch.beep.clips[isOn ? 0 : 1], Channel.Game ).SetVolume(SFX.instance.level.logicSwitch.beep.defaultVolume).SetPitch(SFX.instance.level.logicSwitch.beep.defaultPitch);
                hasBeenToggled = true;
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

    protected override void Jump()
    {
        if(hasBeenToggled)
            return;

        base.Jump();
    }

    private void UpdateBox()
    {
        iconRenderer.color = hasBeenToggled ? offColor : onColor;
        iconRenderer.sprite = isOn ? onSprite : offSprite;
        gradiantRenderer.color = gradiantRenderer.color.SetVibrance(hasBeenToggled ? 0.5f : 1f);
        rotator.enabled = !hasBeenToggled;
    }

    public void ResetSwitch()
    {
        SetIsOn(((OptionGizmo)entity.gizmo).index == 1);
        hasBeenToggled = false;
        UpdateBox();
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
