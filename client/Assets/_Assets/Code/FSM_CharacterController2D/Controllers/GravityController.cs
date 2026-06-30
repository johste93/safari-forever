using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM_CharacterController2D
{
    public class GravityController
    {
        private FSM_CharacterController characterController;

        public void SetReferenceToCharacter(FSM_CharacterController characterController)
        {
            this.characterController = characterController;
        }

        public void ApplyGravity()
        {
            if(characterController.stateController.currentState.Equals(State.StuckInSlime) || characterController.stateController.currentState.Equals(State.StuckInSlimeOnWall) || characterController.stateController.currentState.Equals(State.StuckInSlimeOnRoof))
                return;
                
            if(characterController.motion.rawVelocity.y > characterController.properties.maxFallSpeed)
                characterController.motion.rawVelocity.y += characterController.properties.gravity * characterController.motion.gravityScale * Time.fixedDeltaTime * SaveManager.currentSave.gameSpeed;
        }
    }
}
