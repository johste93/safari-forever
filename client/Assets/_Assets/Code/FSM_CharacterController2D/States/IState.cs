using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM_CharacterController2D
{
    public interface IState
    {
        State stateEnum{get;}
        FSM_CharacterController characterController{get; set;}
        void SetReferenceToCharacter(FSM_CharacterController characterController);

        void OnEnter(State previousState);
        void OnExit(State newState);
        void Act();
        bool CheckTransitions();
        bool Equals(State state);
    }
}
    
