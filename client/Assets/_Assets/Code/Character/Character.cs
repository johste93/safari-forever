using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FSM_CharacterController2D;

public class Character : MonoBehaviour, ISuspendable, ITeleportable
{
    public FSM_CharacterController controller;
    public Transform pivotTransform;
    public AnimalSlime slime;

    private bool instagibbed;

    private float timeOfBirth;

    private List<Tween> tweens = new List<Tween>();

    public List<Key> keys = new List<Key>();

    private void Awake()
    {
        timeOfBirth = Time.time;
    }

    //Kill self if outside of screen!
    private void LateUpdate()
    {
        if (GameMaster.instance == null || !GameMaster.instance.IsPlaying())
            return;

        Bounds b = Camera.main.OrthographicBounds();
        b.extents += Vector3.one;

        if (transform.position.y > b.max.y ||
            transform.position.y < b.min.y ||
            transform.position.x > b.max.x ||
            transform.position.x < b.min.x)
            {
                if (controller.boxCollider2D.enabled == false)
                    return;

                Hurt();
            }
            
    }

    public Vector3 GetPortalOffset()
    {
        return new Vector3(0,  -controller.boxCollider2D.offset.y, 0);
    }

    public Vector2 GetSize()
    {
        return controller.boxCollider2D.size;
    }

    public void Wedge(Vector2 wedge, float angle)
    {
        pivotTransform.localPosition = wedge;
        pivotTransform.eulerAngles = new Vector3(0,0, angle);

        if(wedge.magnitude == 0f)
        {
            slime.Disable();
        }
        else
        {
            slime.Enable(SignVector2(wedge).ToDirection());
        }
    }

    private Vector2 SignVector2(Vector2 vector)
    {
        return new Vector2(Mathf.Approximately(vector.x, 0) ? 0 : (int)Mathf.Sign(vector.x), Mathf.Approximately(vector.y, 0) ? 0 : (int)Mathf.Sign(vector.y));
    }

    public void Hurt(bool invalidSpawn = false)
    {
        //if(controller.stateController.currentState.Equals(State.Inactive))
        //return;

        //Debug.Log("Hurt: " + gameObject.GetInstanceID() + " invalidSpawn: " + invalidSpawn);  
        Die(invalidSpawn);
    }

    private void Die(bool invalidSpawn)
    {
        if (IsDead())
            return;

        controller.stateController.OverideState(State.Dead);
        controller.enabled = false;
        controller.boxCollider2D.enabled = false;
        slime.Disable();

        PlayDeathAnimation();



        if (GlobalSingleton.mode == GameMode.Create)
            instagibbed = Time.time - timeOfBirth <= 0.6f;

        if (GameMaster.instance != null)
            GameMaster.instance.PlayerDied(controller, instagibbed, invalidSpawn);
    }

    public void PlayDeathAnimation()
    {
        Audio.Play(SFX.instance.character.scream.randomClip, Channel.Game);

        tweens.Add(transform.DORotate(new Vector3(0, 0, 360f * controller.motion.runningDirection), 360f, RotateMode.FastBeyond360).SetRelative(true).SetLoops(-1).SetSpeedBased(true).SetEase(Ease.Linear));
        tweens.Add(transform.DOMoveY(3f, 0.2f).SetRelative(true).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            tweens.Add(transform.DOMoveY(-50, 34f).SetRelative(true).SetSpeedBased(true).SetEase(Ease.InQuad));
        }));
    }

    public bool IsDead()
    {
        return controller.stateController.currentState.Equals(State.Dead);
    }

    public void ReadyPlayer()
    {
        timeOfBirth = Time.time;
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        MonoBehaviour mono = other.GetComponent<MonoBehaviour>();

        if (mono is IStompable)
        {
            float wiggleRoom = 0.5f;
            if (controller.boxCollider2D.bounds.min.y > other.bounds.max.y - wiggleRoom &&
                controller.boxCollider2D.bounds.max.x > other.bounds.min.x - wiggleRoom)
            {
                ((IStompable)mono).OnStomped(this);
                return;
            }
        }

        if (mono is IHeadSmashable)
        {
            float wiggleRoom = 0.5f;
            if (
                (controller.stateController.currentState.Equals(State.Jumping) || controller.stateController.currentState.Equals(State.JumpingOnJumpPad) || controller.stateController.currentState.Equals(State.WallSliding)) &&
                controller.boxCollider2D.bounds.max.y < other.bounds.min.y + wiggleRoom &&
                controller.boxCollider2D.bounds.max.x > other.bounds.min.x - wiggleRoom)
            {
                ((IHeadSmashable)mono).OnHeadSmashed(this);
                return;
            }
        }

        if (mono is IDangerous)
        {
            if (controller.boxCollider2D.enabled == false)
                return;

            Hurt();
            return;
        }
    }

    private void Reset()
    {
        keys = new List<Key>();
        pivotTransform.localPosition = Vector2.zero;
        pivotTransform.eulerAngles = Vector3.zero;
        slime.Disable();
    }

    private void On_PlayerStartedRunning(FSM_CharacterController controller)
    {
        controller.boxCollider2D.enabled = true;
    }

    private void On_LevelReset(bool manual) => Reset();

    private void OnEnable()
    {
        GameMaster.On_EnterPlayMode += Reset;
        GameMaster.On_LevelReset += On_LevelReset;

        GameMaster.On_PlayerStartedRunning += On_PlayerStartedRunning;

		SuspensionManager.On_SuspensionEvent += On_SuspensionEvent;
    }

    private void Unsubscribe()
    {
        GameMaster.On_EnterPlayMode -= Reset;
        GameMaster.On_LevelReset -= On_LevelReset;

        GameMaster.On_PlayerStartedRunning -= On_PlayerStartedRunning;
		
		SuspensionManager.On_SuspensionEvent -= On_SuspensionEvent;
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();
        KillAllSounds();
        KillAllTweens();
    }

    private void KillAllSounds()
    {
        if (Audio.Exsists())
        {
            List<AudioPlayer> objects = Audio.GetAudioObjectsInChannel(Channel.Player);
            foreach (AudioPlayer aO in objects)
            {
                aO.Kill();
            }
        }
    }

	private void KillAllTweens()
	{
        transform.DOKill();
        
		if(tweens == null)
			return;

		foreach(Tween t in tweens)
			t?.Kill();

		tweens = null;
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
