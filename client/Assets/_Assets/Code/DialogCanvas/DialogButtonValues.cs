using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogButtonValues
{
    public TranslationKey key;
    public string text;
    public Language language;
    public System.Action callback;
    public DialogButtonType type;
    public bool isPreferableButton;
    public bool isRightToLeftText;

    public DialogButtonValues(string text, Language language, DialogButtonType type, System.Action callback, bool isPreferableButton, bool isRightToLeftText)
    {
        this.text = text;
        this.language = language;
        this.type = type;
        this.callback = callback;
        this.isPreferableButton = isPreferableButton;
        this.isRightToLeftText = isRightToLeftText;
    }

    public DialogButtonValues(TranslationKey key, DialogButtonType type, System.Action callback, bool isPreferableButton)
    {
        this.key = key;
        this.language = SaveManager.currentSave.language;
        this.type = type;
        this.callback = callback;
        this.isPreferableButton = isPreferableButton;
    }
}
