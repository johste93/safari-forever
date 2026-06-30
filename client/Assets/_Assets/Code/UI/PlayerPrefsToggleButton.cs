using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPrefsToggleButton : MonoBehaviour
{
    public Image img;
    public Image surface;
    public Image cross;
    public UIToggleButton button;
    public bool isOn;
    public string playerPrefsKey;
    public Color colorWhenPressed;

    public delegate void PlayerPrefsEvent();
    public static PlayerPrefsEvent OnPlayerPrefsUpdated;

    private void OnEnable()
    {
        isOn = PlayerPrefs.GetString(playerPrefsKey, true.ToString()) == "True";
        
        button.SetOn(isOn);

        UpdateColor();
    }

    public void OnClick()
    {
        isOn = !isOn;
        button.SetOn(isOn);
        PlayerPrefs.SetString(playerPrefsKey, isOn.ToString());
        PlayerPrefs.Save();

        if(OnPlayerPrefsUpdated != null)
            OnPlayerPrefsUpdated();

        UpdateColor();
    }

    private void UpdateColor()
    {
        Color c = img.color;
        c.a = isOn ? 0.5f : 1f;
        img.color = c;

        surface.color = isOn ? new Color(0.85f, 0.85f, 0.85f, 1f) : Color.white;

        img.color = isOn ? colorWhenPressed : new Color(0.18f, 0.18f, 0.18f, 1f);
        if(cross != null)
            cross.enabled = isOn;
    }
}
