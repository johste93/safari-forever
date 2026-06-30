using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.Linq;
using TMPro;

public static class Localization
{
    private const string PATH = "LocalizedText/";
    
    private static Dictionary<string, string> dictionary;
    private static Language? loadedLanguage;

    private static string[] textKeys;
    public static string[] TextKeys
    {
        get {
            if(textKeys == null)
                LoadDictionary(Globals.localizationConstants.defaultLanguage);

            return textKeys;
        }
        private set {
            textKeys = value;
        }
    }

    public static void ReloadDictionary()
    {
        if(!loadedLanguage.HasValue)
        {
            Debug.LogError("No language loaded");
            return;
        }
        textKeys = null;
        LoadDictionary(loadedLanguage.Value);
    }

    public static void Translate(this TextMeshPro textMesh, TranslationKey key, Language language, FontType fontType, bool flipAlignment)
    {
        Language lang = Globals.localizationConstants.defaultLanguage;
        if(KeyAvailable(key, language))
        {
            lang = language;
        }

        textMesh.isRightToLeftText = IsRightToLeftLanguage(lang);
        textMesh.font = Globals.accessibilityConstants.GetFont(SaveManager.currentSave.openDyslexic, fontType, textMesh.font, language);
        textMesh.text = GetTranslation2(key, lang);
        UpdateTextAlignment(textMesh, language, flipAlignment);
    }

    /*
    public static void TranslateFormat(this TextMeshPro textMesh, TranslationKey key, Language language, FontType fontType, bool flipAlignment, params object[] args)
    {
        Language lang = Globals.localizationConstants.defaultLanguage;
        if(KeyAvailable(key, language))
        {
            lang = language;
        }

        textMesh.isRightToLeftText = IsRightToLeftLanguage(lang);
        textMesh.font = Globals.accessibilityConstants.GetFont(SaveManager.currentSave.openDyslexic, fontType, textMesh.font, language);
        UpdateTextAlignment(textMesh, flipAlignment);

        if(textMesh.isRightToLeftText)
            textMesh.text = LTRTags(string.Format(dictionary[key.ToString()], args));
        else
            textMesh.text = string.Format(dictionary[key.ToString()], args);
    }
    */


    public static void Translate(this TextMeshProUGUI textMesh, TranslationKey key, Language language, FontType fontType, bool flipAlignment)
    {
        if(!KeyAvailable(key, language))
        {
            if(language == Globals.localizationConstants.defaultLanguage)
            {
                Debug.LogError($"Key: {key.ToString()} not found!");
                return;
            }

            textMesh.Translate(key, Globals.localizationConstants.defaultLanguage, fontType, flipAlignment);
            return;
        }

        textMesh.isRightToLeftText = IsRightToLeftLanguage(language);
        textMesh.font = Globals.accessibilityConstants.GetFont(SaveManager.currentSave.openDyslexic, fontType, textMesh.font, language);
        textMesh.text = GetTranslation2(key, language);
        UpdateTextAlignment(textMesh, language, flipAlignment);
    }

    public static void TranslateFormat(this TextMeshProUGUI textMesh, TranslationKey key, Language language, FontType fontType, bool flipAlignment, params object[] args)
    {
        if(!KeyAvailable(key, language))
        {
            if(language == Globals.localizationConstants.defaultLanguage)
            {
                Debug.LogError($"Key: {key.ToString()} not found!");
                return;
            }

            textMesh.TranslateFormat(key, Globals.localizationConstants.defaultLanguage, fontType, flipAlignment, args);
            return;
        }

        textMesh.isRightToLeftText = IsRightToLeftLanguage(language);
        textMesh.font = Globals.accessibilityConstants.GetFont(SaveManager.currentSave.openDyslexic, fontType, textMesh.font, language);
        UpdateTextAlignment(textMesh, language, flipAlignment);

        if(textMesh.isRightToLeftText)
            textMesh.text = HandleLTRTags(string.Format(dictionary[key.ToString()], args));
        else
            textMesh.text = string.Format(dictionary[key.ToString()], args);
    }

    public static bool KeyAvailable(TranslationKey key, Language language)
    {
        if(!loadedLanguage.HasValue || loadedLanguage != language)
        {
            LoadDictionary(language);
        }

        if(!dictionary.ContainsKey(key.ToString()))
            return false;

        return !string.IsNullOrWhiteSpace(dictionary[key.ToString()]);
    }

    public static string GetTranslation2(TranslationKey key, Language language)
    {
        if(!KeyAvailable(key, language))
        {
            if(language == Globals.localizationConstants.defaultLanguage)
            {
                Debug.LogError($"Key: {key.ToString()} not found!");
                return "";
            }

            return GetTranslation2(key, Globals.localizationConstants.defaultLanguage);
        }

        if(IsRightToLeftLanguage(language))
        {    
            return HandleLTRTags(dictionary[key.ToString()]);
        }
        else
        {
            return dictionary[key.ToString()];
        }
    }

    public static string GetTranslationFormat2(TranslationKey key, Language language, params object[] args)
    {
        if(!KeyAvailable(key, language))
        {
            if(language == Globals.localizationConstants.defaultLanguage)
            {
                Debug.LogError($"Key: {key.ToString()} not found!");
                return "";
            }

            return GetTranslationFormat2(key, Globals.localizationConstants.defaultLanguage, args);
        }

        if(IsRightToLeftLanguage(language))
        {
            return HandleLTRTags(string.Format(dictionary[key.ToString()], args));
        }
        else
        {
            return string.Format(dictionary[key.ToString()], args);
        }
    }

    public static bool IsRightToLeftLanguage(Language language)
    {
        switch(language)
        {
            default:
                return false;
            case Language.Hebrew:
                return true;
        }
    }

    //Does not handle input that contains {0}, {1} ect...
    private static string HandleLTRTags(string input)
    {
        MatchCollection matches = Regex.Matches(input, "<ltr>(.*?)</ltr>");

        if(matches.Count == 0)
            return input;

        string result = input;

        List<string> values = new List<string>();

        for(int i = 0; i < matches.Count; i++)
        {
            string value = matches[i].Groups[1].Value;
            values.Add(ReverseString(value));
            result = result.Replace(value, "{" + i + "}");
        }

        //Remove tags
        result = result.Replace("<ltr>","");
        result = result.Replace("</ltr>","");

        //Reinsert values
        return string.Format(result, values.ToArray());
    }

    private struct Inserts
    {
        public string value;
        public int startIndex;
    }

    public static string ReverseString(string s)
    {
        char[] arr = s.ToCharArray();
        Array.Reverse(arr);
        return new string(arr);
    }

    private static void LoadDictionary(Language language)
    {
        string pathToFile = PATH + GetFileNameByLanguage(language);

        TextAsset textAsset = Resources.Load<TextAsset>(pathToFile);
        dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(textAsset.ToString());

        loadedLanguage = language;
        //Debug.Log("loaded Language: " + language);

        textKeys = dictionary.Keys.ToArray();
    }

    public static void UpdateTextAlignment(TextMeshProUGUI textMesh, Language language, bool flipAlignment)
    {
        if(!flipAlignment)
            return;
        
        if( (textMesh.alignment == TextAlignmentOptions.Left 
            || textMesh.alignment == TextAlignmentOptions.TopLeft
            || textMesh.alignment == TextAlignmentOptions.BottomLeft) && Localization.IsRightToLeftLanguage(language))
                textMesh.alignment = FlipHorizontally(textMesh.alignment);

        if( (textMesh.alignment == TextAlignmentOptions.Right 
            || textMesh.alignment == TextAlignmentOptions.TopRight
            || textMesh.alignment == TextAlignmentOptions.BottomRight) && !Localization.IsRightToLeftLanguage(language))
                textMesh.alignment = FlipHorizontally(textMesh.alignment);
    }

    private static void UpdateTextAlignment(TextMeshPro textMesh, Language language, bool flipAlignment)
    {
        if(!flipAlignment)
            return;
        
        if( (textMesh.alignment == TextAlignmentOptions.Left 
            || textMesh.alignment == TextAlignmentOptions.TopLeft
            || textMesh.alignment == TextAlignmentOptions.BottomLeft) && Localization.IsRightToLeftLanguage(language))
                textMesh.alignment = FlipHorizontally(textMesh.alignment);

        if( (textMesh.alignment == TextAlignmentOptions.Right 
            || textMesh.alignment == TextAlignmentOptions.TopRight
            || textMesh.alignment == TextAlignmentOptions.BottomRight) && !Localization.IsRightToLeftLanguage(language))
                textMesh.alignment = FlipHorizontally(textMesh.alignment);
    }

    private static TextAlignmentOptions FlipHorizontally(TextAlignmentOptions input)
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

    private static string GetFileNameByLanguage(Language language)
    {
        switch(language)
        {
            default:
            case Language.English:
                return "English";
            case Language.Norwegian:
                return "Norwegian";
            case Language.Hebrew:
                return "Hebrew";
            case Language.Euro_Spanish:
                return "Spanish (Spain)";
            case Language.German:
                return "German";
            case Language.French:
                return "French";
            case Language.Greek:
                return "Greek";
            case Language.Danish:
                return "Danish";
            case Language.Dutch:
                return "Dutch";
            case Language.Finnish:
                return "Finnish";
            case Language.Italian:
                return "Italian";
            case Language.Polish:
                return "Polish";
            case Language.Brazilian_Portuguese:
                return "Portuguese (BR)";
            case Language.Euro_Portuguese:
                return "Portuguese (PT)";
            case Language.LatinAmerican_Spanish:
                return "Spanish (Latin America)";
            case Language.Swedish:
                return "Swedish";
            case Language.Russian:
                return "Russian";
            case Language.Vietnamese:
                return "Vietnamese";
            case Language.Afrikaans:
                return "Afrikaans";
            case Language.Bulgarian:
                return "Bulgarian";
            case Language.Canadian_French:
                return "Canadian French";
            case Language.Estonian:
                return "Estonian";
            case Language.Hungarian:
                return "Hungarian";
            case Language.Latvian:
                return "Latvian";
            case Language.Lithuanian:
                return "Lithuanian";
            case Language.Romanian:
                return "Romanian";
            case Language.Serbian:
                return "Serbian";
            case Language.Turkish:
                return "Turkish";
            case Language.Ukrainian:
                return "Ukrainian";
        }
    }
}
