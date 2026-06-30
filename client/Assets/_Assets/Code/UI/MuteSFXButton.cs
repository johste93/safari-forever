using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuteSFXButton : MonoBehaviour
{
    public GameObject cross;
    public UIToggleButton button;

    public void On_Toggle()
    {
        SaveManager.currentSave.sfx = !button.IsOn();
        SaveManager.Save();

        cross.SetActive(!SaveManager.currentSave.sfx);
    }

    private void OnEnable()
    {
        button.SetOn(!SaveManager.currentSave.sfx);
        cross.SetActive(!SaveManager.currentSave.sfx);
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
