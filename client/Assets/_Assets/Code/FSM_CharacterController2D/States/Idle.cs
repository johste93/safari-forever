using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM_CharacterController2D
{
    public class Idle : IState
    {   
        public State stateEnum{
            get{
                return State.Idle;
            }
        }
        public FSM_CharacterController characterController{get; set;}
        public void  SetReferenceToCharacter(FSM_CharacterController characterController)
        {
            this.characterController = characterController;
        }

        public void OnEnter(State previousState)
        {      
            characterController.motion.runningVelocity = 0f;
        }

        public void OnExit(State newState)
        {
            characterController.inputController.ConsumeInput();
        }

        public void Act()
        {
        }

        public bool CheckTransitions()
        {
            return Transition_IdleToRunning.ConditionsMet(characterController);
        }

        public bool Equals(State state)
        {
            return state == stateEnum;
        }
    }
}
