using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM_CharacterController2D
{
    public class WallSliding : IState
    {   
        public State stateEnum{
            get{
                return State.WallSliding;
            }
        }
        public FSM_CharacterController characterController{get; set;}
        public void  SetReferenceToCharacter(FSM_CharacterController characterController)
        {
            this.characterController = characterController;
        }

        private AudioPlayer slideAudioObject;

        public void OnEnter(State previousState)
        {            
            characterController.motion.timeSpentFluttering = 0f;
            characterController.motion.runningVelocity = 0f;
            slideAudioObject = Audio.Play(SFX.instance.character.slide, Channel.Player);
        }

        public void OnExit(State newState)
        {  
            //Debug.Log(newState);
            //if(newState == State.Bouncing)
                //return;

            //Jump away from wall
            if(newState != State.FallingOfWall)
                characterController.motion.runningDirection *= -1;
            
            slideAudioObject.Kill();
        }

        public void Act()
        {
            float deltaGravityToBeApplied = characterController.properties.gravity * characterController.motion.gravityScale * Time.fixedDeltaTime * SaveManager.currentSave.gameSpeed;
            float maxWallSLideSpeed = characterController.properties.maxWallSlideSpeed + deltaGravityToBeApplied;


            float maxUpwardSpeed = 8f;
            characterController.motion.rawVelocity.y = Mathf.Clamp(characterController.motion.rawVelocity.y, -Mathf.Abs(maxWallSLideSpeed), maxUpwardSpeed);
        }

        public bool CheckTransitions()
        {
            return 
                Transition_WallSlidingToRunning.ConditionsMet(characterController) ||
                Transition_WallSlidingToWallJumping.ConditionsMet(characterController) ||
                Transition_WallslidingToFallingOfWall.ConditionsMet(characterController) ||
                Transition_WallslidingToSlidingAboveWall.ConditionsMet(characterController);
        }
        
        public bool Equals(State state)
        {
            return state == stateEnum;
        }
    }
}