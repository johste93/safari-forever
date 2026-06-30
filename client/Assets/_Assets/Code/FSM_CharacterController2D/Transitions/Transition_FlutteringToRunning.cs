using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM_CharacterController2D
{
    public class Transition_FlutteringToRunning
    {
        public static bool ConditionsMet(FSM_CharacterController characterController)
        {
            bool conditionsMet = characterController.collisionInfo.below;

            if(conditionsMet)
                characterController.stateController.SetState(State.Running);

            return conditionsMet;
        }
    }
}
