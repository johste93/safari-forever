using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM_CharacterController2D
{
    public class Bouncing : IState
    {   
        public State stateEnum{
            get{
                return State.Bouncing;
            }
        }
        public FSM_CharacterController characterController{get; set;}
        public void  SetReferenceToCharacter(FSM_CharacterController characterController)
        {
            this.characterController = characterController;
        }

        public void OnEnter(State previousState)
        {      
            Audio.Play( SFX.instance.character.bounce.randomClip, Channel.Game ).SetPitch( Random.Range(0.8f, 1.2f) ).SetVolume(0.3f);
            characterController.motion.runningVelocity = characterController.properties.movementSpeed * characterController.motion.runningDirection;
        }

        public void OnExit(State newState)
        {
        }

        public void Act()
        {
        }

        public bool CheckTransitions()
        {
            return
                Transition_BouncingToJumping.ConditionsMet(characterController);
        }

        public bool Equals(State state)
        {
            return state == stateEnum;
        }
    }
}
