using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Card : MonoBehaviour
{
    public RectTransform root;
    public Image front;
    public Image back;
    public Image shadow;
    public CanvasGroup canvasGroup;

    public GameObject[] rewards;

    private int cardIndex;
    private Vector3 targetPosition;

    private const float tweenDuration = 0.5f;

    private List<Tween> tweens = new List<Tween>();

    public void OnClick()
    {
        GetComponentInParent<CardsWindow>().TurnCard(cardIndex);
    }

    public void View(bool instant, System.Action onComplete)
    {
        Elevate(instant ? 0f : tweenDuration);
        tweens.Add(root.DORotate(new Vector3(0, 180, 0), instant ? 0f : tweenDuration).SetEase(Ease.OutQuad));

        tweens.Add(DOVirtual.DelayedCall(1f, ()=>{
            Vanish();
            onComplete?.Invoke();
        }));
    }

    public void Vanish()
    {
        tweens.Add(canvasGroup.DOFade(0f, tweenDuration).OnComplete(()=>{
            gameObject.SetActive(false);
        }));
    }

    public void Hide(bool instant)
    {
        Lower(instant ? 0f : tweenDuration);
        tweens.Add(root.DORotate(new Vector3(0, 0.01f, 0), instant ? 0f : tweenDuration).SetEase(Ease.OutQuad));
    }

    public void Initalize(int cardIndex, Vector3 targetPosition)
    {
        this.cardIndex = cardIndex;
        this.targetPosition = targetPosition;
    }

    public void Elevate(float duration)
    {
        root.SetAsLastSibling();
        tweens.Add(root.DOScale(2f, duration).SetEase(Ease.OutQuad));
        tweens.Add(root.DOAnchorPos(Vector2.zero, duration).SetEase(Ease.OutQuad));
    }

    public void Lower(float duration)
    {
        tweens.Add(root.DOScale(1f, duration).SetEase(Ease.OutQuad));
        tweens.Add(root.DOAnchorPos(targetPosition, duration).SetEase(Ease.OutQuad));
    }

    public void SetReward(Hat hat)
    {
        int numberOfHats = System.Enum.GetNames(typeof(Hat)).Length;
        for(int i = 0; i < numberOfHats; i++)
            rewards[i].SetActive((Hat)i == hat);
    }

    private void Update()
    {
        front.gameObject.SetActive(FacingCamera(front.transform));
        back.gameObject.SetActive(FacingCamera(back.transform));
    }

    private bool FacingCamera(Transform transform)
    {
        Vector3 dirToSprite = transform.position - Camera.main.transform.position;
        float d = Vector3.Dot(transform.right, dirToSprite.normalized);

        if (d > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnDisable()
    {
        KillAllTweens();
    }

    public void KillAllTweens()
    {
        foreach(Tween t in tweens)
        {
            if(t == null)
                continue;
            
            t.Kill();
        }
        tweens = new List<Tween>();
    }
}
