using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;
using System.Linq;
using TMPro;
using UnityEngine.Events;

public class PickerWindow : MonoBehaviour, IOptionSelectedCallbackReciver
{
    public RectTransform containerRect;
    public RectTransform letterAndSearchRect;
    public RectTransform scrollViewRect;

    public GameObject letterAndSearch;

    public void SetSearchVisible(bool visible)
    {
        letterAndSearch.SetActive(visible);

        if(visible)
        {
            letterAndSearchRect.sizeDelta = new Vector2(letterAndSearchRect.sizeDelta.x, 77f);
            //scrollViewRect.SetTop(42);
            containerRect.sizeDelta = new Vector2(containerRect.sizeDelta.x, 340);
        }
        else
        {
            letterAndSearchRect.sizeDelta = new Vector2(letterAndSearchRect.sizeDelta.x, 45f);
            //scrollViewRect.SetTop(10);
            containerRect.sizeDelta = new Vector2(containerRect.sizeDelta.x, 312);
        }
    }

    public RectTransform window;
    public CanvasGroup buttonCanvasGroup;
    public CustomScrollRect customScrollRect;
    public RectTransform optionParent;
    public CurrentAlphaLetter currentAlphaLetter;
    public TMP_InputField searchField;
    public TextMeshProUGUI title;

    public GameObject optionPickerButtonPrefab;
    public GameObject separatorPrefab;

    private System.Action<int> onComplete;
    private Tween tween;
    private int selectectedOption;

    private List<string> options;
    private List<LocalizedOption> localizedOptions;
    private List<AlfabeticalSeparator> separators;
    private Dictionary<string, GameObject> buttons;

    /*
    public delegate void OptionSelectedEvent(int index);
    public OptionSelectedEvent OnOptionSelected;
    */

    private UnityEvent<int> OnOptionSelected = new IntEvent();

    public void Show(TranslationKey translationKey, Language language, List<string> options, bool spawnSeparator, bool search, System.Action<int> onComplete)
    {
        title.Translate(translationKey, language, FontType.Stylized, false);
        SetSearchVisible(search);
        SpawnOptions(options, spawnSeparator);

        currentAlphaLetter.FindSeparators();
        
        this.options = new List<string>(options);
		window.DOKill();
        window.anchoredPosition = new Vector3(0, -400f, 0);

        Canvas.ForceUpdateCanvases();

        window.gameObject.SetActive(true);
        tween = window.DOAnchorPosY(Screen.width <= Screen.height ? 100f : (800f - window.sizeDelta.y) / 2f, 0.3f);

        this.onComplete = onComplete;

        selectectedOption = -1;
        UpdateConfirmButton();

        ScrollToTop();
    }

    public void Show(TranslationKey translationKey, Language language, List<LocalizedOption> options, bool spawnSeparator, bool search, System.Action<int> onComplete)
    {
        title.Translate(translationKey, language, FontType.Stylized, false);
        SetSearchVisible(search);
        SpawnOptions(options, spawnSeparator);

        currentAlphaLetter.FindSeparators();
        
        this.localizedOptions = new List<LocalizedOption>(options);
		window.DOKill();
        window.anchoredPosition = new Vector3(0, -400f, 0);

        Canvas.ForceUpdateCanvases();

        window.gameObject.SetActive(true);
        tween = window.DOAnchorPosY(100f, 0.3f);

        this.onComplete = onComplete;

        selectectedOption = -1;
        UpdateConfirmButton();

        ScrollToTop();
    }

    public void Close()
	{
		gameObject.SetActive(false);
        DialogCanvas.instance.FadeOutbackground();
        Clear();
	}

    private void SpawnOptions(List<string> options, bool spawnSeparator)
    {
        separators = new List<AlfabeticalSeparator>();
        buttons = new Dictionary<string, GameObject>();
        char firstChar = '_';
        
        for(int i = 0; i < options.Count; i++)
        {
            if(spawnSeparator)
            {
                if(options[i][0] != firstChar)
                {
                    SpawnSeparator(options[i][0]);
                    firstChar = options[i][0];
                }
            }

            OptionPickerButton button = Instantiate(optionPickerButtonPrefab, optionParent).GetComponent<OptionPickerButton>();
            button.Intialize(i, options[i], this, customScrollRect);

            buttons.Add(options[i].ToLower(), button.gameObject);
        }
    }

    public void SpawnOptions(List<LocalizedOption> options, bool spawnSeparator)
    {
        separators = new List<AlfabeticalSeparator>();
        buttons = new Dictionary<string, GameObject>();
        char firstChar = '_';
        
        for(int i = 0; i < options.Count; i++)
        {
            if(spawnSeparator)
            {
                if(options[i].label[0] != firstChar)
                {
                    SpawnSeparator(options[i].label[0]);
                    firstChar = options[i].label[0];
                }
            }

            OptionPickerButton button = Instantiate(optionPickerButtonPrefab, optionParent).GetComponent<OptionPickerButton>();
            button.Intialize(i, options[i], this, customScrollRect);

            buttons.Add(options[i].label.ToLower(), button.gameObject);
        }
    }

    public void Search(string input)
    {
        SelectOption(-1);

        input = input.ToLower();

        List<char> enabledChars = new List<char>();
        
        foreach(string option in buttons.Keys)
        {
            bool isMatch = option.Contains(input);
            buttons[option].SetActive(isMatch);

            if(isMatch)
            {
                char c = option.ToLower()[0];
                if(!enabledChars.Contains(c))
                    enabledChars.Add(c);
            }
        }

        foreach(AlfabeticalSeparator separator in separators)
        {
            separator.gameObject.SetActive(input.Length == 0 || enabledChars.Contains(separator.textMesh.text.ToLower()[0]));
        }

        ScrollToTop();
    }

    public void Clear()
    {
        optionParent.transform.DestroyChildren();
        separators = null;
        buttons = null;
        options = null;
        currentAlphaLetter.ClearSeparators();
        searchField.text = string.Empty;
        OnOptionSelected.RemoveAllListeners();
    }

    private void SpawnSeparator(char firstChar)
    {
        AlfabeticalSeparator separator = Instantiate(separatorPrefab, optionParent).GetComponent<AlfabeticalSeparator>();
        separator.textMesh.text = firstChar.ToString();
        separator.gameObject.name = firstChar.ToString();
        separators.Add(separator);
    }

    private void UpdateConfirmButton()
    {
        buttonCanvasGroup.interactable = selectectedOption >= 0;
        buttonCanvasGroup.blocksRaycasts = selectectedOption >= 0;
        buttonCanvasGroup.alpha = selectectedOption >= 0 ? 1f : 0.5f;
    }

    private void ScrollToTop()
    {
        customScrollRect.normalizedPosition = new Vector2(0, 1);
    }

    public void SelectOption(int optionIndex)
    {
        this.selectectedOption = optionIndex;
        UpdateConfirmButton();

        OnOptionSelected?.Invoke(optionIndex);
    }

    public void ConfirmSelection()
    {
        if(selectectedOption < 0)
            return;

        Close();

        onComplete?.Invoke(selectectedOption);
    }

    public void RegisterOnOptionSelectedListener(UnityAction<int> optionSelected)
    {
        OnOptionSelected.AddListener(optionSelected);
    }
}
