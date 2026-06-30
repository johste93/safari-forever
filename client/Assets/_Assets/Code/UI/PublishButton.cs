using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PublishButton : MonoBehaviour
{
    public EditorMenuWindow window;

    public void OnClick()
    {
        TouchInput.CancelTouch();
        window.Close();
        GameMaster.instance.AttemptPublishLevel();
    }
}
