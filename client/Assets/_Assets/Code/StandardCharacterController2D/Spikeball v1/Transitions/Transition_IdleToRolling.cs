using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StandardCharacterController2D.Spikeball.V1
{
    public class Transition_IdleToRolling
    {
        public static bool ConditionsMet(Spikeball_CharacterController2D controller2D)
        {
            bool isStandingOnSlope = ((MotionController)controller2D.MotionController).slopeController.IsStandingOnSlope();
            bool conditionsMet = (controller2D.CollisionController.GetCollisionInfo().below || isStandingOnSlope);

            if(conditionsMet)
                controller2D.StateController.SetState(new Rolling());

            return conditionsMet;
        }
    }
}
