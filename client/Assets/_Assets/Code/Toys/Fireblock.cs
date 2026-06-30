using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Fireblock : MonoBehaviour
{
    public Fire fire;
    public BoxCollider2D boxCollider2D;

    public SpriteRenderer holeRenderer;
    public Color litColor;
    public Color defaultColor;

    public SpriteRenderer fireRenderer;
    public SpriteRenderer alphaRenderer;
    private Coroutine lightFire;

    private LevelEntity entity;

    private void Awake()
    {
        entity = GetComponentInParent<LevelEntity>();
        //fireRenderer.color = fromColor;

        alphaRenderer.DOFade(0f, Random.Range(0.2f, 0.7f)).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad);
        //fireRenderer.DOColor( Color.Lerp(fromColor, toColor, Random.Range(0f, 1f)), Random.Range(0.5f, 0.7f)).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad);
    }

    private void Start()
    {
        if(entity.GetSerializableData().gizmoDirection.HasValue)
        {
            transform.eulerAngles = new Vector3(0,0,((Direction4)entity.GetSerializableData().gizmoDirection).ToDegree());
        }
        else
        {
            transform.eulerAngles = Vector3.zero;
        }
        
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if(!Globals.gameConstants.whatIsPlayer.Contains(other.gameObject.layer))
            return;

        if(lightFire != null)
            return;

        float duration = 0.5f/SaveManager.currentSave.gameSpeed;

        Vector2 hitNormal = other.GetContact(0).normal;
        if((hitNormal * -1) == ((Direction4) entity.GetSerializableData().gizmoDirection).ToVector())
        {
            holeRenderer.DOColor(litColor, duration).SetEase(Ease.Linear);
            lightFire = this.Delay(duration, ()=>
            {
                LightFire();
            });
        }
    }

    private void LightFire()
    {
        fire.transform.DOKill();
        fire.transform.localScale = Vector3.zero;
        fire.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
        fire.gameObject.SetActive(true);
        fire.circleCollider2D.enabled = true;
        lightFire = this.Delay(1f/SaveManager.currentSave.gameSpeed, ()=>
        {
            fire.gameObject.SetActive(false);
            lightFire = null;
            holeRenderer.color = defaultColor;
        });
    }

    private void On_EnterPlayMode()
    {
        boxCollider2D.enabled = true;
        fire.circleCollider2D.enabled = true;
    }

    private void On_ExitPlayMode()
    {
        boxCollider2D.enabled = false;
        Reset();
    }

    private void Reset()
    {
        fire.transform.DOKill();
        fire.circleCollider2D.enabled = false;
        fire.gameObject.SetActive(false);
        
        if(lightFire != null)
            StopCoroutine(lightFire);

        lightFire = null;
        fire.circleCollider2D.enabled = false;

        holeRenderer.DOKill();
        holeRenderer.color = defaultColor;
    }

    private void On_LevelReset(bool manual) => Reset();

    private void OnEnable()
    {
        GameMaster.On_LevelReset += On_LevelReset;
        GameMaster.On_EnterPlayMode += On_EnterPlayMode;
        GameMaster.On_ExitPlayMode += On_ExitPlayMode;
    }

    private void Unsubscribe()
    {
        GameMaster.On_LevelReset -= On_LevelReset;
        GameMaster.On_EnterPlayMode -= On_EnterPlayMode;
        GameMaster.On_ExitPlayMode -= On_ExitPlayMode;
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
		alphaRenderer.DOKill();
		holeRenderer.DOKill();
		fire.transform.DOKill();
	}
}
