using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM_CharacterController2D;

namespace FSM_CharacterController2D
{
    public class JumpingOnJumpPad : IState
    {   
        public delegate void JumpPadEvent(FSM_CharacterController controller);
        public static JumpPadEvent OnPlayerJumpedOnJumpPad;

        public State stateEnum{
            get{
                return State.JumpingOnJumpPad;
            }
        }
        public FSM_CharacterController characterController{get; set;}
        public void  SetReferenceToCharacter(FSM_CharacterController characterController)
        {
            this.characterController = characterController;
        }

        private int frameEntered = -1;
        private bool jumpStarted = false;

        public void OnEnter(State previousState)
        {      
            OnPlayerJumpedOnJumpPad?.Invoke(characterController);
            jumpStarted = false;
            frameEntered = Time.frameCount;
            characterController.motion.timeSpentFluttering = 0f;
            characterController.motion.timeOfLastJumpAttempt = 0f;        
            characterController.motion.runningVelocity = characterController.properties.movementSpeed * characterController.motion.runningDirection;
        }

        public void OnExit(State newState)
        {
            characterController.motion.gravityScale = 1f;
        }
        
        public void Act()
        {
            if(characterController.inputInfo.onButtonPressed)
            {
                characterController.motion.gravityScale = 1f/(characterController.properties.minimumJumpPercent*1.7f);	
            }	
            else
                characterController.motion.gravityScale = 1f/characterController.properties.minimumJumpPercent;

            if(characterController.motion.combinedVelocity.y > 0)
                jumpStarted = true;
        }


        public bool CheckTransitions()
        {
            return
                Transition_JumpPadToWallSliding.ConditionsMet(characterController) ||
                Transition_JumpPadToFalling.ConditionsMet(characterController) ||
                Transition_JumpPadToStuckInSlimeOnWall.ConditionsMet(characterController) ||
                Transition_JumpPadToLanding.ConditionsMet(characterController, frameEntered, jumpStarted) ||
                Transition_JumpPadToStuckInSlimeOnRoof.ConditionsMet(characterController);
        }
        
        public bool Equals(State state)
        {
            return state == stateEnum;
        }
    }
}