using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StandardCharacterController2D
{
    public interface IState
    {
        string Name{get;}

        void Initalize(CharacterController2D characterController2D);
        void OnEnter(IState previousState);
        void OnExit(IState newState);
        void Act();
        bool CheckTransitions();
        bool Equals(IState state);
    }
}
    