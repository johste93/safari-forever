using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM_CharacterController2D
{
    public class Falling : IState
    {
        public State stateEnum{
            get{
                return State.Falling;
            }
        }
        public FSM_CharacterController characterController{get; set;}
        public void  SetReferenceToCharacter(FSM_CharacterController characterController)
        {
            this.characterController = characterController;
        }

        public void OnEnter(State previousState)
        {
            characterController.motion.peakFallingVelocity = 0;
        }

        public void OnExit(State newState)
        {
        }

        public void Act()
        {
            if(characterController.motion.combinedVelocity.y < characterController.motion.peakFallingVelocity)
                characterController.motion.peakFallingVelocity = characterController.motion.combinedVelocity.y;
        }

        public bool CheckTransitions()
        {
            return 
                Transition_FallingToStuckInSlime.ConditionsMet(characterController) ||
                Transition_FallingToLanding.ConditionsMet(characterController) ||
                Transition_FallingToStuckInSlimeOnWall.ConditionsMet(characterController) ||
                Transition_FallingToWallSliding.ConditionsMet(characterController) ||
                Transition_FallingToFluttering.ConditionsMet(characterController);
        }

        
        public bool Equals(State state)
        {
            return state == stateEnum;
        }
    }
}
