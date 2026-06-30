using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class StatPopup : MonoBehaviour
{   
    public TextMeshProUGUI textMesh;
    public RectTransform child;
    
    private bool isShowing;
    private Tween tween;

    public bool IsShowing()
    {
        return isShowing;
    }

    public void Show()
    {
        if(isShowing)
            return;

        tween?.Kill();
        child.localScale = new Vector3(0,0,1);
        child.gameObject.SetActive(true);
        tween = child.DOScale(1f, 0.2f).SetEase(Ease.OutBack);
        isShowing = true;
    }

    public void Close()
    {
        if(!isShowing)
            return;

        tween?.Kill();
        tween = child.DOScale(0f, 0.2f).SetEase(Ease.OutQuad);
        isShowing = false;
    }

    private void OnDisable()
    {
        tween?.Kill();
        tween = null;
        isShowing = false;
        child.gameObject.SetActive(false);
        textMesh.text = "?";
    }
}
