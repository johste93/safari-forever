using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LocalizedFont : MonoBehaviour
{
    public FontType type;
    public bool flipAlignment = true;

    private TextMeshProUGUI textMesh;
    private TextMeshPro textMesh3D;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        textMesh3D = GetComponent<TextMeshPro>();
    }

    private void OnEnable()
    {
        On_DyslexicButtonClicked();
        Subscribe();
    }

    private void On_DyslexicButtonClicked()
    {
        if(textMesh3D != null)
        {
            textMesh3D.font = Globals.accessibilityConstants.GetFont(SaveManager.currentSave.openDyslexic, type, textMesh3D.font, SaveManager.currentSave.language);
        }

        if(textMesh != null)
        {
            textMesh.font = Globals.accessibilityConstants.GetFont(SaveManager.currentSave.openDyslexic, type, textMesh.font, SaveManager.currentSave.language);
        }

        UpdateTextAlignment();
    }

    private void UpdateTextAlignment()
    {
        if(!flipAlignment)
            return;
            
        if(textMesh3D != null)
        {
            if( (textMesh3D.alignment == TextAlignmentOptions.Left 
                || textMesh3D.alignment == TextAlignmentOptions.TopLeft
                || textMesh3D.alignment == TextAlignmentOptions.BottomLeft) && Localization.IsRightToLeftLanguage(SaveManager.currentSave.language))
                    textMesh3D.alignment = FlipHorizontally(textMesh3D.alignment);
            
            if( (textMesh3D.alignment == TextAlignmentOptions.Right 
                || textMesh3D.alignment == TextAlignmentOptions.TopRight
                || textMesh3D.alignment == TextAlignmentOptions.BottomRight) && !Localization.IsRightToLeftLanguage(SaveManager.currentSave.language))
                    textMesh3D.alignment = FlipHorizontally(textMesh3D.alignment);
        }

        if(textMesh != null)
        {
            if( (textMesh.alignment == TextAlignmentOptions.Left 
                || textMesh.alignment == TextAlignmentOptions.TopLeft
                || textMesh.alignment == TextAlignmentOptions.BottomLeft) && Localization.IsRightToLeftLanguage(SaveManager.currentSave.language))
                    textMesh.alignment = FlipHorizontally(textMesh.alignment);

            if( (textMesh.alignment == TextAlignmentOptions.Right 
                || textMesh.alignment == TextAlignmentOptions.TopRight
                || textMesh.alignment == TextAlignmentOptions.BottomRight) && !Localization.IsRightToLeftLanguage(SaveManager.currentSave.language))
                    textMesh.alignment = FlipHorizontally(textMesh.alignment);
        }
    }

    private TextAlignmentOptions FlipHorizontally(TextAlignmentOptions input)
    {
        switch(input)
        {
            case TextAlignmentOptions.Left:
                return TextAlignmentOptions.Right;
            case TextAlignmentOptions.TopLeft:
                return TextAlignmentOptions.TopRight;
            case TextAlignmentOptions.BottomLeft:
                return TextAlignmentOptions.BottomRight;
            case TextAlignmentOptions.Right:
                return TextAlignmentOptions.Left;
            case TextAlignmentOptions.TopRight:
                return TextAlignmentOptions.TopLeft;
            case TextAlignmentOptions.BottomRight:
                return TextAlignmentOptions.BottomLeft;
        }

        return input;
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
