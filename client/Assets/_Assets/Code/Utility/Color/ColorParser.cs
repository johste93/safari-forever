using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ColorParser
{
    public static Color Parse(string colorName)
    {
        colorName = colorName.ToLower();

        if("black" == colorName)
            return Color.black;

        if("blue" == colorName)
            return Color.blue;

        if("cyan" == colorName)
            return Color.cyan;

        if("gray" == colorName || "grey" == colorName)
            return Color.gray;

        if("green" == colorName)
            return Color.green;
        
        if("magenta" == colorName)
            return Color.magenta;

        if("red" == colorName)
            return Color.red;

        if("white" == colorName)
            return Color.white;

        if("yellow" == colorName)
            return Color.yellow;

        return Color.clear;
    }
}
