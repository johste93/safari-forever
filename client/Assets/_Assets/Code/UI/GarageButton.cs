using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarageButton : MonoBehaviour
{
    public EditorMenuWindow window;

    public void OnClick()
    {
        TouchInput.CancelTouch();
        window.Close();
    
        Garage.ShowGarage();
    }
}
