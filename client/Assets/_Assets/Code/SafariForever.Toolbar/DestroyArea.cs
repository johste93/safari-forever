using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace SafariForever.Toolbar
{
    public class DestroyArea : Singleton<DestroyArea>
    {
        public RectTransform toolbarRect;
        public RectTransform aspectRatioScalerRect;
        public RectTransform destroyRect;

        public Toolbar toolbar;
        

        public RectTransform destroyPointAnchor;
        private Canvas parentCanvas;

        private void Awake()
        {
            parentCanvas = toolbarRect.root.GetComponent<Canvas>();
        }

        private IEnumerator Start()
        {
            yield return 0;

            destroyRect.sizeDelta += new Vector2(0, toolbar.safeArea.GetBottomIndentation());
            destroyRect.DOAnchorPosY(-destroyRect.sizeDelta.y, 0f);
        }
        
        private void Show()
        {
            float differance = toolbar.safeArea.GetBottomIndentation();

            destroyRect.DOKill();
            destroyRect.DOAnchorPosY(0f, 0.3f).SetEase(Ease.InOutQuad);
            
            toolbar.HideToolbar();
        }

        private void Hide()
        {
            destroyRect.DOKill();
            destroyRect.DOAnchorPosY(-destroyRect.sizeDelta.y, 0.3f).SetEase(Ease.InOutQuad);

            toolbar.ShowToolbar();
        }

        public bool PositionInsideDestroyArea(Vector2 viewPortPostion)
        {
            float screenPointY = 0f;

            Vector2 vp = new Vector2(0.5f, destroyRect.sizeDelta.y / parentCanvas.rootCanvas.GetComponent<RectTransform>().sizeDelta.y);
            screenPointY = parentCanvas.rootCanvas.worldCamera.ViewportToScreenPoint(vp).y;

            float maxViewPortY = screenPointY / Screen.height;

            if(viewPortPostion.y <= maxViewPortY)
            {
                return true;
            }

            return false;
        }

        private void OnEnable()
        {
            EntitySpy.OnSomeEntityStartedMoving += Show;
            EntitySpy.OnAllEntitiesStoppedMoving += Hide;
        }

        private void Unsubscribe()
        {
            EntitySpy.OnSomeEntityStartedMoving -= Show;
            EntitySpy.OnAllEntitiesStoppedMoving -= Hide;
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        private void OnDestroy()
        {
            Unsubscribe();
			KillAllTweens();
        }

		private void KillAllTweens()
		{
			destroyRect.DOKill();
		}
    }
}
