using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SuspensionManager
{
    public delegate void SuspensionEvent(bool suspend);
    public static SuspensionEvent On_SuspensionEvent;

    private static bool isSupsended;

    public static bool IsSuspended()
    {
        return isSupsended;
    }

    public static void Suspend(bool suspend)
    {
        isSupsended = suspend;

        Animator[] animatorsInScene = GameObject.FindObjectsOfType<Animator>();
        foreach(Animator animator in animatorsInScene)
        {
            animator.speed = suspend ? 0 : 1f;
        }

        if(On_SuspensionEvent != null)
            On_SuspensionEvent(suspend);
    }
}
