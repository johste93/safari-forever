using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITeleportable
{
    Vector3 GetPortalOffset();
    Vector2 GetSize();
} 