using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM_CharacterController2D
{
    public class Transition_IdleToRunning
    {
        public static bool ConditionsMet(FSM_CharacterController characterController)
        {
            bool conditionsMet = JumpedWithinWiggleRoom(characterController) && (characterController.collisionInfo.below || characterController.motion.standingOnSlope);

            if(conditionsMet)
                characterController.stateController.SetState(State.Running);

            return conditionsMet;
        }

        public static bool JumpedWithinWiggleRoom(FSM_CharacterController characterController)
        {
            return (Time.time - characterController.motion.timeOfLastJumpAttempt) < characterController.properties.wiggleRoomSeconds;
        }
    }
}