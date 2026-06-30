using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoOrientationButton : MonoBehaviour
{
    public UIToggleButton button;

    public delegate void LowPowerModeEvent();
    public static LowPowerModeEvent On_LowPowerButtonClicked;

    public void On_Toggle()
    {
        SaveManager.autoOrientationEnabled = button.IsOn();
        ScreenOrientationManager.instance.UpdateScreenOrientation();
    }

    private void OnEnable()
    {
        button.SetOn(SaveManager.autoOrientationEnabled);
        button.On_Toggle += On_Toggle;
    }

    private void Unsubscribe()
    {
        button.On_Toggle -= On_Toggle;
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }
}
