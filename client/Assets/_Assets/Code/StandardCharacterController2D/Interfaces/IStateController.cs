using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StandardCharacterController2D 
{
    public interface IStateController
    {
        void SetState(IState newState);
        void Act();
        void CheckTransitions();
    }
}
