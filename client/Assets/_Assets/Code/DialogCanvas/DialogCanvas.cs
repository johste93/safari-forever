using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class DialogCanvas : Singleton<DialogCanvas>
{
    public RectTransform window;
    public RectTransform buttonParent;
    public GameObject buttonPrefab;

    public CanvasGroup windowCanvasGroup;

    public TextMeshProUGUI titleTextMesh;
    public TextMeshProUGUI messageTextMesh;

    public ColorPickerWindow colorPickerWindow;
    public AgeVerificationWindow ageVerificationWindow;
	public TermsOfServiceWindow termsOfServiceWindow;
    public PrivacyPolicyWindow privacyPolicyWindow;
    public NicknameWindow nicknameWindow;
    public EditorMenuWindow editorMenuWindow;
    public PlayMenuWindow playMenuWindow;
    public RecordMenuWindow recordMenuWindow;
    public ShareRecordingMenuWindow shareRecordingMenuWindow;
	public CreditsWindow creditsWindow; 
    public CountryWindow countryWindow;
    public ConsentWindow consentWindow;
    public BoostWindow boostWindow;
    public PickerWindow pickerWindow;
    public ReviewWindow reviewWindow;
    public LevelGaragePickerWindow levelGarageWindow;
    public SlotMachineWindow slotMachineWindow;
    public CardsWindow cardsWindow;

    public Image backgroundImg;
    public Image loadingBackgroundImg;
    public Image progressBackgroundImg;


    public CanvasGroup backgroundCanvasGroup;
    public CanvasGroup loadingBackgroundCanvasGroup;
    public CanvasGroup progressBackgroundCanvasGroup;

    public ProgressAnimation progressAnimation;

	private System.Action onComplete;
    private List<Tween> tweens = new List<Tween>();

    private bool hideLoadingAtEndOfFrame;
    private bool hideProgressAtEndOfFrame;
    private Dialog currentDialog;

    public void ShowLoading(bool instant = false)
    {
        hideLoadingAtEndOfFrame = false;

        if(loadingBackgroundCanvasGroup.gameObject.activeInHierarchy)
            return;
            
        loadingBackgroundCanvasGroup.DOKill();
        //loadingBackgroundImg.color = SaveManager.currentSave.noBrightFlashesMode ? new Color(0.23f, 0.23f, 0.23f, 0.75f) : Color.white.SetAlpha(0.75f);
        loadingBackgroundCanvasGroup.alpha = 0f;
        loadingBackgroundCanvasGroup.gameObject.SetActive(true);
        tweens.Add(loadingBackgroundCanvasGroup.DOFade(1f, instant ? 0f : 0.2f));
    }

    public void ShowProgress(bool instant = false)
    {
        hideProgressAtEndOfFrame = false;

        if(progressBackgroundCanvasGroup.gameObject.activeInHierarchy)
            return;

        progressBackgroundCanvasGroup.DOKill();
        //progressBackgroundImg.color = SaveManager.currentSave.noBrightFlashesMode ? new Color(0.23f, 0.23f, 0.23f, 0.75f) : Color.white.SetAlpha(0.75f);
        progressBackgroundCanvasGroup.alpha = 0f;
        progressBackgroundCanvasGroup.gameObject.SetActive(true);
        tweens.Add(progressBackgroundCanvasGroup.DOFade(1f, instant ? 0f : 0.2f));
    }

    public void SetProgress(float progress)
    {
        progressAnimation.SetValue(progress);
    }
	
	public void HideLoading()
	{
        hideLoadingAtEndOfFrame = true;
        StartCoroutine(HideLoadingAtEndOfFrame());
	}

    public void HideProgress()
	{
        hideProgressAtEndOfFrame = true;
        StartCoroutine(HideProgressAtEndOfFrame());
	}

    private IEnumerator HideLoadingAtEndOfFrame()
    {
        yield return new WaitForEndOfFrame();

        if(!hideLoadingAtEndOfFrame)
            yield break;

        loadingBackgroundCanvasGroup.gameObject.SetActive(false);
    }

    private IEnumerator HideProgressAtEndOfFrame()
    {
        yield return new WaitForEndOfFrame();

        if(!hideProgressAtEndOfFrame)
            yield break;

        progressBackgroundCanvasGroup.gameObject.SetActive(false);
    }

    public void FadeInBackground(bool instant = false)
    {
        TouchInput.CancelTouch();

        SuspensionManager.Suspend(true);

        //backgroundImg.color = SaveManager.currentSave.noBrightFlashesMode ? new Color(0.23f, 0.23f, 0.23f, 0.75f) : Color.white.SetAlpha(0.75f);
        backgroundCanvasGroup.DOKill();
        backgroundCanvasGroup.gameObject.SetActive(true);
        tweens.Add(backgroundCanvasGroup.DOFade(1f, instant ? 0f : 0.2f));
    }

    public void FadeOutbackground(bool instant = false)
    {
        backgroundCanvasGroup.DOKill();
        tweens.Add(backgroundCanvasGroup.DOFade(0f, instant ? 0f : 0.2f).OnComplete(() =>
        {
            backgroundCanvasGroup.gameObject.SetActive(false);
            SuspensionManager.Suspend(false);
        }));
    }

    public ColorPicker ShowColorPicker(bool hue, bool saturation, bool vibrance, Color currentColor, System.Action<bool, Color> callback)
    {
		//Audio.Play(SFX.instance.ui.dialogBoxOpen, Channel.Game);

        FadeInBackground();

        return colorPickerWindow.Show(hue, saturation, vibrance, currentColor, callback);
    }

    public ColorPicker ShowColorPicker(bool hue, bool saturation, bool vibrance, Color currentColor, ColorPicker.Constraints constraints, System.Action<bool, Color> callback)
    {
        return ShowColorPicker(hue, saturation, vibrance, new Color[]{currentColor}, new ColorPicker.Constraints[]{constraints}, (confirmed, colors)=>
        {
            callback(confirmed, confirmed ? colors[0] : Color.black);
        });
    }

    public ColorPicker ShowColorPicker(bool hue, bool saturation, bool vibrance, Color[] colors, ColorPicker.Constraints[] constraints, System.Action<bool, Color[]> callback)
    {
		//Audio.Play(SFX.instance.ui.dialogBoxOpen, Channel.Game);

        FadeInBackground();

        return colorPickerWindow.Show(hue, saturation, vibrance, colors, constraints, callback);
    }

    public void ShowEditorMenu()
    {
        //Audio.Play(SFX.instance.ui.dialogBoxOpen, Channel.Game);
        FadeInBackground();

        editorMenuWindow.Show();
    }

    public void ShowPlayMenu()
    {
        //Audio.Play(SFX.instance.ui.dialogBoxOpen, Channel.Game);
        FadeInBackground();

        playMenuWindow.Show();
    }

    public void ShowRecordMenuWindow(System.Action onComplete = null)
    {
        FadeInBackground();

        recordMenuWindow.Show(onComplete);
    }

    public void ShowShareRecordingMenuWindow(System.Action onComplete = null)
    {
        FadeInBackground();

        shareRecordingMenuWindow.Show(onComplete);
    }

    public void CloseAllWindows()
    {
	    termsOfServiceWindow.gameObject.SetActive(false);
        privacyPolicyWindow.gameObject.SetActive(false);
        nicknameWindow.gameObject.SetActive(false);
        editorMenuWindow.gameObject.SetActive(false);
        creditsWindow.gameObject.SetActive(false);
        playMenuWindow.gameObject.SetActive(false);
        recordMenuWindow.gameObject.SetActive(false);
        shareRecordingMenuWindow.gameObject.SetActive(false);
        countryWindow.gameObject.SetActive(false);
        consentWindow.gameObject.SetActive(false);
        boostWindow.gameObject.SetActive(false);
        pickerWindow.gameObject.SetActive(false);
        reviewWindow.gameObject.SetActive(false);
        levelGarageWindow.gameObject.SetActive(false);
        slotMachineWindow.gameObject.SetActive(false);
        cardsWindow.gameObject.SetActive(false);
        FadeOutbackground(true);
    }

    public void ShowAgeVerification(System.Action<bool, int> callback, bool showCancelButton)
    {
        DialogCanvas.instance.HideLoading();

        FadeInBackground();

        ageVerificationWindow.Show(callback, showCancelButton);
    }

    public void ShowTermsOfService(System.Action<bool> callback)
    {
        DialogCanvas.instance.HideLoading();

        FadeInBackground();

        termsOfServiceWindow.Show(callback);
    }

    public void ShowPrivacyPolicy(System.Action<bool> callback)
    {
        DialogCanvas.instance.HideLoading();

        FadeInBackground();

        privacyPolicyWindow.Show(callback);
    }

	public void ShowCredithWindow()
    {
        FadeInBackground();

        creditsWindow.Show();
    }

    public void ShowNicknameWindow(string currentNickname, System.Action<bool, string> callback)
    {
        FadeInBackground();

        nicknameWindow.Show(currentNickname, callback);
    }

    public void ShowConsentWindow(bool skipIfHasConsent, System.Action onComplete)
    {
        //FadeInBackground(); We take care of this later.

        consentWindow.Show(skipIfHasConsent, onComplete);
    }

    public void ShowReviewWindow(System.Action onComplete)
    {
        reviewWindow.Show(onComplete);
    }

    public void ShowBoostWindow(LevelElement levelElement, System.Action<int> onComplete)
    {
        FadeInBackground();
        
        boostWindow.Show(levelElement, onComplete);
    }

    public void ShowBoostWindow(string levelId, int coinsInvested, string levelName, long createdOn, System.Action<int> onComplete)
    {
        FadeInBackground();
        
        boostWindow.Show(levelId, coinsInvested, levelName, createdOn, onComplete);
    }

    public void ShowSlotMachineWindow(System.Action onComplete)
    {
        FadeInBackground();
        
        slotMachineWindow.Show(onComplete);
    }

    public void ShowCardsWindow(System.Action onComplete)
    {
        FadeInBackground();
        
        cardsWindow.Show(onComplete);
    }

    public void ShowCountryWindow( System.Action<Country> callback)
    {
        FadeInBackground();

        countryWindow.Show(callback);
    }

    public void ShowPickerWindow(TranslationKey translationKey, Language languague, List<string> options, bool spawnSeparator, bool search, System.Action<int> callback)
    {
        FadeInBackground();

        pickerWindow.Show(translationKey, languague, options, spawnSeparator, search, callback);
    }

    public void ShowPickerWindow(TranslationKey translationKey, Language languague, List<LocalizedOption> options, bool spawnSeparator, bool search, System.Action<int> callback)
    {
        FadeInBackground();

        pickerWindow.Show(translationKey, languague, options, spawnSeparator, search, callback);
    }

    public void ShowLevelGaragePickerWindow(List<string> options, System.Action<int> onConfirm, System.Action<int> onDelete)
    {
        FadeInBackground();

        levelGarageWindow.Show(options, onConfirm, onDelete);
    }

    public void Show(Dialog dialog, bool instant = false, System.Action onComplete = null)
    {
		this.onComplete = onComplete;

        window.DOKill();
        window.anchoredPosition = new Vector3(0, -400f, 0);

        window.gameObject.SetActive(true);

        currentDialog = dialog;

        Initalize();

        if(!string.IsNullOrWhiteSpace(currentDialog.title))
        {
            titleTextMesh.text = currentDialog.title;
            titleTextMesh.isRightToLeftText = dialog.titleIsRTL;
            titleTextMesh.font = Globals.accessibilityConstants.GetFont(SaveManager.currentSave.openDyslexic, FontType.Stylized, titleTextMesh.font, currentDialog.titleLanguage);
            //Dont flip alignment for dialogs
        }
        else
        {
            titleTextMesh.Translate(currentDialog.titleKey, SaveManager.currentSave.language, FontType.Stylized, false);
        }

        if(!string.IsNullOrWhiteSpace(currentDialog.message))
        {
            messageTextMesh.text = currentDialog.message;
		    messageTextMesh.isRightToLeftText = currentDialog.messageIsRTL;
            messageTextMesh.font = Globals.accessibilityConstants.GetFont(SaveManager.currentSave.openDyslexic, FontType.Regular, messageTextMesh.font, currentDialog.messageLanguage);
            //Dont flip alignment for dialogs
        }
        else
        {
            messageTextMesh.Translate(currentDialog.messageKey, SaveManager.currentSave.language, FontType.Regular, false);
        }

        windowCanvasGroup.alpha = 0;

        //LayoutRebuilder.ForceRebuildLayoutImmediate(window);

        FadeInBackground(instant);

        window.gameObject.SetActive(false);
        window.gameObject.SetActive(true);

        //Canvas.ForceUpdateCanvases();

        windowCanvasGroup.alpha = 1;

        this.Delay1Frame(()=>
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(window);
            LayoutRebuilder.ForceRebuildLayoutImmediate(window);
            tweens.Add( window.DOAnchorPosY(Screen.width <= Screen.height ? 100f : (800f - window.sizeDelta.y) / 2f, instant ? 0f : 0.3f) );
        });
    }

    public void Close()
    {
        FadeOutbackground();
        currentDialog = null;
    }

    private void Initalize()
    {
        buttonParent.DestroyChildren();

        foreach(DialogButtonValues values in currentDialog.buttons)
        {
            GameObject lastSpawn = GameObject.Instantiate(buttonPrefab, buttonParent);
            lastSpawn.transform.SetAsFirstSibling();
            
            DialogButton lastButton = lastSpawn.GetComponent<DialogButton>();
            lastButton.Initalize(values);
        }
    }

    public void Complete()
    {
        window.gameObject.SetActive(false);
        
        currentDialog = null;

		//Audio.Play(SFX.instance.ui.dialogBoxClose, Channel.Game);

        FadeOutbackground();


        if (onComplete != null)
		{
			onComplete();
			onComplete = null;
		}
    }

	private void OnDestroy()
	{
		KillAllTweens();
	}

	private void KillAllTweens()
	{
        foreach(Tween t in tweens)
        {
            if (t != null)
                t.Kill();
        }
        tweens = new List<Tween>();
    }
}
