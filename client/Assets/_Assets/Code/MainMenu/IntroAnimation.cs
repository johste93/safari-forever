using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class IntroAnimation : MonoBehaviour
{
    public PunchButton logoPunchButton;

    public RectTransform logo;
    public RectTransform buttonsParent;
    public RectTransform touchToStartRect;

    public GameObject shopCamera;

    public Image flashImg;

    public CanvasGroup logoCanvasGroup;
    public CanvasGroup buttonsCanvasGroup;

    public TextMeshProUGUI startText;

    private Vector3 logoTargetPosition;
    private Vector3 buttonsTargetPosition;

    private bool hasPlayedIntro;

    public Tween logoAnimation;
	public Tween delay;

    public AudioClip shimmer;

    private void Awake()
    {
        PlayIntroAnimation();
    }

    public void PlayIntroAnimation()
    {
        logoCanvasGroup.alpha = 0;
        buttonsCanvasGroup.alpha = 0;

        logoTargetPosition = logo.localPosition;
        buttonsTargetPosition = buttonsParent.localPosition;

        logo.anchoredPosition -= new Vector2(0, 200f);
        buttonsParent.anchoredPosition -= new Vector2(0, 200f);

        startText.color = new Color(1,1,1,0);
    }

    private void Start()
    {
        logoCanvasGroup.DOFade(1f, 1f).SetDelay(1f).OnComplete(()=>
        {
            if(!hasPlayedIntro)
            {
                logo.DOMoveY(-0.25f, 2f).SetRelative(true).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
                startText.DOColor(Color.white, 0.75f).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);
            }
        });
    }

    public void OnClick()
    {   
        if(hasPlayedIntro)
            return;

        if(SaveManager.currentSave.sfx)
            Audio.Play(shimmer, Channel.UI);

        delay = DOVirtual.DelayedCall(shimmer.length, ()=>{
            MusicManager.Play(Music.Menu, false);
        });

        flashImg.DOFade(1f, 0.1f).OnComplete(()=>{
            flashImg.DOFade(0f, 0.4f);
        });

        hasPlayedIntro = true;
        Unsubscribe();

        startText.transform.DOKill();

        startText.DOKill();
        startText.DOFade(0f, 0.1f);

        logo.DOKill();

        logoPunchButton.OnClick();

        logo.DOLocalMove(logoTargetPosition, 1f).SetEase(Ease.InOutQuad).OnComplete(()=>
        {
            logoAnimation = logo.DOMoveY(-0.25f, 2f).SetRelative(true).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
        });
        
        this.Delay(0.75f, ()=>
        {
            shopCamera.gameObject.SetActive(true);
        });

        this.Delay(1f, ()=>
        {
            buttonsCanvasGroup.DOFade(1f, 0.5f);
            buttonsParent.DOLocalMove(buttonsTargetPosition, 0.5f).SetEase(Ease.OutQuad);
        });
    }

    private void On_TouchMaintained(TouchInfo touch)
    {
        if(touch.pickedUIElement != touchToStartRect.gameObject)
            return;

        OnClick();
        touchToStartRect.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        TouchInput.On_TouchMaintained += On_TouchMaintained;
    }

    private void Unsubscribe()
    {
        TouchInput.On_TouchMaintained -= On_TouchMaintained;
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
		if(logoAnimation != null)
			logoAnimation.Kill();

		if(delay != null)
			delay.Kill();

		buttonsCanvasGroup.DOKill();
		buttonsParent.DOKill();
		logo.DOKill();
		startText.transform.DOKill();
        startText.DOKill();
		flashImg.DOKill();
	}

}
