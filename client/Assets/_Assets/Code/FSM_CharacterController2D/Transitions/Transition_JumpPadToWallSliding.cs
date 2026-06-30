using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM_CharacterController2D
{
    public class Transition_JumpPadToWallSliding
    {
        public static bool ConditionsMet(FSM_CharacterController characterController)
        {
            bool conditionsMet =
                ((characterController.collisionInfo.left && characterController.motion.runningDirection < 0) || (characterController.collisionInfo.right && characterController.motion.runningDirection > 0)) &&
                !characterController.motion.touchingSlime;

            if (conditionsMet)
                characterController.stateController.SetState(State.WallSliding);

            return conditionsMet;
        }
    }
}
