using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class LoadEditorButton : MonoBehaviour
{
	public int requiredUnlock = 3;
	public Image shadow;
	public Image surface;
	public Image padlock;
	public GameObject buttonPrint;
	public TextMeshProUGUI textMesh;

	private bool unlocked {
		get{
			return GetNumberOfLevelsRequiredToUnlock() == 0 || Application.isEditor;
		}
	}

	private void UpdateButton()
	{
		surface.color = unlocked ? Color.white : Color.white.SetAlpha(0.6f);
		textMesh.color = LABColor.Lerp(shadow.color, surface.color, 0.5f).SetAlpha(0.6f);
		textMesh.text = GetNumberOfLevelsRequiredToUnlock().ToString();

		textMesh.gameObject.SetActive(!unlocked);
		padlock.gameObject.SetActive(!unlocked);
		buttonPrint.SetActive(unlocked);
	}

	private int GetNumberOfLevelsRequiredToUnlock()
	{
		int beatenLevelsInThisWorld = SaveManager.currentSave.campaignProgress[(int)World.World_1].Count(x => x.beaten == true);
		return Mathf.Max(requiredUnlock - beatenLevelsInThisWorld, 0);
	}

    public void OnClick()
    {
		if(!unlocked)
		{
			Language messageLanguage = Localization.KeyAvailable(TranslationKey.Menu_EditorLocked_Message, SaveManager.currentSave.language) ? SaveManager.currentSave.language : Globals.localizationConstants.defaultLanguage;
			bool messageIsRTL = Localization.IsRightToLeftLanguage(messageLanguage);
			string message = Localization.GetTranslationFormat2(TranslationKey.Menu_EditorLocked_Message, SaveManager.currentSave.language, GetNumberOfLevelsRequiredToUnlock());

			new Dialog(
				TranslationKey.Menu_EditorLocked_Header,
				message, messageLanguage,
				messageIsRTL)
			.AddNeutralButton(TranslationKey.Generic_Ok, null, true)
			.Show();
			return;
		}

        GlobalSingleton.colorPalette = ColorGenerator.GetRandomColorPalette(true);
        GlobalSingleton.mode = GameMode.Create;
        SceneLoader.Load(SafariScene.Game);
    }

	private void OnMergeComplete()
	{
		UpdateButton();
	}

	private void OnEnable()
	{
		UpdateButton();
		Save.OnMergeComplete += OnMergeComplete;
	}

	private void Unsubscribe()
	{
		Save.OnMergeComplete -= OnMergeComplete;
	}

	private void OnDisable()
	{
		Unsubscribe();
	}

	private void OnDestroy()
	{
		Unsubscribe();
	}
}
