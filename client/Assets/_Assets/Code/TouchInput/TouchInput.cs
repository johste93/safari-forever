using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchInput : MonoBehaviour
{   
    public LayerMask whatIsPickable;
    public static LayerMask WhatIsPickable;

    public delegate void TouchEvent(TouchInfo touchInfo);
    public static TouchEvent On_TouchStart;
    public static TouchEvent On_TouchMaintained;
    public static TouchEvent On_TouchEnd;


    private static Dictionary<int, TouchInfo> touches = new Dictionary<int, TouchInfo>();
    private static bool touchCanceled;

    private void Awake()
    {
        WhatIsPickable = whatIsPickable;
    }

    private void Update()
    {
        if(touchCanceled)
        {
            touchCanceled = false;
            return;
        }

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
        if(Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            SimulateTouch(TouchPhase.Began);

        if(Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space))
            SimulateTouch(TouchPhase.Moved);

        if(Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Space))
            SimulateTouch(TouchPhase.Ended);
#else
        if (Input.touchCount > 0)
        {
            for (var i = 0; i < Input.touchCount; i++) 
                HandleTouch(Input.GetTouch(i));
        }
#endif
    }

    private void SimulateTouch(TouchPhase phase)
    {
        Touch simulatedTouch = new Touch();
        simulatedTouch.fingerId = 0;
        simulatedTouch.position = Input.mousePosition;
        simulatedTouch.phase = phase;
        HandleTouch(simulatedTouch);
    }

    private void HandleTouch(Touch touch)
    {
        if(!touches.ContainsKey(touch.fingerId))
        {
            //Register new touch
            TouchInfo newTouchInfo = new TouchInfo(touch.fingerId);
            newTouchInfo.viewportStartPosition = new Vector2( touch.position.x/Screen.width, touch.position.y/Screen.height);
            touches.Add(touch.fingerId, newTouchInfo);
        }
        
        TouchInfo touchInfo = touches[touch.fingerId];

        touchInfo.screenPosition = touch.position;
        touchInfo.viewportPosition = new Vector2( touch.position.x/Screen.width, touch.position.y/Screen.height);
        touchInfo.phase = touch.phase;
        touchInfo.duration += Time.deltaTime;

        touchInfo.pickedUIElement = touchInfo.GetFirstPickedUIElement();

        touches[touch.fingerId] = touchInfo;

        UpdateEvents(touchInfo);
    }

    private static void UpdateEvents(TouchInfo touchInfo)
    {
        touchInfo.ResetCache();
        switch(touchInfo.phase)
        {
            case TouchPhase.Began:
                if(On_TouchStart != null)
                    On_TouchStart(touchInfo);
            break;
            case TouchPhase.Moved:
            case TouchPhase.Stationary:
                if(On_TouchMaintained != null)
                    On_TouchMaintained(touchInfo);
            break;
            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                if(On_TouchEnd != null)
                    On_TouchEnd(touchInfo);

                touches.Remove(touchInfo.fingerIndex);
            break;
        }
    }

    public static void CancelTouch()
    {
        touchCanceled = true;

        Dictionary<int, TouchInfo> dictionary = new Dictionary<int, TouchInfo>(touches);
        foreach(KeyValuePair<int, TouchInfo> kVP in dictionary)
        {
            kVP.Value.phase = TouchPhase.Canceled;
            UpdateEvents(kVP.Value);
        }

        touches = new Dictionary<int, TouchInfo>();
    }

    public static int GetTouchCount()
    {
        return touches.Count;
    }
}
