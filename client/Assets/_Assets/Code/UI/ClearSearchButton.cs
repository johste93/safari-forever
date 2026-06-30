using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class ClearSearchButton : MonoBehaviour
{
    public SearchView searchView;
    public Transform child;

    private Tween tween;

    public void OnClick()
    {
        child.DOComplete();
        tween = child.DOPunchScale(Vector3.one * 0.2f, 0.3f);

        searchView.ClearSearch();
    }

    private void OnDestroy()
    {
        if(tween != null)
        {
            tween.Kill();
            tween = null;
        }
    }
}
