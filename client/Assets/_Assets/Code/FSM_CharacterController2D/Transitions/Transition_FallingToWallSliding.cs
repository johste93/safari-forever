using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM_CharacterController2D
{
    public class Transition_FallingToWallSliding
    {
        public static bool ConditionsMet(FSM_CharacterController characterController)
        {
            bool conditionsMet = 
                (characterController.collisionInfo.left || characterController.collisionInfo.right) && !characterController.motion.touchingSlime;

            if(conditionsMet)
                characterController.stateController.SetState(State.WallSliding);

            return conditionsMet;
        }
    }
}
