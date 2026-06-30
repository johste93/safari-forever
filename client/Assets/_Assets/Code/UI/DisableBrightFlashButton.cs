using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableBrightFlashButton : MonoBehaviour
{
    public UIToggleButton button;

    public void On_Toggle()
    {
        SaveManager.currentSave.noBrightFlashesMode = !SaveManager.currentSave.noBrightFlashesMode;
        SaveManager.Save();
    }

    private void OnEnable()
    {
        button.SetOn(SaveManager.currentSave.noBrightFlashesMode);
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
