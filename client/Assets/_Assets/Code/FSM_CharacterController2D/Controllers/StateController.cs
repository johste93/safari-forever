using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM_CharacterController2D
{
    public class StateController
    {   
        public FSM_CharacterController characterController;
        public IState currentState;

        public delegate void StateChangedEvent(State previousState, State newState);
        public StateChangedEvent OnStateChanged;

        private bool stateChanged;
        private State nextState;
        
        public StateController(FSM_CharacterController characterController)
        {
            this.characterController = characterController;
        }

        public void SetState(State state)
        {
            if(stateChanged)
                return;

            nextState = state;
            stateChanged = true;
        }

        public void OverideState(State state)
        {
            nextState = state;
            stateChanged = true;
            UpdateState();
        }

        public void UpdateState()
        {
            if(!stateChanged)
                return;

            IState targeState = StateMap.LookUp(nextState);
            State previousState = State.Idle;

            if(currentState != null)
                previousState = currentState.stateEnum;

            //Debug.Log(Time.frameCount + ": " + currentState + " -> " + nextState);

            if(currentState != null)
                currentState.OnExit(nextState);
                
            currentState = targeState;
            currentState.SetReferenceToCharacter(characterController);
            currentState.OnEnter(previousState);

            if(OnStateChanged != null)
                OnStateChanged(previousState, nextState);

            stateChanged = false;
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
