using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace SafariForever.Toolbar
{
    public class Toolbar : Singleton<Toolbar>
    {
        public RectTransform toolbarRect;
        public ToolbarHorizontalLayoutGroup layoutGroup;
        public ToolbarViews toolbarViews;
        public RectTransform aspectRatioScalerRect;
        public SafeArea safeArea;
        public RectTransform viewsRect;

        public RectTransform categoryBar;
        public RectTransform gizmoRect;

        public delegate void TabChangeEvent(int index);
	    public static TabChangeEvent On_TabChange;

        private bool isOpen;
        private bool isHidden;
        private int currentButtonIndex = 0;

        public void Toggle()
        {
            if(isOpen)
                Close();
            else
                Open();
        }
        
        public void Open()
        {       
            isOpen = true;

            float differance = safeArea.GetBottomIndentation();

            float height = Screen.width <= Screen.height ? 120f : 60f;

            aspectRatioScalerRect.DOKill();
            aspectRatioScalerRect.DOAnchorPosY(height, 0.3f).SetEase(Ease.OutBack);

            viewsRect.DOKill();
            viewsRect.DOAnchorPosY(height + differance, 0.3f).SetEase(Ease.OutBack);
        }

        public void Close()
        {   
            isOpen = false;

            aspectRatioScalerRect.DOKill();
            aspectRatioScalerRect.DOAnchorPosY(0f, 0.3f).SetEase(Ease.OutBack);

            viewsRect.DOKill();
            viewsRect.DOAnchorPosY(0f, 0.3f).SetEase(Ease.OutBack);
        }

        public void SetCategory(int index, bool instant)
        {
            currentButtonIndex = index;
            bool isOpening = false;
            if(isOpen)
            {
                if(index == layoutGroup.GetCurrentIndex())
                    Close();
            }
            else
            {
                Open();
                isOpening = true;
            }

            //layoutGroup.ResizeButtons(index, instant);
            layoutGroup.SelectButton(index, instant);
            toolbarViews.SetView(index, instant || isOpening);

            On_TabChange?.Invoke(index);
        }

        public void HideToolbar()
        {
            isHidden = true;
            float differance = safeArea.GetBottomIndentation();

            aspectRatioScalerRect.DOKill();
            aspectRatioScalerRect.DOAnchorPosY(0f, 0.3f).SetEase(Ease.InOutQuad);

            categoryBar.DOKill();
            categoryBar.DOAnchorPosY(-(55f + differance), 0.3f).SetEase(Ease.InOutQuad);

            gizmoRect.DOKill();
            gizmoRect.DOAnchorPosY(-150f - differance, 0.3f).SetEase(Ease.InOutQuad);

            viewsRect.DOKill();
            viewsRect.DOAnchorPosY(-55f, 0.3f).SetEase(Ease.InOutQuad);
        }

        public void ShowToolbar()
        {
            isHidden = false;
            float height = Screen.width <= Screen.height ? 120f : 60f;

            aspectRatioScalerRect.DOKill();
            aspectRatioScalerRect.DOAnchorPosY(IsOpen() ? height : 0f, 0.3f).SetEase(Ease.InOutQuad);
            
            categoryBar.DOKill();
            categoryBar.DOAnchorPosY(0f, 0.3f).SetEase(Ease.InOutQuad);

            gizmoRect.DOKill();
            gizmoRect.DOAnchorPosY(0f, 0.3f).SetEase(Ease.InOutQuad);

            viewsRect.DOKill();
            viewsRect.DOAnchorPosY(IsOpen() ? height + safeArea.GetBottomIndentation() : 0f, 0.3f).SetEase(Ease.InOutQuad);
        }

        public int GetCurrentButtonIndex()
        {
            return currentButtonIndex;
        }

        public bool IsOpen()
        {
            return isOpen;
        }

		private void OnDestroy()
		{
			KillAllTweens();
		}

		private void KillAllTweens()
		{
			aspectRatioScalerRect.DOKill();
			categoryBar.DOKill();
			gizmoRect.DOKill();
			viewsRect.DOKill();	
		}

        private void On_OrientationChanged(DeviceOrientation orientation)
        {
            if(isHidden)
            {
                HideToolbar();
            }
            else
            {
                Close();
            }
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
