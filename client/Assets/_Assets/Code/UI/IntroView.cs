using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class IntroView : MonoBehaviour
{
    public GameObject container;
    public RectTransform touchToStartRect;
    public GameObject shopCamera;
    public AudioClip shimmer;
    public Image flashImg;
    public TextMeshProUGUI startText;
    public RectTransform logo;
    public PunchButton logoPunchButton;
    public CanvasGroup logoCanvasGroup;
    public CanvasGroup buttonsCanvasGroup;
    public RectTransform buttonsParent;
    public ProfileButton profileButton;

    public Image backgroundPattern;

    private Vector3 logoTargetPosition;
    private Vector3 buttonsTargetPosition;
    private bool clicked;

	private List<Tween> tweens = new List<Tween>();

    private float backgroundPatternTargetAlpha;

    private void Awake()
    {
        logoCanvasGroup.alpha = 0;
        buttonsCanvasGroup.alpha = 0;
		buttonsCanvasGroup.gameObject.SetActive(false);
        
        startText.color = new Color(1,1,1,0);

        backgroundPatternTargetAlpha = backgroundPattern.color.a;
        backgroundPattern.color = backgroundPattern.color.SetAlpha(0f);
    }

    private void Start()
    {
        logoTargetPosition = logo.localPosition;
        buttonsTargetPosition = buttonsParent.localPosition;
        
        logo.anchoredPosition -= new Vector2(0, 200f);
        buttonsParent.anchoredPosition -= new Vector2(0, 200f);

        tweens.Add(backgroundPattern.DOFade(backgroundPatternTargetAlpha, 1f).SetDelay(0.25f));

        logoCanvasGroup.DOFade(1f, 1f).SetDelay(1f).OnComplete(()=>
        {
            startText.DOColor(Color.white, 0.75f).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);
        });
    }

    public void OnClick()
    {
        if(clicked)
            return;

        clicked = true;
        

        Unsubscribe();

        if(SaveManager.currentSave.sfx)
            Audio.Play(shimmer, Channel.UI);

        tweens.Add(DOVirtual.DelayedCall(shimmer.length, ()=>{
            MusicManager.Play(Music.Menu, false);
        }));

        startText.transform.DOKill();

        startText.DOKill();
        startText.DOFade(0f, 0.1f);

        logoPunchButton.OnClick();

        logo.DOLocalMove(logoTargetPosition, 1f).SetEase(Ease.InOutQuad);

		tweens.Add(DOVirtual.DelayedCall(0.75f, ()=>{
            shopCamera.gameObject.SetActive(true);
        }));

		tweens.Add(DOVirtual.DelayedCall(1f, ()=>{
            MenuWebRequestManager.FetchUnrecivedRewards();
            buttonsCanvasGroup.gameObject.SetActive(true);
            buttonsCanvasGroup.DOFade(1f, 0.5f);
            tweens.Add(buttonsParent.DOLocalMove(buttonsTargetPosition, 0.5f).SetEase(Ease.OutQuad).OnComplete(()=>
            {
                profileButton.TryShow();
                
                if(!SaveManager.SaveExsists())
                    SaveManager.Save();
                
            }));
        }));

        if(!SaveManager.currentSave.noBrightFlashesMode)
        {
            flashImg.gameObject.SetActive(true);
            tweens.Add(flashImg.DOFade(1f, 0.1f).OnComplete(()=>
            {
                flashImg.DOFade(0f, 0.4f).OnComplete(()=>{
                    flashImg.gameObject.SetActive(false);
                });
                container.SetActive(false);
            }));
        }
        else
        {
            container.SetActive(false);
        }
    }

    private void On_TouchMaintained(TouchInfo touch)
    {
        if(touch.pickedUIElement != touchToStartRect.gameObject)
            return;

        OnClick();
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
		logoCanvasGroup.DOKill();
		startText.transform.DOKill();
		startText.DOKill();
		flashImg.DOKill();
		logo.DOKill();

		foreach(Tween t in tweens)
			t?.Kill();
	}
}
