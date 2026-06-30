using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class OptionPickerButton : UIToggleButton, IBeginDragHandler,  IDragHandler, IEndDragHandler, IScrollHandler
{
    public TextMeshProUGUI textMeshPro;

    private int optionIndex;
    private IOptionSelectedCallbackReciver callbackReciver;
    private CustomScrollRect scrollRect;

    private float timePressed;

    private Color defaultColor
    {
        get { return new Color(0.2941176f, 0.2941176f, 0.2941176f, 1f); }
    }

    public void Intialize(int index, string label, IOptionSelectedCallbackReciver callbackReciver, CustomScrollRect scrollRect)
    {
        base.Initalize();

        surfaceImage.color = defaultColor;
        textMeshPro.color = Color.white;

        this.optionIndex = index;
        this.callbackReciver = callbackReciver;

        textMeshPro.text = label;

        gameObject.name = textMeshPro.text;

        this.scrollRect = scrollRect;

        callbackReciver.RegisterOnOptionSelectedListener(OnOptionSelected);
    }

    public void Intialize(int index, LocalizedOption localizedOption, IOptionSelectedCallbackReciver callbackReciver, CustomScrollRect scrollRect)
    {
        base.Initalize();

        surfaceImage.color = defaultColor;
        textMeshPro.color = Color.white;

        this.optionIndex = index;
        this.callbackReciver = callbackReciver;

        textMeshPro.text = localizedOption.label;
        textMeshPro.isRightToLeftText = Localization.IsRightToLeftLanguage(localizedOption.language);
        textMeshPro.font = Globals.accessibilityConstants.GetFont(SaveManager.currentSave.openDyslexic, FontType.Regular, textMeshPro.font, localizedOption.language);

        gameObject.name = textMeshPro.text;

        this.scrollRect = scrollRect;

        callbackReciver.RegisterOnOptionSelectedListener(OnOptionSelected);
    }

    private void OnOptionSelected(int optionSelected)
    {
        if(this.optionIndex == optionSelected)
            return;

        Release();
    }

    protected override void Subscribe()
    {
        EventTrigger eT = GetComponent<EventTrigger>();

        eT.AddEventTriggerListener(EventTriggerType.PointerUp, PointerUp);
        eT.AddEventTriggerListener(EventTriggerType.PointerDown, PointerDown);
    }

    private void PointerDown(BaseEventData eventData)
    {
        if(Time.time - timePressed <= 1f)
            return;

        timePressed = Time.time;
    }

    private void PointerUp(BaseEventData eventData)
    {
        if(Time.time - timePressed > 1f)
            return;

        Audio.Play(SFX.instance.ui.tallButtonDown, Channel.UI);
        Press();

        callbackReciver.SelectOption(optionIndex);
    }
 
    public void OnBeginDrag(PointerEventData eventData)
    {
        scrollRect.OnBeginDrag(eventData);
        timePressed = 0f;
    }
 
    public void OnDrag(PointerEventData eventData)
    {
        scrollRect.OnDrag(eventData);
    }
 
    public void OnEndDrag(PointerEventData eventData)
    {
        scrollRect.OnEndDrag(eventData);
    }
 
    public void OnScroll(PointerEventData data)
    {
        scrollRect.OnScroll(data);
    }

    public override void Press()
    {
        //base.Press();

        surfaceImage.color = Color.white;

        textMeshPro.color = defaultColor;
    }

    public override void Release()
    {
        base.Release();

        surfaceImage.color = defaultColor;

        textMeshPro.color = Color.white;
    }
}
