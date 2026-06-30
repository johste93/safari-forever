using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LikeButton : MonoBehaviour
{
    public UIToggleButton button;
    public LikeButton Exclusive;

    private void On_Toggle()
    {
        if(button.IsOn())
            Exclusive.button.SetOn(false);
    }

    private void OnEnable()
    {
        button.On_Toggle += On_Toggle;
    }

    private void Unsubscribe()
    {
        button.On_Toggle -= On_Toggle;
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
