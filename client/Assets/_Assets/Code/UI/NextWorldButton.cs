using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class NextWorldButton : MonoBehaviour
{
	public int requiredUnlock = 0;
	public World thisWorld;
	public ScreenScroller screenScroller;
	public TextMeshProUGUI textMesh;

	public Image shadow;
	public Image surface;
	public Image padlock;
	public Image arrow;

	private bool unlocked {
		
		get { 
			if(Application.isEditor)
                return true;
				
			return GetNumberOfLevelsRequiredToUnlock() == 0; 
			}
	}

	private void Start()
	{
		surface.color = unlocked ? Color.white : Color.white.SetAlpha(0.6f);
		textMesh.color = LABColor.Lerp(shadow.color, surface.color, 0.5f).SetAlpha(0.6f);
		textMesh.text = GetNumberOfLevelsRequiredToUnlock().ToString();

		textMesh.gameObject.SetActive(!unlocked);
		padlock.gameObject.SetActive(!unlocked);
		arrow.gameObject.SetActive(unlocked);
	}

	private int GetNumberOfLevelsRequiredToUnlock()
	{
		int beatenLevelsInThisWorld = SaveManager.currentSave.campaignProgress[(int)thisWorld].Count(x => x.beaten == true);
		return Mathf.Max(requiredUnlock - beatenLevelsInThisWorld, 0);
	}

    public void OnClick()
	{
		if(!unlocked)
		{
			Language messageLanguage = Localization.KeyAvailable(TranslationKey.Menu_WorldLocked_Message, SaveManager.currentSave.language) ? SaveManager.currentSave.language : Globals.localizationConstants.defaultLanguage;
			bool messageIsRTL = Localization.IsRightToLeftLanguage(messageLanguage);
			string message = Localization.GetTranslationFormat2(TranslationKey.Menu_WorldLocked_Message, SaveManager.currentSave.language, GetNumberOfLevelsRequiredToUnlock());
			
			new Dialog(
				TranslationKey.Menu_WorldLocked_Header,
				message, messageLanguage,
				messageIsRTL)
			.AddNeutralButton(TranslationKey.Generic_Ok, null, true)
			.Show();
			return;
		}

		screenScroller.Next();
	}
}
