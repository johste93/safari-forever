using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SafariForever.Toolbar
{
    public class ToolbarGrid : MonoBehaviour
    {
        public ToolbarButton toolbarButton;

        public GameObject elementPrefab;

        public RectTransform horizontalLayoutGroupRectA;
        public RectTransform horizontalLayoutGroupRectB;
        public RectTransform horizontalLayoutGroupRectC;

        private float totalWidth;
        private float totalHeight;

        private const float spacing = 2f;
        private RectTransform gridRect;

        private void Start()
        {
            gridRect = GetComponent<RectTransform>();

            UpdateSize();
        }

        private void UpdateSize()
        {
            totalWidth = gridRect.rect.width;
            totalHeight = gridRect.rect.height;

            float parentWidth = ((RectTransform)gridRect.parent).rect.width;
            gridRect.sizeDelta = new Vector2(parentWidth, totalHeight);

            if (horizontalLayoutGroupRectA != null)
                horizontalLayoutGroupRectA.sizeDelta = new Vector2(parentWidth, horizontalLayoutGroupRectA.sizeDelta.y);

            if (horizontalLayoutGroupRectB != null)
                horizontalLayoutGroupRectB.sizeDelta = new Vector2(parentWidth, horizontalLayoutGroupRectB.sizeDelta.y);

            if (horizontalLayoutGroupRectC != null)
                horizontalLayoutGroupRectC.sizeDelta = new Vector2(parentWidth, horizontalLayoutGroupRectC.sizeDelta.y);
        }

        private void On_OrientationChanged(DeviceOrientation orientation)
        {
            UpdateSize();
        }

        private void OnEnable()
        {
            ScreenOrientationManager.On_OrientationChanged += On_OrientationChanged;
        }

        private void OnDisable()
        {
            ScreenOrientationManager.On_OrientationChanged -= On_OrientationChanged;
        }
    }
}
