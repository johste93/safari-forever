using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

public class PushToggle : MonoBehaviour
{
    public RectTransform handle;

    private bool value;

    public Image image;
    public Color deactive;
    public Color active;

    private Tween tween;
    private Tween colorTween;

    public BoolEvent OnClickEvent;

    public void OnClick()
    {
        SetValue(!value);
        OnClickEvent?.Invoke(value);
    }

    public void SetValue(bool value, bool skipAnimation = false)
    {
        this.value = value;

        Ease ease = Ease.OutQuad;
        float duration = skipAnimation ? 0f : 0.15f;

        tween?.Kill();
        tween = handle.DOAnchorPos(new Vector2( 16.5f * (this.value ? 1 : -1) ,0), duration).SetEase(ease);
        colorTween?.Kill();
        colorTween = image.DOColor(this.value ? active : deactive, duration).SetEase(ease);
    }

    public bool GetValue()
    {
        return this.value;
    }

    private void OnDisable()
    {
        KillAllTweens();
    }

    private void KillAllTweens()
    {
        tween?.Kill();
    }
}
