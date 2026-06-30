using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM_CharacterController2D;

public class TailAnimationController : MonoBehaviour
{
    public Animator tailAnimator;
	private const float  sensitivty = 0.33f;
	public  FSM_CharacterController characterController;

	private float maxJumpForce;

	private void OnEnable()
	{
		tailAnimator.speed = SaveManager.currentSave.gameSpeed;
		maxJumpForce = PhysicsHelper.CalculateSpeedFromHeight(characterController.properties.maxJumpHeight, characterController.properties.gravity);
	}

	private void Update () 
	{
		tailAnimator.enabled = !(characterController.stateController.currentState.Equals(State.StuckInSlime) || characterController.stateController.currentState.Equals(State.StuckInSlimeOnWall));

		float t = 0f;
		float yVelocity = characterController.motion.combinedVelocity.y;;
		if(yVelocity >= 0)
		{
			t = yVelocity / maxJumpForce;
		}
		else
		{
			t = -( Mathf.Abs(yVelocity) / (Mathf.Abs(characterController.properties.maxFallSpeed) * sensitivty));
		}

		if(characterController.motion.standingOnSlope)
			t = 0f;

		tailAnimator.SetFloat("VerticalVelocity", t);
	}
}
