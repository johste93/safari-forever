using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM_CharacterController2D
{
    public class Landing : IState
    {   
        public State stateEnum{
            get{
                return State.Landing;
            }
        }
        public FSM_CharacterController characterController{get; set;}
        public void  SetReferenceToCharacter(FSM_CharacterController characterController)
        {
            this.characterController = characterController;
        }

        public void OnEnter(State previousState)
        {     
            Audio.Play(SFX.instance.character.land, Channel.Player).SetPitch(Random.Range(1f, 1.2f));
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
                Transition_LandingToRunning.ConditionsMet(characterController);
        }
        
        public bool Equals(State state)
        {
            return state == stateEnum;
        }
    }
}