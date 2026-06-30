using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;
using System.Linq;
using TMPro;

public class CountryWindow : MonoBehaviour
{
    public RectTransform window;
    public CanvasGroup buttonCanvasGroup;
    public RectTransform countryParent;
    public CustomScrollRect customScrollRect;
    public CurrentAlphaLetter currentAlphaLetter;
    public TMP_InputField searchField;

    public GameObject countryButtonPrefab;
    public GameObject separatorPrefab;

    private System.Action<Country> onComplete;
    private Country? selectedCountry;

    public delegate void CountrySelectedEvent(Country country);
    public CountrySelectedEvent OnCountrySelected;

    private Tween tween;

    public List<CountryButton> buttons;
    public List<AlfabeticalSeparator> separators;

    public void Show(System.Action<Country> onComplete)
    {
        searchField.text = string.Empty;
        currentAlphaLetter.FindSeparators();

        foreach(CountryButton button in buttons)
            button.gameObject.SetActive(true);

		window.DOKill();
        window.anchoredPosition = new Vector3(0, -400f, 0);

        Canvas.ForceUpdateCanvases();

        window.gameObject.SetActive(true);
        tween = window.DOAnchorPosY(Screen.width <= Screen.height ? 100f : (800f - window.sizeDelta.y) / 2f, 0.3f);

        this.onComplete = onComplete;
        //SpawnCountryButtons();
        selectedCountry = null;
        UpdateConfirmButton();

        ScrollToTop();
    }

    public void Close()
	{
		gameObject.SetActive(false);
        DialogCanvas.instance.FadeOutbackground();
	}

    public void Search(string input)
    {
        input = input.ToLower();

        List<char> enabledChars = new List<char>();

        foreach(CountryButton button in buttons)
        {
            bool isMatch = button.textMeshPro.text.ToLower().Contains(input);
            button.gameObject.SetActive(isMatch);

            if(isMatch)
            {
                char c = button.textMeshPro.text.ToLower()[0];
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

    public void SpawnCountryButtons()
    {
        separators = new List<AlfabeticalSeparator>();
        buttons = new List<CountryButton>();
        char firstChar = '_';

        List<Country> countries = new List<Country>();

        foreach(var country in Enum.GetValues(typeof(Country)))
            countries.Add((Country) country);
        
        countries = countries.OrderBy(x => x.ToString()).ToList();
        
        foreach(Country country in countries)
        {
            if(country.ToString()[0] != firstChar)
            {
                SpawnSeparator(country.ToString()[0]);
                firstChar = country.ToString()[0];
            }

            CountryButton button = Instantiate(countryButtonPrefab, countryParent).GetComponent<CountryButton>();
            button.Intialize(country, this, customScrollRect);

            buttons.Add(button);
        }
    }

    private void SpawnSeparator(char firstChar)
    {
        AlfabeticalSeparator separator = Instantiate(separatorPrefab, countryParent).GetComponent<AlfabeticalSeparator>();
        separator.textMesh.text = firstChar.ToString();
        separator.gameObject.name = firstChar.ToString();
        separators.Add(separator);
    }

    public void SelectCountry(Country country)
    {
        this.selectedCountry = country;
        UpdateConfirmButton();

        OnCountrySelected?.Invoke(country);
    }


    public void ConfirmCountry()
    {
        if(!selectedCountry.HasValue)
            return;

        Close();

        onComplete?.Invoke(selectedCountry.Value);
    }

    private void UpdateConfirmButton()
    {
        buttonCanvasGroup.interactable = selectedCountry.HasValue;
        buttonCanvasGroup.blocksRaycasts = selectedCountry.HasValue;
        buttonCanvasGroup.alpha = selectedCountry.HasValue ? 1f : 0.5f;
    }

    private void ScrollToTop()
    {
        customScrollRect.normalizedPosition = new Vector2(0, 1);
    }
}
