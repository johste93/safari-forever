using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM_CharacterController2D
{
    public class Transition_FlutteringToFalling
    {
        public static bool ConditionsMet(FSM_CharacterController characterController)
        {
            bool conditionsMet = 
                !characterController.inputInfo.onButtonPressed ||
                characterController.motion.timeSpentFluttering >= characterController.properties.flutterJumpDuration;


            if(conditionsMet)
                characterController.stateController.SetState(State.Falling);

            return conditionsMet;
        }
    }
}