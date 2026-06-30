using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM_CharacterController2D
{
    public class Transition_FallingToLanding
    {
        public static bool ConditionsMet(FSM_CharacterController characterController)
        {
            bool conditionsMet = 
                (characterController.collisionInfo.below || characterController.motion.standingOnSlope) && 
                !(characterController.motion.touchingSlime && characterController.motion.peakFallingVelocity < characterController.properties.minmumStuckInSlimeVelocity);

            if(conditionsMet)
                characterController.stateController.SetState(State.Landing);

            return conditionsMet;
        }
    }
}