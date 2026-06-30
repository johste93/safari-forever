using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StandardCharacterController2D.Spikeball.V1
{
    public class StateController : IStateController
    {   
        public CharacterController2D characterController;
        public IState currentState;

        public delegate void StateChangedEvent(IState previousState, IState newState);
        public StateChangedEvent OnStateChanged;
        
        public StateController(CharacterController2D characterController)
        {
            this.characterController = characterController;
            currentState = (IState) new Idle();
        }

        public void SetState(IState newState)
        {
            IState previousState = null;

            if(currentState != null)
                previousState = currentState;

            //Debug.Log(Time.frameCount + ": " + currentState.Name + " -> " + newState.Name);

            if(currentState != null)
                currentState.OnExit(newState);
                
            currentState = newState;
            currentState.Initalize(characterController);
            currentState.OnEnter(previousState);

            if(OnStateChanged != null)
                OnStateChanged(previousState, newState);
            //CheckTransitions(); //Prone to creating infite loops! but speeds up transitions.
        }

        public void Act()
        {
            currentState.Act();
        }

        public void CheckTransitions()
        {
            currentState.CheckTransitions();
        }
    }
}

