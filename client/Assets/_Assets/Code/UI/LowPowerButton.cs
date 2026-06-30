using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowPowerButton : MonoBehaviour
{
    public UIToggleButton button;

    public delegate void LowPowerModeEvent();
    public static LowPowerModeEvent On_LowPowerButtonClicked;

    public void On_Toggle()
    {
        SaveManager.currentSave.lowPowerMode = !SaveManager.currentSave.lowPowerMode;
        SaveManager.Save();

        if (On_LowPowerButtonClicked != null)
            On_LowPowerButtonClicked();
    }

    private void OnEnable()
    {
        button.SetOn(SaveManager.currentSave.lowPowerMode);
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
