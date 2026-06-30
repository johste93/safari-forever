using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class LanguageButton : MonoBehaviour
{
    public Image flagImg;
    public Sprite[] flags;
    public SpriteAlignment[] alignment;
    public GameObject communityBanner;

    public delegate void LanguageButtonEvent(Language language);
    public static LanguageButtonEvent On_LanguageChanged;

    private class LanguageOption
    {
        public string label;
        public Language language;

        public LanguageOption(string label, Language language)
        {
            this.label = label;
            this.language = language;
        }
    }

    private string LanguageNameAsDisplayed(Language lang)
    {
        string LanguageName = Localization.GetTranslation2(TranslationKey.Localization_LocalLanguage_Name, lang);

        //if(Localization.IsRightToLeftLanguage(lang))
            //LanguageName = Localization.HandleLTRTags($"<ltr>{LanguageName}</ltr>");

        return LanguageName;
    }

    public void OnClick()
    {
        List<LocalizedOption> options = new List<LocalizedOption>();

        List<LanguageOption> languageOptions = new List<LanguageOption>();

        foreach(Language lang in Enum.GetValues(typeof(Language)))
        {
            if(Globals.localizationConstants.LanguageIsEnabled(lang))
            {
                string LanguageName = LanguageNameAsDisplayed(lang);
                
                languageOptions.Add(new LanguageOption(LanguageName, lang));
            }
        }

        languageOptions = languageOptions.OrderBy(x => x.label.ToString()).ToList();
        
        foreach(LanguageOption languageOption in languageOptions)
        {
            options.Add(new LocalizedOption(languageOption.label, languageOption.language));
        }

        Language language = Localization.KeyAvailable(TranslationKey.Localization_LocalLanguage_Name, SaveManager.currentSave.language) ? SaveManager.currentSave.language : Globals.localizationConstants.defaultLanguage;

        DialogCanvas.instance.ShowPickerWindow(TranslationKey.Localization_SelectLanguage, language, options, false, false, (selectedOption)=>{
            //Debug.Log(languageOptions[selectedOption].language.ToString());

            Language selectedLanguage = languageOptions[selectedOption].language;

            SaveManager.currentSave.language = selectedLanguage;
            SaveManager.Save();
            Localization.ReloadDictionary();
                    
            flagImg.sprite = flags[(int)selectedLanguage];

            SetAlignment(alignment[(int)selectedLanguage]);

            if(On_LanguageChanged != null)
                On_LanguageChanged(selectedLanguage);

            UpdateBanner();
        });

        int startIndex = options.FindIndex(x => x.label == LanguageNameAsDisplayed(SaveManager.currentSave.language));
        DialogCanvas.instance.pickerWindow.SelectOption(startIndex);

        //Next();
    }

    private void OnEnable()
    {
        int intLang = (int) SaveManager.currentSave.language;
        flagImg.sprite = flags[intLang];
        SetAlignment(alignment[intLang]);
        UpdateBanner();
    }

    private void Next()
    {
        int intLang = (int) SaveManager.currentSave.language;
        intLang++;
        if(intLang >= flags.Length)
            intLang = 0;

        while(!Globals.localizationConstants.LanguageIsEnabled((Language) intLang))
        {
            intLang++;
            if(intLang >= flags.Length)
                intLang = 0;
        }   

        Language language = (Language) intLang;

        SaveManager.currentSave.language = language;
        SaveManager.Save();
        Localization.ReloadDictionary();
                
        flagImg.sprite = flags[intLang];

        SetAlignment(alignment[intLang]);

        if(On_LanguageChanged != null)
            On_LanguageChanged(language);

        UpdateBanner();
    }

    private void SetAlignment(SpriteAlignment a)
    {
        switch(a)
        {
            default:
            case SpriteAlignment.Center:
                flagImg.rectTransform.anchorMin = Vector2.one * 0.5f;
                flagImg.rectTransform.anchorMax = Vector2.one * 0.5f;
                flagImg.rectTransform.pivot = Vector2.one * 0.5f;
                break;
            case SpriteAlignment.TopLeft:
            case SpriteAlignment.LeftCenter:
            case SpriteAlignment.BottomLeft:
                flagImg.rectTransform.anchorMin = new Vector2(0f, 0.5f);
                flagImg.rectTransform.anchorMax = new Vector2(0f, 0.5f);
                flagImg.rectTransform.pivot = new Vector2(0f, 0.5f);
                break;
            case SpriteAlignment.TopRight:
            case SpriteAlignment.RightCenter:
            case SpriteAlignment.BottomRight:
                flagImg.rectTransform.anchorMin = new Vector2(1f, 0.5f);
                flagImg.rectTransform.anchorMax = new Vector2(1f, 0.5f);
                flagImg.rectTransform.pivot = new Vector2(1f, 0.5f);
                break;
        }
    }

    private void UpdateBanner()
    {
        communityBanner.gameObject.SetActive(IsCommunityLanguage(SaveManager.currentSave.language));
    }

    private bool IsCommunityLanguage(Language language)
    {
        switch(language)
        {
            case Language.English:
                return false;
        }

        return true;
    }
}
