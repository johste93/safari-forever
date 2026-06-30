using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM_CharacterController2D
{
    public class Transition_JumpPadToStuckInSlimeOnRoof
    {
        public static bool ConditionsMet(FSM_CharacterController characterController)
        {
            bool conditionsMet = 
                characterController.motion.touchingSlime &&
                characterController.collisionInfo.above;

            if(conditionsMet)
                characterController.stateController.OverideState(State.StuckInSlimeOnRoof);

            return conditionsMet;
        }
    }
}
