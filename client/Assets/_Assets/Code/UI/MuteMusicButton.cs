using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuteMusicButton : MonoBehaviour
{
    public GameObject cross;
    public UIToggleButton button;

    public delegate void MuteMusicEvent();
    public static MuteMusicEvent On_MuteMusicButtonClicked;

    public void On_Toggle()
    {
        SaveManager.currentSave.music = !button.IsOn();
        SaveManager.Save();

        cross.SetActive(!SaveManager.currentSave.music);

        if(On_MuteMusicButtonClicked != null)
            On_MuteMusicButtonClicked();

        MusicManager.On_MuteMusicButtonClicked();
    }

    private void OnEnable()
    {
        button.SetOn(!SaveManager.currentSave.music);
        cross.SetActive(!SaveManager.currentSave.music);
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
