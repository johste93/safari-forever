using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackButton : MonoBehaviour
{
    public Button btn;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            btn.onClick.Invoke();
        }
    }
}
