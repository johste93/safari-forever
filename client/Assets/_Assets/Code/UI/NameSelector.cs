using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class NameSelector : MonoBehaviour
{ 
    public TextMeshProUGUI[] nameSegments;
    public string levelName;

    private const int numberOfOptions = 50;
    private List<Tween> tweens;
    private bool editable;

    public void Editable(bool editable)
    {
        this.editable = editable;
        if(editable)
        {
            if(tweens != null)
            {
                //Debug.LogError("Already blinking!");
                return;
            }
            tweens = new List<Tween>();
            foreach(TextMeshProUGUI textMesh in nameSegments)
            {
                textMesh.color = new Color(0.75f, 0.75f, 0.75f, 1f);
                textMesh.isRightToLeftText = false;
                textMesh.font = Globals.accessibilityConstants.GetFont(SaveManager.currentSave.openDyslexic, FontType.Stylized_Outlined, textMesh.font, SaveManager.currentSave.language);
                tweens.Add(textMesh.DOColor(Color.white, 0.5f).SetEase(Ease.InQuad).SetLoops(-1, LoopType.Yoyo));
            }
        }
        else
        {
            Reset();
        }
    }

    public void OnClickReroll(int index)
    {
        if(!editable)
            return;

        if(index == 0)
        {
            PickAdjective();
        }
        else
        {
            PickNoun();
        }
        //nameSegments[index].text = NameGenerator.RerollIndex(index);

        levelName = $"{nameSegments[0].text} {nameSegments[1].text}";
    }

    private void PickAdjective()
    {
        //Get 100 random adjectives.
        List<string> randomOptions = new List<string>();
        randomOptions.Add(nameSegments[0].text);
        while(randomOptions.Count < numberOfOptions-1)
        {
            string option = NameGenerator.RerollIndex(0).FirstLetterToUpper();

            if(!randomOptions.Contains(option))
                randomOptions.Add(option);
        }

        randomOptions.Sort();

        Language language = Localization.KeyAvailable(TranslationKey.Editor_PickAdjective, SaveManager.currentSave.language) ? SaveManager.currentSave.language : Globals.localizationConstants.defaultLanguage;

        DialogCanvas.instance.ShowPickerWindow(TranslationKey.Editor_PickAdjective, language, randomOptions, true, false, (selectedIndex)=>{
            nameSegments[0].text = randomOptions[selectedIndex];
            levelName = $"{nameSegments[0].text} {nameSegments[1].text}";
        });

        int startIndex = randomOptions.IndexOf(nameSegments[0].text);
        DialogCanvas.instance.pickerWindow.SelectOption(startIndex);
    }

    private void PickNoun()
    {
        //Get 100 random nouns.
        List<string> randomOptions = new List<string>();
        randomOptions.Add(nameSegments[1].text);
        while(randomOptions.Count < numberOfOptions-1)
        {
            string option = NameGenerator.RerollIndex(1).FirstLetterToUpper();

            if(!randomOptions.Contains(option))
                randomOptions.Add(option);
        }

        randomOptions.Sort();

        //USE THIS WHEN CHANING TO TRANSLATION KEY
        Language language = Localization.KeyAvailable(TranslationKey.Editor_PickNoun , SaveManager.currentSave.language) ? SaveManager.currentSave.language : Globals.localizationConstants.defaultLanguage;

        DialogCanvas.instance.ShowPickerWindow(TranslationKey.Editor_PickNoun, language, randomOptions, true, false, (selectedIndex)=>{
            nameSegments[1].text = randomOptions[selectedIndex];
            levelName = $"{nameSegments[0].text} {nameSegments[1].text}";
        });

        int startIndex = randomOptions.IndexOf(nameSegments[1].text);
        DialogCanvas.instance.pickerWindow.SelectOption(startIndex);
    }
    
    private void OnEnable()
    {
        string[] name = NameGenerator.RerollAll();
        for(int i = 0; i < 2; i++)
        {
            nameSegments[i].text = name[i];
        }
        levelName = $"{nameSegments[0].text} {nameSegments[1].text}";
    }

    private void Reset()
    {
        if(tweens == null)
            return;

        foreach(Tween tween in tweens)
        {
            tween.OnStepComplete(()=>{
                tween.Kill();
                ((TextMeshProUGUI)tween.target).color = Color.white;
            });
        }
        tweens = null;
    }

	private void OnDestroy()
	{
		KillAllTweens();
	}

	private void KillAllTweens()
	{
        if(tweens == null)
            return;
            
		foreach(Tween tween in tweens)
        {
			if(tween != null)
				tween.Kill();
		}
		tweens = null;
	}
}
