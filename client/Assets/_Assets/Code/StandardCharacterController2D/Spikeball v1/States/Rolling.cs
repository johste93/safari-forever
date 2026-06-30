using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StandardCharacterController2D.Spikeball.V1
{
    public class Rolling : IState
    {
        public string Name {
            get {
                return this.GetType().Name;
            }
        }

        private Spikeball_CharacterController2D controller2D;
        private MotionController motionController;

        public void Initalize(CharacterController2D characterController2D)
        {
            this.controller2D = (Spikeball_CharacterController2D) characterController2D;
            motionController = (StandardCharacterController2D.Spikeball.V1.MotionController) controller2D.MotionController;
        }

        public void OnEnter(IState previousState)
        {
            Vector2 currentVelocity = motionController.GetInternalVelocity();
            motionController.SetInternalVelocity(new Vector2(controller2D.MovementSpeed * motionController.MovingDirection, currentVelocity.y));
        }

        public void OnExit(IState newState)
        {
        }

        public void Act()
        {
            if( motionController.MovingDirection == 1 ? controller2D.CollisionController.GetCollisionInfo().right : controller2D.CollisionController.GetCollisionInfo().left)
            {
                motionController.MovingDirection *= -1;
                Vector2 currentVelocity = motionController.GetInternalVelocity();
                motionController.SetInternalVelocity(new Vector2(controller2D.MovementSpeed * motionController.MovingDirection, currentVelocity.y));
                //Audio.Play(SFX.instance.character.turn);
            }
        }

        public bool CheckTransitions()
        {
            return Transition_RollingToFalling.ConditionsMet(controller2D);
        }

        public bool Equals(IState state)
        {
            return Name == state.Name;
        }
    }
}
