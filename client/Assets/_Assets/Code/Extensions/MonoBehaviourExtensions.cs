using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MonoBehaviourExtensions
{
    public static Coroutine Delay(this MonoBehaviour mono, float duration, System.Action actions)
    {
        return mono.StartCoroutine(DelayRoutine(duration, actions));
    }

    private static IEnumerator DelayRoutine(float duration, System.Action actions)
    {
        yield return new WaitForSeconds(Mathf.Max(0f, duration));
        if(actions != null)
            actions();
    }

    public static Coroutine Delay1Frame(this MonoBehaviour mono,  System.Action actions)
    {
        return mono.StartCoroutine(Delay1FrameRoutine(actions));
    }

    private static IEnumerator Delay1FrameRoutine(System.Action actions)
    {
        yield return 0;
        if(actions != null)
            actions();
    }

    public static Coroutine DelayEndOfFrame(this MonoBehaviour mono,  System.Action actions)
    {
        return mono.StartCoroutine(Delay1EnfOfFrameRoutine(actions));
    }

    private static IEnumerator Delay1EnfOfFrameRoutine(System.Action actions)
    {
        yield return new WaitForEndOfFrame();
        if(actions != null)
            actions();
    }
}
