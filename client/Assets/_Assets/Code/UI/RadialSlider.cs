using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadialSlider : MonoBehaviour
{
    public float value{
        get{
            return _value;
        }
        set{
            _value = value;
            onValueChanged?.Invoke(_value);
        }
    }
    private float _value = 0f;

    public Vector2 constraints = new Vector2(0f, 1f);

    public bool stepped;
    public int steps = 30;

    public RectTransform thisRect;
    public RectTransform colorRing;
    public RectTransform handle;

    public float distanceFromCenter = 75f;

    private int finderIndex = -1;
    private Vector2 offset;
    private float targetValue;

    public delegate void ValueChanged(float value);
    public ValueChanged onValueChanged;

    private void LateUpdate()
    {
        float rad = value * 360f * Mathf.Deg2Rad;
        Vector2 dir = new Vector2(Mathf.Sin(rad), Mathf.Cos(rad));
        handle.anchoredPosition = dir * distanceFromCenter;
    }

    private void On_TouchStart(TouchInfo touchInfo)
    {   
        if(touchInfo.pickedUIElement != handle.gameObject)
            return;

        finderIndex = touchInfo.fingerIndex;
        Vector2 touchedWorldPos = new Vector2(touchInfo.viewportPosition.x * Screen.width, touchInfo.viewportPosition.y * Screen.height);
        offset = (Vector2)handle.position - touchedWorldPos;
    }

    private void On_TouchMaintained(TouchInfo touchInfo)
    {
        if(touchInfo.fingerIndex != finderIndex)
            return;
    
        Vector2 touchedWorldPos = new Vector2(touchInfo.viewportPosition.x * Screen.width, touchInfo.viewportPosition.y * Screen.height) + offset;

        Vector2 dir = touchedWorldPos - (Vector2)colorRing.transform.position;
        dir.Normalize();

        targetValue = dir.ToAngle()/360f;
        targetValue = targetValue - Mathf.FloorToInt(targetValue);

        if(stepped)
            targetValue = Mathf.RoundToInt(targetValue * steps) / (float)steps;

        value = Mathf.Clamp(targetValue, constraints.x, constraints.y);
    }

    private void On_TouchEnd(TouchInfo touchInfo)
    {
        if(touchInfo.fingerIndex != finderIndex)
            return;
        
        finderIndex = -1;
    }

    private void OnEnable()
    {
        TouchInput.On_TouchStart += On_TouchStart;
        TouchInput.On_TouchMaintained += On_TouchMaintained;
        TouchInput.On_TouchEnd += On_TouchEnd;
    }

    private void Unsubscribe()
    {
        TouchInput.On_TouchStart -= On_TouchStart;
        TouchInput.On_TouchMaintained -= On_TouchMaintained;
        TouchInput.On_TouchEnd -= On_TouchEnd;

        finderIndex = -1;
    }

    private void OnDisable()
    {
        Unsubscribe();
    }
}
