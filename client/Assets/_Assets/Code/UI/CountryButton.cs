using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class CountryButton : UIToggleButton, IBeginDragHandler,  IDragHandler, IEndDragHandler, IScrollHandler
{
    public TextMeshProUGUI textMeshPro;

    public Country country;
    public CountryWindow parentWindow;
    public CustomScrollRect scrollRect;

    private float timePressed;

    private Color defaultColor
    {
        get { return new Color(0.2941176f, 0.2941176f, 0.2941176f, 1f); }
    }

    public void Intialize(Country country, CountryWindow parentWindow, CustomScrollRect scrollRect)
    {
        base.Initalize();

        surfaceImage.color = defaultColor;
        textMeshPro.color = Color.white;

        this.country = country;
        this.parentWindow = parentWindow;

        textMeshPro.text = country.ToString().Replace("_", " ");

        gameObject.name = textMeshPro.text;

        this.scrollRect = scrollRect;
    }

    private void OnCountrySelected(Country country)
    {
        if(country == this.country)
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

        parentWindow.SelectCountry(country);
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

    private void OnEnable()
    {
        this.parentWindow.OnCountrySelected += OnCountrySelected;
    }

    private void Unsubscribe()
    {
        this.parentWindow.OnCountrySelected -= OnCountrySelected;
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();
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
