using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MonoChromatic : MonoBehaviour
{
    public UIToggleButton button;
    public Image surface;
    public TextMeshProUGUI textMesh;

    private Color fontDefaultColor;

    public void On_Toggle()
    {
        SaveManager.currentSave.monoChromatic = button.IsOn();
        SaveManager.Save();

        UpdateButton();
    }

    private void UpdateButton()
    {
        surface.color = SaveManager.currentSave.monoChromatic ? Color.black : Color.white;
        textMesh.color = SaveManager.currentSave.monoChromatic ? Color.white : fontDefaultColor;
    }

    private void OnEnable()
    {
        fontDefaultColor = textMesh.color;
        button.SetOn(SaveManager.currentSave.monoChromatic);
        UpdateButton();

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
