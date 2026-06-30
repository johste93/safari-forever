using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class CreditsWindow : MonoBehaviour
{
    public RectTransform window;

	public GameObject[] pages;

	public TextMeshProUGUI page3;

	private int currentIndex = 0;

	public void OnClickExit()
	{
		Close();
	}

	public void OnClickNext()
	{
		Next();
	}

	public void Next()
	{
		pages[currentIndex].SetActive(false);
		currentIndex++;
		pages[currentIndex].SetActive(true);

		LayoutRebuilder.ForceRebuildLayoutImmediate(window);
		LayoutRebuilder.ForceRebuildLayoutImmediate(window);
	}

	public void Show()
    {
		currentIndex = 0;

		for(int i = 0; i < pages.Length; i++)
		{
			pages[i].SetActive(i == 0);
		}

		window.DOKill();
        window.anchoredPosition = new Vector3(0, -400f, 0);

        Canvas.ForceUpdateCanvases();

        window.gameObject.SetActive(true);
		window.DOAnchorPosY(Screen.width <= Screen.height ? 100f : (800f - window.sizeDelta.y) / 2f, 0.3f);

		string names = string.Empty;

		if(Localization.KeyAvailable(TranslationKey.Credits_Names, SaveManager.currentSave.language))
		{
			names = Localization.GetTranslation2(TranslationKey.Credits_Names, SaveManager.currentSave.language);
		}

		page3.TranslateFormat(TranslationKey.Credits_Page_3, SaveManager.currentSave.language, FontType.Regular, false, names);
    }

	public void Close()
	{
		gameObject.SetActive(false);
        DialogCanvas.instance.FadeOutbackground();
	}
}
