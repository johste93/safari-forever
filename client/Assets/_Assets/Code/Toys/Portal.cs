using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FSM_CharacterController2D;
using StandardCharacterController2D;

public class Portal : MonoBehaviour, ISuspendable
{
	public Transform container;
    public GameObject graphic;
	public bool animate = true;
	public Color color;

	private float length = 0.2f;
	private float pingPong;

	private Portals parent;

	private List<Tween> tweens = new List<Tween>();

	public void Initalize(Portals parent)
	{
		this.parent = parent;
	}

	
	private void OnTriggerEnter2D(Collider2D other)
    {
		if(!GameMaster.instance.IsPlaying())
			return;

		if(other.isTrigger)
			return;

         if(!parent.IsOpen())
			return;

		if(parent.ObjectIsBeingTransported(other, out _))
			return;


		ITeleportable teleportableObject = other.GetComponent<ITeleportable>();
		if(teleportableObject == null)
			return;

		//Teleport(other.transform);

		parent.OnEnterPortal(other, this, teleportableObject);
    }
	

	public void PunchPortal()
	{
		animate = false;
		graphic.transform.DOComplete();
		tweens.Add(graphic.transform.DOPunchScale(Vector3.one * 0.8f, 0.3f / (SaveManager.currentSave.gameSpeed + 0.1f), 1).SetEase(Ease.OutBack).OnComplete(()=>
		{
			animate = true;
		}));
	}

	private void Awake()
	{
		Animate();
	}

	private void Update()
	{
		Animate();
	}

	private void Animate()
	{
		if(!animate)
			return;

		pingPong = Mathf.PingPong(Time.time * length, length) - (length*0.5f);
		graphic.transform.localScale = new Vector3(1f + pingPong,1f - pingPong,1f);
	}

	public void Open(bool instant = false)
	{
		if(transform == null)
			return;

		container.DOKill();
		container.DOScale(Vector2.one, instant ? 0f : 0.3f).SetEase(Ease.OutBack).OnComplete(()=>
		{
			animate = true;
		});
	}

	public void Close(bool instant = false)
	{
		if(transform == null)
			return;
			
		animate = false;
		container.DOKill();
		container.DOScale(Vector2.one * 0.3f, instant ? 0f : 0.15f).SetEase(Ease.InBack);
	}

	private void On_EnterPlayMode()
	{
		Collider2D collider2D = Physics2D.OverlapCircle(transform.position, 0.1f, Globals.gameConstants.whatIsPlayer);
		if (collider2D != null)
		{
			OnTriggerEnter2D(collider2D);
		}
	}

	public void KillAllTweens()
	{
		container.DOComplete();
		graphic.transform.DOComplete();

		foreach(Tween tween in tweens)
		{
			tween.Kill();
		}
		tweens = new List<Tween>();
	}

	private void OnEnable()
	{
		GameMaster.On_EnterPlayMode += On_EnterPlayMode;
		SuspensionManager.On_SuspensionEvent += On_SuspensionEvent;
	}

	private void OnDisable()
	{
		Unsubscribe();
	}

	private void OnDestroy()
	{
		KillAllTweens();
		Unsubscribe();
	}

	private void Unsubscribe()
	{
		GameMaster.On_EnterPlayMode -= On_EnterPlayMode;
		SuspensionManager.On_SuspensionEvent -= On_SuspensionEvent;
	}

	public void On_SuspensionEvent(bool isSuspended)
	{
		Suspend(isSuspended);
	}

	public void Suspend(bool suspend)
	{
		foreach(Tween t in tweens)
		{
			if(suspend)
				t?.Pause();
			else
				t?.Play();
		}
	}
}
