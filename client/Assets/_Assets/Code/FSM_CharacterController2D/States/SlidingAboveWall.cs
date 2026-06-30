using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM_CharacterController2D
{
    public class SlidingAboveWall : IState
    {
        public State stateEnum{
            get{
                return State.FallingOfWall;
            }
        }
        public FSM_CharacterController characterController{get; set;}
        public void  SetReferenceToCharacter(FSM_CharacterController characterController)
        {
            this.characterController = characterController;
        }

        public void OnEnter(State previousState)
        {      
            characterController.motion.runningDirection *= -1;
            characterController.motion.runningVelocity = characterController.properties.movementSpeed * characterController.motion.runningDirection;
        }

        public void OnExit(State newState)
        {
        }

        public void Act()
        {
        }

        public bool CheckTransitions()
        {
            //These are shared by Falling.cs //Replace with duplicates before changing
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
