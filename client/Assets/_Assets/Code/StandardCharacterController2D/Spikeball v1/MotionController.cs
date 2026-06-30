using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StandardCharacterController2D.Spikeball.V1
{
    public class MotionController : IMotionController
    {
        private Spikeball_CharacterController2D controller2D;
        public GravityController gravityController;
        public SlopeController slopeController;

        private Vector2 conveyorVelocity;
        private Vector2 internalVelocity;
        private Vector2 clampedVelocity; 

        public float MovingDirection;

        public MotionController(Spikeball_CharacterController2D characterController2D)
        {
            this.controller2D = characterController2D;
            gravityController = new GravityController(characterController2D);
            slopeController = new SlopeController(characterController2D);
        }

        public void UpdateRigidbody()
        {
            //Apply gravity
            gravityController.ApplyGravity(ref internalVelocity);

            //Handle slopes
            slopeController.HandleSlopes();

            //Check collisions
            controller2D.CollisionController.UpdateCollisions();
            
            //Finally Apply Motion
            controller2D.Rigidbody2D.position += clampedVelocity * Time.fixedDeltaTime;
        }

        public Vector2 GetInternalVelocity()
        {
            return internalVelocity;
        }

        public Vector2 GetCombinedVelocity()
        {
            return internalVelocity + conveyorVelocity;
        }

        public Vector2 GetClampedVelocity()
        {
            return clampedVelocity;
        }

        public void SetInternalVelocityX(float x)
        {
            Vector2 velocity = GetInternalVelocity();
            velocity.x = x;
            SetInternalVelocity(velocity);
        }

        public void SetInternalVelocityY(float y)
        {
            Vector2 velocity = GetInternalVelocity();
            velocity.y = y;
            SetInternalVelocity(velocity);
        }

        public void SetInternalVelocity(Vector2 clampedVelocity)
        {
            this.internalVelocity = clampedVelocity; 
        }

        public void SetClampedVelocityX(float x)
        {
            Vector2 velocity = GetClampedVelocity();
            velocity.x = x;
            SetClampedVelocity(velocity);
        }

        public void SetClampedVelocityY(float y)
        {
            Vector2 velocity = GetClampedVelocity();
            velocity.y = y;
            SetClampedVelocity(velocity);
        }

        public void SetClampedVelocity(Vector2 clampedVelocity)
        {
            this.clampedVelocity = clampedVelocity; 
        }

        public void SetConveyorVelocity(Vector2 conveyorVelocity)
        {
            conveyorVelocity.y = 0;
            this.conveyorVelocity = conveyorVelocity;
        }

    }
}
