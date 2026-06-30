using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM_CharacterController2D
{
    public class MovementController 
    {
        public FSM_CharacterController characterController;
        public GravityController gravityController;
        public CollisionController collisionController;
        public SlopeController slopeController;

        public MovementController(FSM_CharacterController characterController)
        {
            this.characterController = characterController;
            
            gravityController = new GravityController();
            gravityController.SetReferenceToCharacter(characterController);

            collisionController = new CollisionController();
            collisionController.SetReferenceToCharacter(characterController);

            slopeController = new SlopeController();
            slopeController.SetReferenceToCharacter(characterController);
        }

        public void UpdateRigidbody()
        {   
            //Apply gravity
            gravityController.ApplyGravity();

            //Handle slopes
            slopeController.HandleSlopes();

            //Check collisions
            collisionController.UpdateCollisions();

            //Finally
            characterController.rigidBody2D.position += characterController.motion.clampedVelocity * Time.fixedDeltaTime;
        }
    }
}
