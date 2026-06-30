using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.Events;

public class Crank : MonoBehaviour
{
    [SerializeField]
    public UnityEvent OnCrank;

    public bool interactable = true;
    public Slider slider;

    public RectTransform Handle;
    public RectTransform Ball;
    public RectTransform TopShaft;
    public RectTransform BottomShaft;

    private RectTransform myRect; 
    private bool isDragging;
    private float speed = 2f;
    private float handleMinSize = 1f;
    private float handleMaxSize = 1.3f;
    private Tween tween;

    public const float MaxValueWhenLocked = 0.6f;

    private void OnEnable()
    {
        myRect = GetComponent<RectTransform>();
        UpdateCrank();
        isDragging = false;
        slider.value = 0f;
    }

    private void Update()
    {
        if(isDragging)
        {
            if(!interactable && slider.value > MaxValueWhenLocked)
                slider.value = MaxValueWhenLocked;

            return;
        }

        slider.value = Mathf.MoveTowards(slider.value, 0f, Time.deltaTime * speed);
    }

    public void OnPointerDown(BaseEventData data)
    {
        isDragging = true;

        //tween?.Complete();
        //tween = Handle.DOPunchScale(Vector3.one * 0.5f, 0.3f, 1);
    }

    public void OnPointerUp(BaseEventData data)
    {
        isDragging = false;

        if(slider.value > MaxValueWhenLocked -0.01f)
        {
            OnCrank?.Invoke();
        }
    }

    public void OnValueChanged(float value)
    {
        UpdateCrank();
    }

    private void UpdateCrank()
    {
        float topNormalizedPosition = 1f - (Mathf.Min(slider.value, 0.5f)/0.5f);
        float bottomNormalizedPosition = (Mathf.Max(slider.value, 0.5f)-0.5f)/0.5f;
        
        float halfHeight = (myRect.sizeDelta.y + Handle.sizeDelta.y) * 0.25f;
        TopShaft.sizeDelta = new Vector2(TopShaft.sizeDelta.x, topNormalizedPosition * halfHeight);
        BottomShaft.sizeDelta = new Vector2(BottomShaft.sizeDelta.x, bottomNormalizedPosition * halfHeight);

        float handleSizeNormalized = 1f - Mathf.Abs(0.5f - slider.value)/0.5f;

        float size = Mathf.Lerp(handleMinSize, handleMaxSize, handleSizeNormalized);
        //Ball.sizeDelta = new Vector2(size, size);
        Ball.localScale = new Vector3(size, size, 1);
    }

    private void OnDisable()
    {
        KillTweens();
    }

    private void KillTweens()
    {
        if(tween != null)
            tween.Kill();
    }
}
