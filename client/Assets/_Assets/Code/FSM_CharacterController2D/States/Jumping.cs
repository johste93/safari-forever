using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM_CharacterController2D
{
    public class Jumping : IState
    {   
        public delegate void JumpEvent(FSM_CharacterController controller);
        public static JumpEvent OnPlayerJumped;

        public State stateEnum{
            get{
                return State.Jumping;
            }
        }
        public FSM_CharacterController characterController{get; set;}
        public void  SetReferenceToCharacter(FSM_CharacterController characterController)
        {
            this.characterController = characterController;
        }

        public void OnEnter(State previousState)
        {      
            OnPlayerJumped?.Invoke(characterController);

            characterController.motion.timeOfLastJumpAttempt = 0f;
            characterController.motion.rawVelocity.y = PhysicsHelper.CalculateSpeedFromHeight(characterController.properties.maxJumpHeight, characterController.properties.gravity);

            Audio.Play(SFX.instance.character.jump.clips[characterController.audio.clipIndex], Channel.Player);
        }

        public void OnExit(State newState)
        {
            characterController.motion.gravityScale = 1f;
        }
        
        public void Act()
        {
            if(characterController.inputInfo.onButtonPressed)
            {
                characterController.motion.gravityScale = 1f;	
            }	
            else
                characterController.motion.gravityScale = 1f/characterController.properties.minimumJumpPercent;

        }


        public bool CheckTransitions()
        {
            return 
                Transition_JumpingToWallSliding.ConditionsMet(characterController) ||
                Transition_JumpingToFalling.ConditionsMet(characterController) ||
                Transition_JumpingToRunning.ConditionsMet(characterController) ||
                Transition_JumpingToStuckInSlimeOnRoof.ConditionsMet(characterController) ||
                Transition_JumpingToStuckInSlimeOnWall.ConditionsMet(characterController);
        }
        
        public bool Equals(State state)
        {
            return state == stateEnum;
        }
    }
}