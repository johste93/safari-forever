using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolbarAspectAdjuster : MonoBehaviour
{
    public RectTransform buildCategory;
    public GameObject[] verticalAspectGroups; 
    public GameObject[] horizontalAspectGroups;

    private void Awake()
    {
        AdjustAspectRatio();
    }

    private void AdjustAspectRatio()
    {
        bool verticalView = Screen.width <= Screen.height;

        foreach (GameObject group in verticalAspectGroups)
            group.SetActive(verticalView);

        foreach(GameObject group in horizontalAspectGroups)
            group.SetActive(!verticalView);

        buildCategory.sizeDelta = new Vector2(buildCategory.sizeDelta.x, verticalView ? 112f : 50f);
    }

    private void On_OrientationChanged(DeviceOrientation orientation) => AdjustAspectRatio();

    private void OnEnable()
    {
        ScreenOrientationManager.On_OrientationChanged += On_OrientationChanged;
    }

    private void OnDisable()
    {
        ScreenOrientationManager.On_OrientationChanged -= On_OrientationChanged;
    }
}
