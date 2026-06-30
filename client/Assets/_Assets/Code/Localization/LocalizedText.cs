using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour
{
    public bool flipAlignment;
    public FontType fontType = FontType.NotSelected;

    public string key;
    [HideInInspector] public TranslationKey _key;

    private TextMeshProUGUI UItextMesh;
    private TextMeshPro textMesh;
    private Text lagacyText;

    public void Awake()
    {
        UItextMesh = GetComponent<TextMeshProUGUI>();
        textMesh = GetComponent<TextMeshPro>();
        lagacyText = GetComponent<Text>();
        
        if(fontType == FontType.NotSelected)
            Debug.LogError("FontType Not Selected: " + gameObject.name);
    }

    private void OnEnable()
    {
        On_LanguageChanged(SaveManager.currentSave.language);
        Subscribe();
    }

    public void On_LanguageChanged(Language language)
    {
        if(!this.enabled)
            return;

        if(UItextMesh != null)
        {
            UItextMesh.Translate(_key, language, fontType, flipAlignment);
        }

        if(textMesh != null)
        {
            textMesh.Translate(_key, language, fontType, flipAlignment);
        }

        if(lagacyText != null)
        {
            lagacyText.text = Localization.GetTranslation2(_key, language);
        }
    }
    
    private void On_DyslexicButtonClicked()
    {
        On_LanguageChanged(SaveManager.currentSave.language);
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
