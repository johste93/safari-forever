using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM_CharacterController2D
{
    public class FallingWithoutControl : IState
    {
        public State stateEnum{
            get{
                return State.FallingWithoutControl;
            }
        }
        public FSM_CharacterController characterController{get; set;}
        public void  SetReferenceToCharacter(FSM_CharacterController characterController)
        {
            this.characterController = characterController;
        }

        public void OnEnter(State previousState)
        {      
        }

        public void OnExit(State newState)
        {
        }

        public void Act()
        {
        }

        public bool CheckTransitions()
        {
            return 
                Transition_FallingToStuckInSlime.ConditionsMet(characterController) || 
                Transition_FallingToLanding.ConditionsMet(characterController) ||
                Transition_FallingToStuckInSlimeOnWall.ConditionsMet(characterController) ||
                Transition_FallingToWallSliding.ConditionsMet(characterController);
        }

        public bool Equals(State state)
        {
            return state == stateEnum;
        }
    }
}