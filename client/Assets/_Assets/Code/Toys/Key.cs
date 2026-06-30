using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Key : MonoBehaviour, ISuspendable
{
    public GameObject poff;
    public GameObject graphic;
    public CircleCollider2D circleCollider2D;

    private Character target;
    private Vector3 offset = new Vector3(0, 0.5f, 0);
    private int keyIndex;
    private float bobbingOffset;

	private Tween tween;
	private bool isSuspended;

    public delegate void UnlockEvent();
    public static UnlockEvent On_Unlock;


    private void Awake()
    {
        bobbingOffset = Random.Range(0f, 1f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {   
        if(!GameMaster.instance.IsPlaying())
            return;

        if(target != null)
            return;

        if(!Globals.gameConstants.whatIsPlayer.Contains(other.gameObject.layer))
            return;
        
        Follow(other.gameObject.GetComponent<Character>());
    }

    private void Follow(Character character)
    {
        target = character;
        target.keys.Add(this);
        circleCollider2D.enabled = false;
        keyIndex = target.keys.Count;
        On_Unlock += OnUnlock;
        
        tween?.Complete();
        tween = graphic.transform.DOPunchScale(Vector3.one * 0.3f, 0.3f, 1);
    }

    private void Update()
    {
		if(isSuspended)
			return;

        graphic.transform.localPosition = new Vector3(0,Mathf.Sin((Time.time+bobbingOffset)*2f)*0.15f,0);

        if(target == null)
            return;

        Vector3 targetPos = keyIndex == 1 ? target.transform.position + offset : target.keys[keyIndex-2].transform.position;
        Vector3 dir = (transform.position - targetPos).normalized * 0.25f; 
        targetPos += dir;

        float t = Mathf.Min(Vector2.Distance(transform.position, targetPos), 5f) / 5f;
        transform.position = Vector3.Lerp(transform.position, targetPos, t);
    }

    public void Unlock()
    {
        //Play unlock sound
        target.keys.Remove(this);
        target = null;

        Instantiate(poff, transform.position, Quaternion.identity);
        graphic.SetActive(false);

		On_Unlock -= OnUnlock;
		KillAllTweens();

        On_Unlock?.Invoke();
		
    }

    public void Reset()
    {
        KillAllTweens();
        graphic.SetActive(true);
        circleCollider2D.enabled = true;
        target = null;
	
		On_Unlock -= OnUnlock;
		KillAllTweens();
    }

    private void OnUnlock()
    {
        if(target == null)
            return;

        keyIndex--;
    }

	private void OnEnable()
	{
		SuspensionManager.On_SuspensionEvent += On_SuspensionEvent;
	}

    private void Unsubscribe()
    {
		SuspensionManager.On_SuspensionEvent -= On_SuspensionEvent;
        On_Unlock -= OnUnlock;
        KillAllTweens();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void KillAllTweens()
    {
		tween?.Kill();
        graphic.transform.localScale = Vector3.one;
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
			tween?.Pause();
		}
		else
		{
			tween?.Play();
		}
	}
}
