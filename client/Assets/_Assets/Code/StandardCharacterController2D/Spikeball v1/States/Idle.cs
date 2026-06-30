using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StandardCharacterController2D.Spikeball.V1
{
    public class Idle : IState
    {
        public string Name { get { return this.GetType().Name; } }

        private Spikeball_CharacterController2D controller2D;

        public void Initalize(CharacterController2D characterController2D)
        {
            this.controller2D = (Spikeball_CharacterController2D) characterController2D;
        }

        public void OnEnter(IState previousState)
        {
            controller2D.MotionController.SetInternalVelocityX(0f);
        }

        public void OnExit(IState newState)
        {
        }

        public void Act()
        {
        }

        public bool CheckTransitions()
        {
            return 
                Transition_IdleToFalling.ConditionsMet(controller2D) ||
                Transition_IdleToRolling.ConditionsMet(controller2D);
        }

        public bool Equals(IState state)
        {
            return Name == state.Name;
        }
    }
}