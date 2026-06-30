using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class TransitionSingleton : MonoBehaviour
{
    public static TransitionSingleton instance;

    public Image background;
    public TextMeshProUGUI flyByText;
    public RectTransform flyByTextRectTransform;

    public Color previousLevelColor;

    private List<Tween> tweens = new List<Tween>();

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        background = GetComponentInChildren<Image>();

        previousLevelColor = Camera.main.backgroundColor;
		
		//CloseBlinds(previousLevelColor, true);
        //OpenBlinds();
    }

    private void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (GlobalSingleton.skipBlindsOpenOnNextSceneLoad)
        {
            GlobalSingleton.skipBlindsOpenOnNextSceneLoad = false;
            return;
        }

        this.Delay1Frame(()=>{
            OpenBlinds();
        });
    }

	public void SetColor(Color c, float alpha)
	{
		background.color = new Color(c.r, c.g, c.b, alpha);
	}

    public Tween FadeToColor(Color color, float duration, System.Action onComplete)
    {
        return background.DOColor(color, duration).SetEase(Ease.Linear).OnComplete(()=> onComplete());
    }

    public void OpenBlinds(bool instant = false, System.Action callback = null)
	{
        KillAllTweens();

        background.raycastTarget = false;

        tweens.Add(
           this.flyByText.DOFade(0f, instant ? 0f : Globals.gameConstants.transitionInDuration).SetUpdate(true).SetEase(Ease.OutQuad)
        );

        tweens.Add(
           background.DOFade(0f, instant ? 0f : Globals.gameConstants.transitionInDuration).SetUpdate(true).SetEase(Ease.OutQuad).OnComplete(() =>
           {
               background.enabled = false;

               if (callback != null)
                   callback();
           })
       );
	}

    public void CloseBlinds(Color color, bool instant = false, System.Action callback = null)
    {
        KillAllTweens();

        SetColor(color , 0f );

        background.enabled = true;
        background.raycastTarget = true;


        tweens.Add(
            background.DOFade(1f, instant ? 0f : Globals.gameConstants.transitionOutDuration).SetUpdate(true).SetEase(Ease.InQuad).OnComplete(() =>
            {
                Resources.UnloadUnusedAssets();
                System.GC.Collect();

                if (callback != null)
                    callback();
            })
        );

    }

    public void ShowFlyByText(TranslationKey key, Language language, float duration, bool instant = false, System.Action onExit = null, System.Action onComplete = null)
    {
        flyByText.Translate(key, language, FontType.Stylized_Outlined, true);

        DoFlyByText(duration, instant, onExit, onComplete);
    }

    public void ShowFlyByText(string text, Language language, bool isRightToLeftText, float duration, bool instant = false, System.Action onExit = null, System.Action onComplete = null)
    {
        flyByText.text = text;
        flyByText.isRightToLeftText = isRightToLeftText;
        flyByText.font = Globals.accessibilityConstants.GetFont(SaveManager.currentSave.openDyslexic, FontType.Stylized_Outlined, flyByText.font, language);

        DoFlyByText(duration, instant, onExit, onComplete);
    }

    private void DoFlyByText(float duration, bool instant = false, System.Action onExit = null, System.Action onComplete = null)
    {
        float enterDuration = instant ? 0f : Globals.gameConstants.transitionInDuration;
        float exitDuration = instant ? 0f : Globals.gameConstants.transitionOutDuration;

        flyByText.alpha = 0;
        flyByTextRectTransform.gameObject.SetActive(true);
        flyByTextRectTransform.anchoredPosition = new Vector2(-100f, flyByTextRectTransform.anchoredPosition.y);

        tweens.Add(
            flyByText.DOFade(1f, enterDuration).SetUpdate(true).SetEase(Ease.InQuad)
        );

        tweens.Add(
            this.flyByTextRectTransform.DOAnchorPosX(0f, enterDuration).SetUpdate(true).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                tweens.Add(
                    DOVirtual.DelayedCall(duration, () =>
                    {
                        onExit?.Invoke();

                        tweens.Add(
                            this.flyByTextRectTransform.DOAnchorPosX(100f, exitDuration).SetUpdate(true).SetEase(Ease.OutQuad).OnComplete(()=> { onComplete?.Invoke(); })
                        );

                        tweens.Add(
                            flyByText.DOFade(0f, exitDuration).SetUpdate(true).SetEase(Ease.OutQuad).OnComplete(()=>{
                                flyByTextRectTransform.gameObject.SetActive(false);
                            })
                        );
                    }).SetUpdate(true)
                );
            })
        );
    }

	public bool IsTransitioning()
	{
		return background.enabled;
	}

	/*
    private void On_SuspensionEvent(bool isSuspended)
    {
        if(!IsTransitioning())
            return;

        background.enabled = !isSuspended;
    }
	*/

    private void On_LevelReset(bool manual)
    {
        //Dont cancel if reset was not triggered by player.
        if (!manual)
            return;

        KillAllTweens();

        flyByText.color = flyByText.color.SetAlpha(0);
        background.color = background.color.SetAlpha(0);

        background.raycastTarget = false;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += SceneLoaded;
        //SuspensionManager.On_SuspensionEvent += On_SuspensionEvent;
        GameMaster.On_LevelReset += On_LevelReset;
    }

    private void Unsubscribe()
    {
        SceneManager.sceneLoaded -= SceneLoaded;
        //SuspensionManager.On_SuspensionEvent -= On_SuspensionEvent;
        GameMaster.On_LevelReset -= On_LevelReset;
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

    public void KillAllTweens()
    {
        if (tweens == null)
            return;

        foreach (Tween t in tweens)
            t?.Kill();

        tweens = new List<Tween>();
    }
}
