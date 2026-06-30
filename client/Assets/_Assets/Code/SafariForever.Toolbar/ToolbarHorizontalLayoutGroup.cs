using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace SafariForever.Toolbar
{
    public class ToolbarHorizontalLayoutGroup : MonoBehaviour
    {
        public List<ToolbarButton> buttons; 

        private int currentButtonIndex = -1;

        private const float minWidth = 50f;
        private float selectedButtonWidth = minWidth;

        private const float animationDuration = 0.3f;

        public Color unselectedColor;

        public RectTransform heightlight;
        public RectTransform toolbarRect;
        public RectTransform aspectRatioScalerRect;

        private IEnumerator Start()
        {
            yield return 0;
            SelectButton(0, true);
        }

        public void SelectButton(int selectedButtonIndex, bool instant = false)
        {
            bool selected = currentButtonIndex == selectedButtonIndex;
            if(selected && !instant)
                return;

            if(!instant)
                Audio.Play(SFX.instance.ui.tabChange.randomClip, Channel.UI).SetPitch(Random.Range(0.9f, 1.1f));

            currentButtonIndex = selectedButtonIndex;

            for(int i = 0; i < buttons.Count; i++)
            {
                buttons[i].image.DOKill();
                buttons[i].image.DOColor(selectedButtonIndex == i ?  Color.white : new Color(1f,1f,1f,0f), instant ? 0f : animationDuration);
            }

            RectTransform rect = buttons[currentButtonIndex].GetComponent<RectTransform>();
            heightlight.sizeDelta = rect.sizeDelta;
            heightlight.DOKill();
            heightlight.DOAnchorPos(rect.anchoredPosition, instant ? 0f : 0.15f).SetEase(Ease.InOutQuad);
        }

        public int GetCurrentIndex()
        {
            return currentButtonIndex;
        }

		private void OnDestroy()
		{
			KillAllTweens();
		}

		private void KillAllTweens()
		{
			heightlight.DOKill();
		}
    }
}
