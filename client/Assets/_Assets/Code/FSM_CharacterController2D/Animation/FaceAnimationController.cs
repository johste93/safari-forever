using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM_CharacterController2D;

public class FaceAnimationController : MonoBehaviour
{

    public FSM_CharacterController characterController;
    public Animator faceAnimator; 

    private float t;
    private float maxJumpForce;

    private void OnEnable()
	{
		if(faceAnimator != null)
			faceAnimator.speed = SaveManager.currentSave.gameSpeed;
		maxJumpForce = FSM_CharacterController2D.PhysicsHelper.CalculateSpeedFromHeight(characterController.properties.maxJumpHeight, characterController.properties.gravity);
	}

	private void Update () 
	{
		faceAnimator.enabled = !(characterController.stateController.currentState.Equals(State.StuckInSlime) || characterController.stateController.currentState.Equals(State.StuckInSlimeOnWall));

		if(faceAnimator.speed < Mathf.Epsilon)
			return;

		if(Mathf.Abs(characterController.motion.combinedVelocity.y ) >= 1f)
		{
			t = Mathf.Lerp(t, Mathf.Sign(characterController.motion.combinedVelocity.y), 0.1f);
		}
		else
		{
			t = Mathf.Lerp(t, 0f, 0.1f);
		}

		if(characterController.motion.standingOnSlope)
			t = 0f;

		if(faceAnimator != null)
			faceAnimator.SetFloat("VerticalVelocity", t);
	}
}
