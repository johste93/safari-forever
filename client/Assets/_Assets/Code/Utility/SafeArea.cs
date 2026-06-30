using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeArea : MonoBehaviour
{
    RectTransform panel;
    RectTransform parentCanvasPanel;
    Rect lastSafeArea = new Rect (0, 0, 0, 0);

    public delegate void SafeAreaUpdate(RectTransform tranform);
    public SafeAreaUpdate OnSafeAreaUpdate;

    void Awake ()
    {
        panel = GetComponent<RectTransform> ();
        parentCanvasPanel = (RectTransform) panel.root.GetComponent<Canvas>().transform;
        Refresh ();
    }

    void Update ()
    {
        Refresh ();
    }

    void Refresh ()
    {
        Rect safeArea = GetSafeArea ();

        if (safeArea != lastSafeArea)
            ApplySafeArea (safeArea);
    }

    Rect GetSafeArea ()
    {
        return Screen.safeArea;
    }

    void ApplySafeArea (Rect r)
    {
        lastSafeArea = r;

        // Convert safe area rectangle from absolute pixels to normalised anchor coordinates
        Vector2 anchorMin = r.position;
        Vector2 anchorMax = r.position + r.size;
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;
        panel.anchorMin = anchorMin;
        panel.anchorMax = anchorMax;

        /*
        Debug.LogFormat ("New safe area applied to {0}: x={1}, y={2}, w={3}, h={4} on full extents w={5}, h={6}",
            name, r.x, r.y, r.width, r.height, Screen.width, Screen.height);
        */

        OnSafeAreaUpdate?.Invoke(panel);
    }

    public float GetTopIndentation()
    {
        return (1f - panel.anchorMax.y) * parentCanvasPanel.sizeDelta.y;
    }

    public float GetBottomIndentation()
    {
        return panel.anchorMin.y * parentCanvasPanel.sizeDelta.y;
    }

    public float GetRightIndentation()
    {
        return (1f - panel.anchorMax.x) * parentCanvasPanel.sizeDelta.x;
    }

    public float GetLeftIndentation()
    {
        return panel.anchorMin.x * parentCanvasPanel.sizeDelta.x;
    }
}
