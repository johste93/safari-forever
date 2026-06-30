using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenOrientationManager : ImmortalSingleton<ScreenOrientationManager>
{
    private DeviceOrientation currentOrientation;
    public delegate void OrientationChanged(DeviceOrientation orientation);
    public static OrientationChanged On_OrientationChanged;

    public void Start()
    {
        UpdateScreenOrientation();
    }

    public void UpdateScreenOrientation()
    {
        if(Application.isMobilePlatform)
        {
            if(DeviceUtil.IsTablet())
            {
                Screen.orientation = ScreenOrientation.AutoRotation;
            }
            else
            {
                Screen.orientation = ScreenOrientation.Portrait;
            }
        }
        else
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft;
        }
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.U) && Application.isEditor)
        {
            this.Delay1Frame(() =>
            {
                On_OrientationChanged?.Invoke(currentOrientation);
            });
        }

        DeviceOrientation deviceOrientation = Screen.width > Screen.height ? DeviceOrientation.LandscapeLeft : DeviceOrientation.Portrait;

        if(deviceOrientation != currentOrientation)
        {
            currentOrientation = deviceOrientation;
            
            this.Delay1Frame(() =>
            {
                UpdateScreenOrientation();
                On_OrientationChanged?.Invoke(currentOrientation);
            });
        }
    }


}
