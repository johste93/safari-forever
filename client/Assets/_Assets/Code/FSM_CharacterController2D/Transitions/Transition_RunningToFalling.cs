using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM_CharacterController2D{
    public class Transition_RunningToFalling
    {
        public static bool ConditionsMet(FSM_CharacterController characterController)
        {
            bool conditionsMet = 
                characterController.motion.combinedVelocity.y < 0f &&
                !characterController.motion.standingOnSlope;

            if(conditionsMet)
                characterController.stateController.SetState(State.Falling);

            return conditionsMet;
        }
    }
}
