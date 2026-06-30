using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeviceUtil
{
    public static bool IsTablet()
    {
        if (Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer && !Application.isEditor)
            return false;

        if(Application.isEditor)
        {
            //If editor we dont care for the diagocal size.
            return Screen.width > Screen.height;
        }
        
        /*
        Debug.Log("dpi: " + Screen.dpi);
        if (Screen.dpi < 25 || Screen.dpi > 1000)
            return false;
        */

        var aspectRatio = Mathf.Max(Screen.width, Screen.height) / Mathf.Min(Screen.width, Screen.height);
        return (DeviceDiagonalSizeInInches() >= 9.6f && aspectRatio < 2f);
    }

    public static float DeviceDiagonalSizeInInches()
    {
        float screenWidth = Screen.width / Screen.dpi;
        float screenHeight = Screen.height / Screen.dpi;
        float diagonalInches = Mathf.Sqrt(Mathf.Pow(screenWidth, 2) + Mathf.Pow(screenHeight, 2));

        //Debug.Log("Getting device inches: " + diagonalInches);

        return diagonalInches;
    }
}
