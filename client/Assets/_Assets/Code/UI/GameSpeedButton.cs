using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameSpeedButton : MonoBehaviour
{   
    public TextMeshProUGUI textMesh;
    //public TextMeshProUGUI percentText;

    public float step = 0.1f;

    private float min = 0.6f;
    private float max = 1.0f;

    private void OnEnable()
    {
        float x = SaveManager.currentSave.gameSpeed;

        string percentText = (Mathf.RoundToInt(x * 100f)+10) + "%";
        textMesh.Translate(TranslationKey.Menu_Accessibility_GameSpeed, SaveManager.currentSave.language, FontType.Stylized, false);
        textMesh.text += $"  {percentText}";

        Subscribe();
    }

    public void OnClick()
    {
        float x = SaveManager.currentSave.gameSpeed;
        x += step;
        if (x < min)
            x = max - (min - x) % (max - min);
        else
            x = min + (x - min) % (max - min);

        SaveManager.currentSave.gameSpeed = x;
        SaveManager.Save();
        
        string percentText = (Mathf.RoundToInt(x * 100f)+10) + "%";
        textMesh.Translate(TranslationKey.Menu_Accessibility_GameSpeed, SaveManager.currentSave.language, FontType.Stylized, false);
        textMesh.text += $"  {percentText}";
    }

    private void On_DyslexicButtonClicked()
    {
        textMesh.font = Globals.accessibilityConstants.GetFont(SaveManager.currentSave.openDyslexic, FontType.Stylized, textMesh.font, SaveManager.currentSave.language);
    }

    private void On_LanguageChanged(Language language)
    {
        On_DyslexicButtonClicked();
    }

    private void Subscribe()
    {
        OpenDyslexicButton.On_DyslexicButtonClicked += On_DyslexicButtonClicked;
        LanguageButton.On_LanguageChanged += On_LanguageChanged;
    }

    private void Unsubscribe()
    {
        OpenDyslexicButton.On_DyslexicButtonClicked -= On_DyslexicButtonClicked;
        LanguageButton.On_LanguageChanged -= On_LanguageChanged;
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }
}
