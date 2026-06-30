using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM_CharacterController2D;
using DG.Tweening;

public class GhostBlock : Block
{
    public BoxCollider2D boxCollider2D;
    public Transform face;

    public static bool isVisible = true;
    public static int lastChange;

    private FSM_CharacterController characterController;

    private Tween shakeTween;

    protected override void Start()
    {
        base.Start();
        shakeTween = face.DOShakePosition(1f, 0.02f, 15, 90, false, false).SetEase(Ease.Linear).SetLoops(-1).Pause();
    }

    private void UpdateBlock()
    {
        boxCollider2D.enabled = isVisible;

        if(isVisible)
            shakeTween.Pause();
        else
            shakeTween.Play();

        for(int i = 0; i < 5; i++)
            spriteRenderers[i].color = spriteRenderers[i].color.SetAlpha(isVisible ? 1f : 0.25f);
    }

    private void Reset()
    {
        isVisible = true;
        UpdateBlock();

        if(characterController != null)
            characterController.stateController.OnStateChanged -= OnStateChanged;
    }

    private void OnStateChanged(State previousState, State newState)
    {
        if(newState != State.Jumping)
            return;

        if(lastChange != Time.frameCount)
        {
            isVisible = !isVisible;
            lastChange = Time.frameCount;

            if(isVisible)
            {
                Collider2D player;
                player = Physics2D.OverlapBox(transform.position + (Vector3)boxCollider2D.offset, boxCollider2D.size, 0, Globals.gameConstants.whatIsPlayer);
                if(player != null)
                {
                    Character character = player.GetComponent<Character>();
                    if(!character.IsDead())
                    {
                        //Splat!
                        Audio.Play( SFX.instance.level.crush.crush, Channel.Game);
                        character.Hurt();
                    }
                }
                    
            }
        }

        UpdateBlock();
    }

    private void On_PlayerStartedRunning(FSM_CharacterController characterController)
    {
        this.characterController = characterController;
        this.characterController.stateController.OnStateChanged += OnStateChanged;
    }

    private void On_LevelReset(bool manual) => Reset();

    protected override void OnEnable()
    {
        base.OnEnable();
        GameMaster.On_LevelReset += On_LevelReset;
        GameMaster.On_ExitPlayMode += Reset;
        GameMaster.On_PlayerStartedRunning += On_PlayerStartedRunning;

        Reset();
    }

    protected override void Unsubscribe()
    {
        base.Unsubscribe();
        GameMaster.On_LevelReset -= On_LevelReset;
        GameMaster.On_ExitPlayMode -= Reset;
        GameMaster.On_PlayerStartedRunning -= On_PlayerStartedRunning;
        Reset();
    }

	protected override void OnDestroy()
	{
		base.OnDestroy();
		KillAllTweens();
	}

	private void KillAllTweens()
	{
		face.DOKill();
		shakeTween?.Kill();
	}
}
