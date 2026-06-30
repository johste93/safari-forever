using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialog
{
    public string title;
    public string message;

    public bool titleIsRTL;
    public bool messageIsRTL;

    public TranslationKey titleKey;
    public TranslationKey messageKey;
    public Language titleLanguage;
    public Language messageLanguage;

    public List<DialogButtonValues> buttons;

    public Dialog(string title, bool titleIsRTL, Language titleLanguage, string message, bool messageIsRTL, Language messageLanguage)
    {
        this.title = title;
        this.titleIsRTL = titleIsRTL;
        this.titleLanguage = titleLanguage;

        this.message = message;
        this.messageIsRTL = messageIsRTL;
        this.messageLanguage = messageLanguage;

        buttons = new List<DialogButtonValues>();
    }

    public Dialog(TranslationKey titleKey, string message, Language messageLanguage, bool messageIsRTL)
    {
        this.titleKey = titleKey;

        this.message = message;
        this.messageIsRTL = messageIsRTL;
        this.messageLanguage = messageLanguage;

        buttons = new List<DialogButtonValues>();
    }

    public Dialog(string title, Language titleLanguage, TranslationKey messageKey, bool titleIsRTL)
    {
        this.title = title;
        this.titleIsRTL = titleIsRTL;
        this.titleLanguage = titleLanguage;

        this.messageKey = messageKey;
    
        buttons = new List<DialogButtonValues>();
    }

    public Dialog(TranslationKey titleKey, TranslationKey messageKey)
    {
        this.titleKey = titleKey;
        this.messageKey = messageKey;

        buttons = new List<DialogButtonValues>();
    }

    public Dialog AddNeutralButton(string text, Language language, System.Action callback, bool isRightToLeftText, bool isPreferedButton = false)
    {
        buttons.Add(new DialogButtonValues(text, language, DialogButtonType.Neutral, callback, isPreferedButton, isRightToLeftText));
        return this;
    }

    public Dialog AddNeutralButton(TranslationKey key, System.Action callback, bool isPreferedButton = false)
    {
        buttons.Add(new DialogButtonValues(key, DialogButtonType.Neutral, callback, isPreferedButton));
        return this;
    }

    public Dialog AddPositiveButton(string text, Language language, System.Action callback, bool isRightToLeftText, bool isPreferedButton = false)
    {
        buttons.Add(new DialogButtonValues(text, language, DialogButtonType.Positive, callback, isPreferedButton, isRightToLeftText));
        return this;
    }

    public Dialog AddPositiveButton(TranslationKey key, System.Action callback, bool isPreferedButton = false)
    {
        buttons.Add(new DialogButtonValues(key, DialogButtonType.Positive, callback, isPreferedButton));
        return this;
    }

    public Dialog AddNegativeButton(string text, Language language, System.Action callback, bool isRightToLeftText, bool isPreferedButton = false)
    {
        buttons.Add(new DialogButtonValues(text, language, DialogButtonType.Negative, callback, isPreferedButton, isRightToLeftText));
        return this;
    }

    public Dialog AddNegativeButton(TranslationKey key, System.Action callback, bool isPreferedButton = false)
    {
        buttons.Add(new DialogButtonValues(key, DialogButtonType.Negative, callback, isPreferedButton));
        return this;
    }

    public Dialog AddDestructiveButton(string text, Language language,  System.Action callback, bool isRightToLeftText, bool isPreferedButton = false)
    {
        buttons.Add(new DialogButtonValues(text, language, DialogButtonType.Destructive, callback, isPreferedButton, isRightToLeftText));
        return this;
    }

    public Dialog AddDestructiveButton(TranslationKey key, System.Action callback, bool isPreferedButton = false)
    {
        buttons.Add(new DialogButtonValues(key, DialogButtonType.Destructive, callback, isPreferedButton));
        return this;
    }

    public void Show(bool instant = false)
    {
        if(DialogCanvas.instance == null)
        {
            Debug.LogError("There is no dialog canvas in the scene!");
            return;
        }

        DialogCanvas.instance.Show(this, instant);
    }
}
