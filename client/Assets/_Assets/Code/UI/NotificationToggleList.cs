using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationToggleList : MonoBehaviour
{
    public CustomScrollRect scrollRect;
    public Image scrollBarHandle;
    private Color scrollBarHandleColor;

    private void Awake()
    {
        scrollBarHandleColor = scrollBarHandle.color;
    }

    private void Update()
    {
        if(scrollRect.m_Dragging)
            scrollBarHandleColor.a = Mathf.Clamp(scrollBarHandleColor.a + (Time.deltaTime*2f), 0f, 0.5f);
        else
            scrollBarHandleColor.a = Mathf.Clamp01(scrollBarHandleColor.a - Time.deltaTime);

        scrollBarHandle.color = scrollBarHandleColor;
    }
}
