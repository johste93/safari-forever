using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM_CharacterController2D;

public class SmokeParticleController : MonoBehaviour
{
    public GameObject smokePuffPrefab;
	public ParticleSystem wallSlidingTrail;
	public ParticleSystem flutterTrail;
	public ParticleSystem jumpTrail;

	public bool onLand = true;
	public bool onJump = true;
	public bool onWallJump = true;
	public float jumpTrailDuration = 0.15f;

    public FSM_CharacterController characterController;

	private ParticleSystem.EmissionModule wallSlidingTrailEmission;
	private ParticleSystem.EmissionModule flutterTrailEmission;
	private ParticleSystem.EmissionModule jumpTrailEmission;

	private float lastJumpTime;
	
	private bool wasWallslidingLastFrame;
	private int lastFrameWallsliding;
	private int lastFrameOfStuckInSlime;

	private void Awake()
	{
		wallSlidingTrailEmission = wallSlidingTrail.emission;
		flutterTrailEmission = flutterTrail.emission;
		jumpTrailEmission = jumpTrail.emission;

		flutterTrailEmission.enabled = false;
	}

	private void FixedUpdate()
	{
		flutterTrailEmission.enabled = characterController.stateController.currentState.Equals(State.Fluttering) && characterController.boxCollider2D.enabled;
		wallSlidingTrailEmission.enabled = characterController.stateController.currentState.Equals(State.WallSliding);		

		if(characterController.stateController.currentState.Equals(State.WallSliding))
			lastFrameWallsliding = Time.frameCount;

		if(characterController.stateController.currentState.Equals(State.StuckInSlime) || characterController.stateController.currentState.Equals(State.StuckInSlimeOnWall) || characterController.stateController.currentState.Equals(State.StuckInSlimeOnRoof))
			lastFrameOfStuckInSlime = Time.frameCount;
	}

	private void Update()
	{
		jumpTrailEmission.enabled = 
            Time.time - lastJumpTime < jumpTrailDuration &&
            characterController.stateController.currentState.Equals(State.Jumping) &&
            characterController.inputInfo.onButtonDown;
	}

	private void On_BecameGrounded()
	{
		if(!onLand)
			return;

		if(characterController.motion.touchingSlime)
			return;
		
		bool wasWallslidingLessThan3FramesAgo = Time.frameCount - lastFrameWallsliding < 3;

		if(!wasWallslidingLessThan3FramesAgo)
		{
			Vector3 spawnPos = transform.TransformPoint(new Vector3(0f, characterController.motion.combinedVelocity.y * Time.fixedDeltaTime * SaveManager.currentSave.gameSpeed, 0f));
			SpawnPuff(spawnPos, -transform.right * 90);
		}
	}

	private void SpawnPuff(Vector3 position, Vector3 rotation)
	{
		if(!GameMaster.instance.IsPlaying())
			return;

		GameObject lastPuff = Instantiate(smokePuffPrefab, position, Quaternion.Euler(rotation)) as GameObject;
	}


    private void OnStateChanged(State previousState, State newState)
    {
		if(previousState == State.WallJumping)
        {
            lastJumpTime = Time.time;

            if(!onWallJump)
                return;

			bool wasStuckInSlimeLessThan5FramesAgo = Time.frameCount - lastFrameOfStuckInSlime < 5;

			if(!wasStuckInSlimeLessThan5FramesAgo)
			{
				Vector3 spawnPos = transform.parent.TransformPoint(new Vector3(-0.3f * characterController.motion.runningDirection, 0.5f, 0f));

				SpawnPuff(spawnPos, -transform.up * 90 * characterController.motion.runningDirection);	
			}

			return;
        }

        if(newState == State.Jumping)
        {
            lastJumpTime = Time.time;

            if(!onJump)
                return;

            SpawnPuff(transform.position, -transform.right * 90);
			return;
        }
    }

	private Coroutine delay;
	private bool isSubscribed;
	private void On_PlayerStartedRunning(FSM_CharacterController controller)
	{
		if(isSubscribed)
			return;

		if(delay != null)
		{
			StopCoroutine(delay);
			delay = null;
		}

		delay = this.Delay1Frame(()=>{
			characterController.stateController.OnStateChanged += OnStateChanged;
			characterController.movementController.collisionController.On_BecameGrounded += On_BecameGrounded;
			delay = null;
			isSubscribed = true;
		});
	}

	private void OnEnable()
	{
		GameMaster.On_PlayerStartedRunning += On_PlayerStartedRunning;
	}

	private void Unsubscribe()
	{
		if(delay != null)
		{
			StopCoroutine(delay);
			delay = null;
		}

		GameMaster.On_PlayerStartedRunning -= On_PlayerStartedRunning;
		characterController.movementController.collisionController.On_BecameGrounded -= On_BecameGrounded;
        characterController.stateController.OnStateChanged -= OnStateChanged;

		isSubscribed = false;
	}

	private void OnDisable()
	{
		Unsubscribe();
	}

	private void OnDestroy()
	{
		Unsubscribe();
	}
}
