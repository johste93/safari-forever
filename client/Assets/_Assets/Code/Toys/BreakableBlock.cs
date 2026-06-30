using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FSM_CharacterController2D;

public class BreakableBlock : Block, IBreakable, IHeadSmashable
{
    public BoxCollider2D boxCollider2D;
    public Transform container;
    public GameObject poff;
	public GameObject broken;

    private int frameLastHit = -1;
    
    public void OnHeadSmashed(Character character) 
    {
        character.controller.motion.rawVelocity.y = Mathf.Min(0f, character.controller.motion.rawVelocity.y);
        //character.controller.stateController.OverideState(State.Falling);
        TriggerHit(null);
    } 

    private void OnCollisionEnter2D(Collision2D other)
	{
        return;
        if(!Globals.gameConstants.whatIsPlayer.Contains(other.gameObject.layer))
            return;

        //if(GetCollisionRelativeDirection((BoxCollider2D)other.collider) == Direction4.Up)
            //TriggerHit(other.gameObject);	
        
        
        Vector2 hitNormal = other.GetContact(0).normal;
        if((hitNormal * -1) == Direction4.Down.ToVector())
        {
            if(IsCollisionWithinRange((BoxCollider2D)other.collider))
                TriggerHit(other.gameObject);
        }
    }

    private void TriggerHit(GameObject other)
    {
        if(frameLastHit == Time.frameCount)
            return;

        frameLastHit = Time.frameCount;

		float duration = 0.1f;
		container.DOComplete();
		container.DOLocalMove(Vector3.up * 0.3f, 0).SetEase(Ease.Linear).OnComplete(()=>
		{
			//Particle Effect
			container.DOLocalMove(Vector2.zero, duration).SetEase(Ease.Linear);
		});

		OnHit(other);
    }

    private bool IsCollisionWithinRange(BoxCollider2D other)
    {
        float min = boxCollider2D.bounds.center.x - (boxCollider2D.size.x*0.5f);
        float max = boxCollider2D.bounds.center.x + (boxCollider2D.size.x*0.5f);

        return (other.bounds.max.x > min && other.bounds.max.x < max) || (other.bounds.min.x < max && other.bounds.min.x > min);
    }


    protected virtual void OnHit(GameObject other)
	{       
        Break();
	}


	public void Break()
	{
		boxCollider2D.enabled = false;
        container.gameObject.SetActive(false);
        GameObject go = Instantiate(poff, transform.position, Quaternion.identity) as GameObject;
        go.transform.localScale = Vector3.one * 1.4f;

        Audio.Play( SFX.instance.level.breakableBlock.smash.randomClip, Channel.Game ).SetPitch(Random.Range(0.9f, 1.1f));

		broken.SetActive(true);
	}

    private void Reset()
    {
        frameLastHit = -1;
        boxCollider2D.enabled = true;
        container.gameObject.SetActive(true);
		broken.SetActive(false);
    }

    private void On_LevelReset(bool manual) => Reset();

    protected override void OnEnable()
    {
        base.OnEnable();
        GameMaster.On_LevelReset += On_LevelReset;
        GameMaster.On_ExitPlayMode += Reset;
    }

    protected override void Unsubscribe()
    {
        base.Unsubscribe();
        GameMaster.On_LevelReset -= On_LevelReset;
        GameMaster.On_ExitPlayMode -= Reset;
    }

	protected override void OnDestroy()
	{
		base.OnDestroy();
		KillAllTweens();
	}

	private void KillAllTweens()
	{
		container.DOKill();
	}
}
