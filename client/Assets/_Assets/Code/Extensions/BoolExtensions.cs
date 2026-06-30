using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BoolExtensions
{
    public static int ToInt(this bool b)
    {
        return b ? 1 : 0;
    }
}
