using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedAspectRatio : MonoBehaviour
{
    public float width;
    public float height;
    public bool scaleByHeight;

    private RectTransform rectTransform;
    private Canvas canvas;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    void Update()
    {
        float aspectRatio = width / height;
        //Debug.Log(rectTransform.rect.width);

        if(scaleByHeight)
        {
            rectTransform.sizeDelta = new Vector2(rectTransform.rect.height / aspectRatio, rectTransform.sizeDelta.y);
        }
        else
        {
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, rectTransform.rect.width / aspectRatio);
        }
        
    }
}
