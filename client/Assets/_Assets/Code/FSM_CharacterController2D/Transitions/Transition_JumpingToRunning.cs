using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM_CharacterController2D
{
    public class Transition_JumpingToRunning
    {
        public static bool ConditionsMet(FSM_CharacterController characterController)
        {
            bool conditionsMet = 
                characterController.motion.combinedVelocity.y == 0f && characterController.collisionInfo.below;

            if(conditionsMet)
                characterController.stateController.SetState(State.Running);

            return conditionsMet;
        }
    }
}

