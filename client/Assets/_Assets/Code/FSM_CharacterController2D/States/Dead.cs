using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM_CharacterController2D;

namespace FSM_CharacterController2D
{
    public class Dead : IState
    {
        public State stateEnum{
                get{
                    return State.Dead;
                }
            }
            public FSM_CharacterController characterController{get; set;}
            public void SetReferenceToCharacter(FSM_CharacterController characterController)
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
                return false;
            }

            public bool Equals(State state)
            {
                return state == stateEnum;
            }
    }
}
