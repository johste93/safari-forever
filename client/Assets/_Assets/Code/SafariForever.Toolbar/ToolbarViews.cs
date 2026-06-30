using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace SafariForever.Toolbar
{
    public class ToolbarViews : MonoBehaviour
    {
        public RectTransform viewsRect;

        private float width;

        private void Start()
        {
            width = viewsRect.rect.width;
        }

        public void SetView(int index, bool instant)
        {
            float pos = index * -width;

            viewsRect.DOKill();
            viewsRect.DOAnchorPosX(pos, instant ? 0f : 0.3f).SetEase(Ease.InOutQuad);
        }

		private void OnDestroy()
		{
			KillAllTweens();
		}

		private void KillAllTweens()
		{
			viewsRect.DOKill();
		}

        private void On_OrientationChanged(DeviceOrientation orientation)
        {
            width = viewsRect.rect.width;
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
