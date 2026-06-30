using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CleanButton : MonoBehaviour
{
    public void OnClick()
    {
        new Dialog(TranslationKey.Editor_ClearRoom, TranslationKey.Generic_NoUndo)
        .AddPositiveButton(TranslationKey.Generic_Cancel,null)
        .AddDestructiveButton(TranslationKey.Editor_ClearRoom, ()=>{
            LevelBuilder.instance.ClearRoom();
        }).Show(true);
    }
}
