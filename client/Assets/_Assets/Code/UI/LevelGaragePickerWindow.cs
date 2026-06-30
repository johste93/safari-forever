using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;
using System.Linq;
using TMPro;
using UnityEngine.Events;

public class LevelGaragePickerWindow : MonoBehaviour, IOptionSelectedCallbackReciver
{
    public RectTransform window;
    public CanvasGroup buttonCanvasGroup;
    public CustomScrollRect customScrollRect;

    public RectTransform optionParent;
    public GameObject optionPickerButtonPrefab;

    private Dictionary<string, GameObject> buttons;
    private List<string> options;
    private int? selectedLevel;

    private System.Action<int> onComplete;
    private System.Action<int> onDelete;
    private Tween tween;

    private UnityEvent<int> OnOptionSelected = new IntEvent();

    public void Show(List<string> options, System.Action<int> onComplete, System.Action<int> onDelete)
    {
        SpawnOptions(options);
        
        this.options = new List<string>(options);
		window.DOKill();
        window.anchoredPosition = new Vector3(0, -400f, 0);

        Canvas.ForceUpdateCanvases();

        window.gameObject.SetActive(true);
        tween = window.DOAnchorPosY(Screen.width <= Screen.height ? 100f : (800f - window.sizeDelta.y) / 2f, 0.3f);

        this.onComplete = onComplete;
        this.onDelete = onDelete;

        UpdateConfirmButton();

        ScrollToTop();
    }

    public void SelectOption(int selectedLevel)
    {
        this.selectedLevel = selectedLevel;
        UpdateConfirmButton();

        OnOptionSelected?.Invoke(this.selectedLevel.Value);
    }

    public void ConfirmSelection()
    {
        if(!selectedLevel.HasValue)
            return;

        Close();

        onComplete?.Invoke(selectedLevel.Value);
    }

    public void DeleteLevel()
    {
        if(!selectedLevel.HasValue)
            return;

        Close();

        onDelete?.Invoke(selectedLevel.Value);
    }

    public void Close()
	{
		gameObject.SetActive(false);
        DialogCanvas.instance.FadeOutbackground();
        Clear();
	}

    public void Clear()
    {
        optionParent.transform.DestroyChildren();
        buttons = null;
        options = null;
        OnOptionSelected.RemoveAllListeners();
    }

    private void SpawnOptions(List<string> options)
    {
        buttons = new Dictionary<string, GameObject>();
        
        for(int i = 0; i < options.Count; i++)
        {
            OptionPickerButton button = Instantiate(optionPickerButtonPrefab, optionParent).GetComponent<OptionPickerButton>();
            button.Intialize(i, options[i], this, customScrollRect);

            buttons.Add(options[i].ToLower(), button.gameObject);
        }
    }

    private void UpdateConfirmButton()
    {
        buttonCanvasGroup.interactable = selectedLevel.HasValue;
        buttonCanvasGroup.blocksRaycasts = selectedLevel.HasValue;
        buttonCanvasGroup.alpha = selectedLevel.HasValue ? 1f : 0.5f;
    }

    private void ScrollToTop()
    {
        customScrollRect.normalizedPosition = new Vector2(0, 1);
    }

    public void RegisterOnOptionSelectedListener(UnityAction<int> optionSelected)
    {
        OnOptionSelected.AddListener(optionSelected);
    }
}
