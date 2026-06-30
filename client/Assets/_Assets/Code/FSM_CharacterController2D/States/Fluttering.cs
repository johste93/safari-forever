using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM_CharacterController2D
{
    public class Fluttering : IState
    {   
        public State stateEnum{
            get{
                return State.Fluttering;
            }
        }
        public FSM_CharacterController characterController{get; set;}
        public void  SetReferenceToCharacter(FSM_CharacterController characterController)
        {
            this.characterController = characterController;
        }

        public void OnEnter(State previousState)
        {      
            characterController.motion.rawVelocity.y = characterController.properties.flutterJumpMinimumVelocity;
            characterController.motion.runningVelocity = characterController.properties.movementSpeed * characterController.motion.runningDirection;

            flutterObj = Audio.Play(SFX.instance.character.flutter.randomClip, Channel.Player);
        }

        private AudioPlayer flutterObj;

        public void OnExit(State newState)
        {
            //characterController.motion.timeSpentFluttering = 0f;
            flutterObj.Kill();
        }

        public void Act()
        {
            characterController.motion.rawVelocity.y += characterController.properties.flutterJumpCounterForce * Time.fixedDeltaTime * SaveManager.currentSave.gameSpeed;
            characterController.motion.timeSpentFluttering += Time.fixedDeltaTime * SaveManager.currentSave.gameSpeed;

        }

        public bool CheckTransitions()
        {
            return 
                Transition_FlutteringToStuckInSlimeOnWall.ConditionsMet(characterController) ||
                Transition_FlutteringToWallsliding.ConditionsMet(characterController) ||
                Transition_FlutteringToRunning.ConditionsMet(characterController) ||
                Transition_FlutteringToFalling.ConditionsMet(characterController);
        }
        
        public bool Equals(State state)
        {
            return state == stateEnum;
        }
    }
}