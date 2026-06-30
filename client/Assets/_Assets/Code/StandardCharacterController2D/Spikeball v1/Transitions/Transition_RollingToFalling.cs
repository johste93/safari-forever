using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StandardCharacterController2D.Spikeball.V1
{
    public class Transition_RollingToFalling
    {
        public static bool ConditionsMet(Spikeball_CharacterController2D controller2D)
        {
            bool conditionsMet = 
                controller2D.MotionController.GetCombinedVelocity().y < 0f;

            if(conditionsMet)
                controller2D.StateController.SetState(new Falling());

            return conditionsMet;
        }
    }
}
