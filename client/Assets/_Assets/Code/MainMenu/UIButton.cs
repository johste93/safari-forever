using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventTrigger))]
public class UIButton : MonoBehaviour
{   
    public RectTransform surface;
    public RectTransform shadow;
    private Button button;

    public Image surfaceImage;

    public float topMargin = 3;
    private float buttomMargin;

    public delegate void ButtonEvent();
    public ButtonEvent On_Click;

    public List<UIButton> ExclusiveButtons;

    private float defaultVibrance;
    private float pressedVibrance;

    protected bool isPressed;
    protected bool isInteractable = true;

    protected bool hasInitalized;
    protected virtual void Initalize()
    {
        if (hasInitalized)
            return;

        buttomMargin = surface.offsetMin.y;
        surfaceImage = surface.GetComponent<Image>();

        if (surface.GetComponent<Image>())
        {
            defaultVibrance = surfaceImage.color.GetVibrance();
            pressedVibrance = defaultVibrance - 0.15f;
        }

        button = GetComponent<Button>();

        Subscribe();
        hasInitalized = true;
    }


    protected virtual void Subscribe()
    {
        EventTrigger eT = GetComponent<EventTrigger>();
        eT.AddEventTriggerListener(EventTriggerType.PointerDown, PointerDown);
        eT.AddEventTriggerListener(EventTriggerType.PointerUp, PointerUp);
        eT.AddEventTriggerListener(EventTriggerType.PointerExit, PointerExit);
    }

    private void PointerDown(BaseEventData eventData)
    {
        if(!isInteractable)
            return;

        Audio.Play(SFX.instance.ui.tallButtonDown, Channel.UI);
        Press();
    }

    private void PointerUp(BaseEventData eventData)
    {
        if(!isInteractable)
            return;

        Release();
        Audio.Play(SFX.instance.ui.tallButtonUp, Channel.UI);
    }

    private void PointerExit(BaseEventData eventData)
    {
        if(!isInteractable)
            return;

        Release();
    }

    public bool IsPressed()
    {
        return isPressed;
    }

    public virtual void Press()
    {
        Initalize();

        isPressed = true;

        surface.offsetMin = new Vector2(surface.offsetMin.x, topMargin);
        surface.offsetMax = new Vector2(surface.offsetMax.x, -buttomMargin + topMargin);

        shadow.offsetMin = new Vector2(surface.offsetMin.x, 0);
        shadow.offsetMax = new Vector2(surface.offsetMax.x, -buttomMargin + topMargin);
        
        if(surfaceImage)
            surfaceImage.color = surfaceImage.color.SetVibrance(pressedVibrance);

        foreach(UIButton btn in ExclusiveButtons)
        {
            if(btn.isPressed)
                btn.Release();
        }
    }

    public virtual void Release()
    {
        Initalize();

        isPressed = false;

        surface.offsetMin = new Vector2(surface.offsetMin.x, buttomMargin);
        surface.offsetMax = new Vector2(surface.offsetMax.x, 0);

        shadow.offsetMin = new Vector2(surface.offsetMin.x, 0);
        shadow.offsetMax = new Vector2(surface.offsetMax.x, 0);

        if(surfaceImage)
            surfaceImage.color = surfaceImage.color.SetVibrance(defaultVibrance);
    }

    public void SetInteractable(bool interactable)
    {
        isInteractable = interactable;

        if(isInteractable)
        {
            Release();
        }
        else
        {
            Press();
        }

        if(button != null)
            button.enabled = interactable;
    }

    private void Awake()
    {
        Initalize();
    }

    private void OnDisable()
    {
        //Reset
        Release();
    }
}
