using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInput : MonoBehaviour
{
    public Image image;

    private static int[] startFrame = new int[11];
    private static int[] endFrame = new int[11];

    public static bool On_TouchStart(int fingerIndex)
    {
        if(fingerIndex >= endFrame.Length)
            return false;

        return startFrame[fingerIndex] == Time.frameCount;
    }

    public static int GetStartFrame(int fingerIndex)
    {
        if(fingerIndex >= endFrame.Length)
            return -1;

        return startFrame[fingerIndex];
    }

    public static bool On_TouchMaintained(int fingerIndex)
    {
        if(fingerIndex >= endFrame.Length)
            return false;

        return startFrame[fingerIndex] > endFrame[fingerIndex];
    }

    public static bool On_TouchEnd(int fingerIndex)
    {
        if(fingerIndex >= endFrame.Length)
            return false;

        return endFrame[fingerIndex] == Time.frameCount;
    }

    private void TouchStart(TouchInfo touch)
    {
        if(touch.pickedUIElement != image.gameObject)
            return;
        
        if(touch.fingerIndex >= endFrame.Length)
            return;

        startFrame[touch.fingerIndex] = Time.frameCount;
    }

    private void OnTouchMaintained(TouchInfo touch)
    {
         if(touch.pickedUIElement != image.gameObject)
            return;
    }

    private void OnTouchEnd(TouchInfo touch)
    {
         if(touch.pickedUIElement != image.gameObject)
            return;

        if(touch.fingerIndex >= endFrame.Length)
            return;

        endFrame[touch.fingerIndex] = Time.frameCount;
    }

    private void On_EnterPlayMode()
    {
        image.enabled = true;
        Reset();
    }

    private void On_ExitPlayMode()
    {
        image.enabled = false;
        Reset();
    }

    private void Reset()
    {
        startFrame = new int[11];
        endFrame = new int[11];
    }

    private void On_LevelReset(bool manual) => Reset();

    private void OnEnable()
    {
        GameMaster.On_EnterPlayMode += On_EnterPlayMode;
        GameMaster.On_ExitPlayMode += On_ExitPlayMode;
        GameMaster.On_LevelReset += On_LevelReset;

        TouchInput.On_TouchStart += TouchStart;
        TouchInput.On_TouchMaintained += OnTouchMaintained;
        TouchInput.On_TouchEnd += OnTouchEnd;
    }

    private void Unsubscribe()
    {
        GameMaster.On_EnterPlayMode -= On_EnterPlayMode;
        GameMaster.On_ExitPlayMode -= On_ExitPlayMode;
        GameMaster.On_LevelReset -= On_LevelReset;

        TouchInput.On_TouchStart -= TouchStart;
        TouchInput.On_TouchMaintained -= OnTouchMaintained;
        TouchInput.On_TouchEnd -= OnTouchEnd;
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }
}
