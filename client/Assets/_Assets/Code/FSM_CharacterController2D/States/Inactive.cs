using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM_CharacterController2D
{
    public class Inactive : IState
    {   
        public State stateEnum{
            get{
                return State.Inactive;
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
            characterController.inputController.ConsumeInput();

            if(newState != State.Dead)
                GameMaster.instance?.StartRunning(characterController);

            TouchInput.CancelTouch();
        }

        public void Act()
        {
        }

        public bool CheckTransitions()
        {
            characterController.movementController.collisionController.UpdateCollisions();
            return  Transition_InactiveToRunning.ConditionsMet(characterController) || 
                    Transition_InactiveToFalling.ConditionsMet(characterController);
        }

        public bool Equals(State state)
        {
            return state == stateEnum;
        }
    }
}
