using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM_CharacterController2D
{
    public class Transition_WallJumpingToJumping
    {
        public static bool ConditionsMet(FSM_CharacterController characterController)
        {
            bool conditionsMet = true; //Always true!

            if(conditionsMet)
                characterController.stateController.SetState(State.Jumping);

            return conditionsMet;
        }
    }
}
