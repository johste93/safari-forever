using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StandardCharacterController2D.Spikeball.V1
{
    public class GravityController
    {
        private Spikeball_CharacterController2D controller2D;

        public GravityController(Spikeball_CharacterController2D characterController)
        {
            this.controller2D = characterController;
        }

        public void ApplyGravity(ref Vector2 velocity)
        {
            if(velocity.y > controller2D.MaxFallSpeed)
                velocity.y += controller2D.Gravity * Time.fixedDeltaTime * SaveManager.currentSave.gameSpeed;
        }
    }
}