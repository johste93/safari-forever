using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Cannon : MonoBehaviour, ISuspendable
{
    public GameObject bulletPrefab;
    public Transform child;

    private float maxDelay = 2.5f;
    private float t;

    private LevelEntity entity;
    private bool isPlayMode;

    private Direction8 direction;

    private Tween positionTween;
    private Tween scaleTween;

    private float cooldown;
	private bool isSuspended;
    private bool isPowered;

    private static int lastFramePlayedSound = 0;

    private void Awake()
    {
        entity = GetComponentInParent<LevelEntity>(); 

        direction = ((Direction8Gizmo)entity.gizmo).direction;
        child.eulerAngles = new Vector3(0,0,direction.ToDegree());
    }

    private void LateUpdate()
    {
        if(!GameMaster.instance.IsPlaying())
            return;
        
		if(isSuspended)
			return;

        if(entity.inputNode.HasConnections())
        {
            if(cooldown > 0)
            {
                cooldown -= Time.deltaTime * SaveManager.currentSave.gameSpeed;
                return;
            }

            if(entity.inputNode.IsPowered())
            {
                if(!isPowered)
                {
                    isPowered = true;
                    Fire();
                    t = 0f;
                    return;
                }
            }
            else
            {
                if(isPowered)
                {
                    isPowered = false;
                    t = 0;
                }
                return;
            }
        }

        t += Time.deltaTime * SaveManager.currentSave.gameSpeed;
        
        if(t > maxDelay)
        {
            Fire();
            t = 0;
        }
    }

    private void Fire()
    {
        if(lastFramePlayedSound != Time.frameCount)
        {
            lastFramePlayedSound = Time.frameCount;
            Audio.Play(SFX.instance.level.cannon.fire, Channel.Game);
        }

        GameObject lastSpawn = Instantiate(bulletPrefab, transform.position, Quaternion.identity, transform);
        Bullet lastBullet = lastSpawn.GetComponent<Bullet>();
        lastBullet.Initalize(direction);

        child.DOComplete();
        positionTween = child.DOLocalMove(-child.up * 0.33f, 0f).SetEase(Ease.OutQuad).OnComplete(()=>
        {
            positionTween = child.DOLocalMove(Vector2.zero, 0.5f).SetEase(Ease.OutQuad);
        });
        scaleTween = child.DOScale(new Vector2(1.3f, 0.6f), 0.1f).SetEase(Ease.OutQuad).OnComplete(()=>
        {
            scaleTween = child.DOScale(Vector2.one, 0.2f).SetEase(Ease.Linear);
        });

        cooldown = 0.1f;


        if(Bullet.GetBulletCount() > Globals.gameConstants.maxNumberOfBullets)
        {
            Bullet.KillFirst();
        }
    }

    private void Reset()
    {
        isPowered = false;
        t = 0f;
        cooldown = 0f;

        if(positionTween != null)
        {
            positionTween.Kill();
            positionTween = null;
        }

        if(scaleTween != null)
        {
            scaleTween.Kill();
            scaleTween = null;
        }

        child.localScale = Vector3.one;
        child.localPosition = Vector3.zero;
    }

    private void On_LevelReset(bool manual) => Reset();

    private void OnEnable()
    {
        GameMaster.On_EnterPlayMode += Reset;
        GameMaster.On_ExitPlayMode += Reset;
        GameMaster.On_LevelReset += On_LevelReset;

		SuspensionManager.On_SuspensionEvent += On_SuspensionEvent;
    }

    private void Unsubscribe()
    {
        GameMaster.On_EnterPlayMode -= Reset;
        GameMaster.On_ExitPlayMode -= Reset;
        GameMaster.On_LevelReset -= On_LevelReset;

		SuspensionManager.On_SuspensionEvent -= On_SuspensionEvent;
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
		positionTween?.Kill();
		scaleTween?.Kill();
	}

	public void On_SuspensionEvent(bool isSuspended)
	{
		Suspend(isSuspended);
	}

	public void Suspend(bool suspend)
	{
		this.isSuspended = suspend;
		if(suspend)
		{
			positionTween?.Pause();
			scaleTween?.Pause();
		}
		else
		{
			positionTween?.Play();
			scaleTween?.Play();
		}
	}
}
