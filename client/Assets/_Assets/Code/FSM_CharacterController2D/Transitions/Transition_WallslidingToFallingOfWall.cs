using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM_CharacterController2D
{
    public class Transition_WallslidingToFallingOfWall
    {
        public static bool ConditionsMet(FSM_CharacterController characterController)
        {
            bool conditionsMet = 
                !characterController.collisionInfo.left && !characterController.collisionInfo.right && characterController.motion.combinedVelocity.y < 0f;

            if(conditionsMet)
                characterController.stateController.SetState(State.FallingOfWall);

            return conditionsMet;
        }
    }
}