using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM_CharacterController2D
{
    public class Transition_FallingToFluttering
    {
        public static bool ConditionsMet(FSM_CharacterController characterController)
        {
            //If player will hit ground within two frames prevent flutterjump.
            float flutterJumpMinimumDistance = Mathf.Abs(characterController.motion.clampedVelocity.y) * Time.fixedDeltaTime * 2;

            bool conditionsMet = 
                characterController.inputInfo.onButtonPressed &&
                characterController.motion.combinedVelocity.y <= characterController.properties.flutterJumpMinimumVelocity &&
                characterController.motion.timeSpentFluttering < characterController.properties.flutterJumpDuration &&
                characterController.motion.distanceToGround > flutterJumpMinimumDistance;

            if(conditionsMet)
                characterController.stateController.SetState(State.Fluttering);

            return conditionsMet;
        }
    }
}