using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIToggleButton : UIButton
{
    public bool enabledByDefault;
    public bool interactableWhilePressed = true;

    public ButtonEvent On_Toggle;
    public CustomEvent inspectorEvents;

    private bool isOn {
        get
        {
            return isPressed;
        }
        set
        {
            isPressed = value;
        }
    }

    protected override void Initalize()
    {
        base.Initalize();

        if(enabledByDefault)
            Debug.LogError("enabledByDefault!");
        //isOn = enabledByDefault;
    }

    protected override void Subscribe()
    {
        EventTrigger eT = GetComponent<EventTrigger>();
        eT.AddEventTriggerListener(EventTriggerType.PointerDown, PointerDown);
    }

    public void SetOn(bool isOn)
    {
        this.isOn = isOn;

        if (isOn)
            Press();
        else
            Release();

        On_Toggle?.Invoke();
        inspectorEvents?.Invoke();
    }

    public bool IsOn()
    {
        return isOn;
    }

    private void Toggle()
    {
        SetOn(!isOn);
    }

    private void PointerDown(BaseEventData eventData)
    {
        if(!interactableWhilePressed)
            return;

        Toggle();

        if (isOn)
            Audio.Play(SFX.instance.ui.tallButtonDown, Channel.UI);
        else
            Audio.Play(SFX.instance.ui.tallButtonUp, Channel.UI);
    }
}
