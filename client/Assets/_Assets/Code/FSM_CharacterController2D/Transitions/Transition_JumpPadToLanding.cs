using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM_CharacterController2D
{
    public class Transition_JumpPadToLanding
    {
        public static bool ConditionsMet(FSM_CharacterController characterController, int frameEntered, bool jumpStarted)
        {
            bool conditionsMet = (characterController.collisionInfo.below || characterController.motion.standingOnSlope) && !characterController.motion.touchingSlime && frameEntered < Time.frameCount && !jumpStarted;
            
            if(conditionsMet)
                characterController.stateController.SetState(State.Landing);

            return conditionsMet;
        }
    }
}