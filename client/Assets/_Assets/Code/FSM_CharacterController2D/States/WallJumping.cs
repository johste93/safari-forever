using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM_CharacterController2D
{
    public class WallJumping : IState
    {   
        public State stateEnum{
            get{
                return State.WallJumping;
            }
        }
        public FSM_CharacterController characterController{get; set;}
        public void  SetReferenceToCharacter(FSM_CharacterController characterController)
        {
            this.characterController = characterController;
        }

        public void OnEnter(State previousState)
        {      
            characterController.motion.runningVelocity = characterController.properties.movementSpeed * characterController.motion.runningDirection;
            float pitchIncrease = 0.2f;
            float maxPitch = 2f;

            if(characterController.audio.clipIndex < SFX.instance.character.jump.clips.Length-1)
                characterController.audio.clipIndex++;
                
            characterController.audio.jumpPitch += pitchIncrease;
            if(characterController.audio.jumpPitch > maxPitch)
                characterController.audio.jumpPitch = maxPitch - (pitchIncrease * 2);
        }

        public void OnExit(State newState)
        {
        }
        
        public void Act()
        {
        }

        public bool CheckTransitions()
        {
            return Transition_WallJumpingToJumping.ConditionsMet(characterController);
        }
        
        public bool Equals(State state)
        {
            return state == stateEnum;
        }
    }
}