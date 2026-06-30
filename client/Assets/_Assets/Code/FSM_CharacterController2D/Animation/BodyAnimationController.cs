using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FSM_CharacterController2D;

public class BodyAnimationController : MonoBehaviour
{
    public bool animateTurn;
    public FSM_CharacterController characterController;
	public Animator animator;
	
	private float faceDir = 1;
    private Transform pivotTransform;
	
	protected virtual void Awake()
	{
		if(pivotTransform == null)
			pivotTransform = transform;
	}

	protected virtual void Start()
	{
		animator.speed = SaveManager.currentSave.gameSpeed;
		faceDir = Mathf.Sign(characterController.motion.runningDirection);
		pivotTransform.localScale = new Vector3(faceDir * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);	
	}


	protected virtual void Update () 
	{			
		animator.enabled = !(characterController.stateController.currentState.Equals(State.StuckInSlime) || characterController.stateController.currentState.Equals(State.StuckInSlimeOnWall));
			
		bool isGrounded = characterController.collisionInfo.below || characterController.motion.standingOnSlope;

		animator.SetBool("isGrounded", isGrounded);
		animator.SetFloat("horizontalVelocity", characterController.motion.runningVelocity);
		animator.SetFloat("verticalVelocity", (characterController.motion.combinedVelocity.y * Time.fixedDeltaTime * SaveManager.currentSave.gameSpeed) + (isGrounded ? 10f : 0f));
		animator.SetBool("isWallSliding", characterController.stateController.currentState.Equals(State.WallSliding));
		animator.SetBool("isFluttering", characterController.stateController.currentState.Equals(State.Fluttering));

        
		animator.SetBool("isDead", characterController.stateController.currentState.Equals(State.Dead));

		//Disable turning if falling!
		if(
			!characterController.stateController.currentState.Equals(State.Falling) && 
			!characterController.stateController.currentState.Equals(State.FallingOfWall) &&
			!characterController.stateController.currentState.Equals(State.FallingWithoutControl) &&
			!(characterController.stateController.currentState.Equals(State.WallSliding) && characterController.motion.combinedVelocity.y > 0)
		){	
			int wallslidingModifier = characterController.stateController.currentState.Equals(State.WallSliding) || characterController.stateController.currentState.Equals(State.FallingOfWall) ? -1 : 1;

			if(faceDir != Mathf.Sign(characterController.motion.runningDirection) * wallslidingModifier)
			{
				faceDir = Mathf.Sign(characterController.motion.runningDirection) * wallslidingModifier;

				if(animateTurn)
				{
					pivotTransform.DOComplete();
					pivotTransform.DOScaleX(faceDir * Mathf.Abs(transform.localScale.x), 0.15f).SetEase(Ease.InOutQuad);
				}

			}

			if(!animateTurn)
				pivotTransform.localScale = new Vector3(faceDir * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);	

		}

		/*
		float targetRotation = 0f;

		if(characterController.motion.standingOnSlope)
			targetRotation = 22.5f * characterController.motion.slopeDirection;
		*/

		//pivotTransform.eulerAngles = new Vector3(0,0,Mathf.MoveTowardsAngle(pivotTransform.eulerAngles.z, targetRotation, 4.5f));
		//pivotTransform.localPosition = new Vector3(0, Mathf.MoveTowardsAngle(pivotTransform.localPosition.y, characterController.motion.standingOnSlope ? -0.2f : 0f, 0.1f),0);
	}

	public Direction4 GetFacingDirection()
	{
		return faceDir > 0 ? Direction4.Right : Direction4.Left;
	}

	private void OnDestroy()
	{
		KillAllTweens();
	}

	private void KillAllTweens()
	{
		pivotTransform.DOKill();
	}
}
