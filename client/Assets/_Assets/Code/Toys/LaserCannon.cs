using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LaserCannon : MonoBehaviour, ISuspendable
{
    public Transform child;
    public Gradient gradient;

    public LaserGraphic laserGraphic;
    public LaserBeam beam;

    [HideInInspector]
    public bool isFiring;
    [HideInInspector]
    public bool isPreparing;
    [HideInInspector]
    public float startFireTime;

    private LevelEntity entity;
    private Direction8 direction;

    public const float fireDuration = 1f;
    private const float maxDelay = 1f;
    public const float warmUpDuration = 1f;
    
    private float t;
    private float colorT;
    private AudioPlayer beamLoop;

    private bool isPowered = false;

    private List<Tween> tweens = new List<Tween>();
	
	private bool _isSuspended;
	public bool isSuspended {
		get{
			return _isSuspended;
		}
		private set{
			_isSuspended = value;
		}
	}

    private void Awake()
    {
        entity = GetComponentInParent<LevelEntity>(); 

        direction = ((Direction8Gizmo)entity.gizmo).direction;
        child.eulerAngles = new Vector3(0,0,direction.ToDegree());

        colorT = Random.Range(0f, 1f);
        laserGraphic.SetColor(gradient.Evaluate(colorT), 0.5f);
    }

    private void Update()
    {		
		if(isSuspended)
			return;

        if(isFiring)
        {
            float time = Time.time * 3.75f;
            colorT = time - Mathf.FloorToInt(time);
        }

        laserGraphic.SetColor(gradient.Evaluate(colorT), isFiring ? 1f : 0.5f);    
    }

    private void LateUpdate()
    {
        if(!GameMaster.instance.IsPlaying())
            return;

		if(isSuspended)
			return;
        
        if(entity.inputNode.HasConnections())
        {
            if(entity.inputNode.IsPowered())
            {
                if(!isPowered)
                {
                    isPowered = true;
                    Fire();
                }
            }
            else
            {
                isPowered = false;
                Stop();
            }
        }
        else
        {
            if(!isFiring && !isPreparing)
                t += Time.deltaTime * SaveManager.currentSave.gameSpeed;

            if(t > maxDelay)
            {
                Prepare();
                t = 0;
            }
        }

        if(isFiring)
            beam.AdjustLength(direction.ToVector());
    }

    private void Prepare()
    {
        KillAllTweens();
        Stop();

        isPreparing = true;
        beam.Prepare(direction.ToVector());

        Audio.Play(SFX.instance.level.laser.warmup, Channel.Game);

        tweens.Add(DOVirtual.DelayedCall(warmUpDuration/(SaveManager.currentSave.gameSpeed + 0.1f), ()=>
        {    
            Fire();
        }, false));
    }

    private void Fire()
    {
        isFiring = true;
        startFireTime = Time.time;
        isPreparing = false;

        beam.Prepare(direction.ToVector());
        beam.Fire();

        beamLoop = Audio.Play(SFX.instance.level.laser.beam, Channel.Game)?.SetLoop(true).SetPitch(Random.Range(0.9f, 1.1f));

        if(!entity.inputNode.HasConnections())
        {
            tweens.Add(DOVirtual.DelayedCall(fireDuration/(SaveManager.currentSave.gameSpeed + 0.1f), ()=>
            {
                Stop();
            }, false)); 
        }
    }

    public void Stop()
    {
        isPreparing = false;
        isFiring = false;
        beamLoop?.Kill();
        beamLoop = null;
        beam.Stop();
    }

    private void Reset()
    {
        t = 0f;
        isPowered = false;

        Stop();

        KillAllTweens();
    }

    private void On_EnterPlayMode()
    {
        Reset();

        if(GameMaster.instance.IsPlaying() && entity.inputNode.HasConnections())
		{
            this.DelayEndOfFrame(()=>
            {
                if(entity.inputNode.IsPowered())
                {
                    if(!isPowered)
                    {
                        isPowered = true;
                        Fire();
                    }
                }
            });
        }
    }

    private void On_LevelReset(bool manual) => Reset();

    private void OnEnable()
    {
        GameMaster.On_EnterPlayMode += On_EnterPlayMode;
        GameMaster.On_LevelReset += On_LevelReset;
        GameMaster.On_ExitPlayMode += Reset;

		SuspensionManager.On_SuspensionEvent += On_SuspensionEvent;
    }

    private void Unsubscribe()
    {
        GameMaster.On_EnterPlayMode -= On_EnterPlayMode;
        GameMaster.On_LevelReset -= On_LevelReset;
        GameMaster.On_ExitPlayMode -= Reset;

		SuspensionManager.On_SuspensionEvent -= On_SuspensionEvent;
    }

    private void OnDestroy()
    {
        Unsubscribe();
        KillAllTweens();

        beamLoop?.Kill();
    }

    private void OnDisable()
    {
        Unsubscribe();
        KillAllTweens();
    }

    private void KillAllTweens()
    {
        if(tweens != null)
            foreach(Tween t in tweens)
            {
                if(t != null)
                    t.Kill();
            }

        tweens = new List<Tween>();
    }

	public void On_SuspensionEvent(bool isSuspended)
	{
		Suspend(isSuspended);
	}

	public void Suspend(bool suspend)
	{
		this.isSuspended = suspend;

		if(tweens != null)
            foreach(Tween t in tweens)
            {
				if(suspend)
					t?.Pause();
				else
					t?.Play();
            }
	}
}
