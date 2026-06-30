using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEntityPendulum : LevelEntity
{
    public override void ShowSizeGizmos(bool show)
    {
        foreach(SizeGizmo gizmo in sizeGizmos)
            gizmo.gameObject.SetActive(show);
    }
}
