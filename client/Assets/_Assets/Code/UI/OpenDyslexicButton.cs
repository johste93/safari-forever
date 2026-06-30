using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDyslexicButton : MonoBehaviour
{
    public UIToggleButton button;

    public delegate void OpenDyslexicButtonEvent();
    public static OpenDyslexicButtonEvent On_DyslexicButtonClicked;

    public void On_Toggle()
    {
        SaveManager.currentSave.openDyslexic = button.IsOn();
        SaveManager.Save();

        On_DyslexicButtonClicked?.Invoke();
    }

    private void OnEnable()
    {
        button.SetOn(SaveManager.currentSave.openDyslexic);
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
